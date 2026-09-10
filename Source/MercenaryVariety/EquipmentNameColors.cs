#if !MV_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Inventory;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory;

namespace MercenaryVariety
{
    internal static class EquipmentNameColors
    {
        private const string PatchId = "MercenaryVariety.EquipmentNameColors";
        private static readonly Color Orange = new Color(1f, 0.55f, 0.15f, 1f);
        private static readonly HashSet<string> ItemIds = new HashSet<string>(StringComparer.Ordinal);
        private static readonly HashSet<string> LockIds = new HashSet<string>(StringComparer.Ordinal);
        private static readonly ConditionalWeakTable<TextWidget, NameBrush> NameBrushes = new ConditionalWeakTable<TextWidget, NameBrush>();
        private static readonly ConditionalWeakTable<InventoryScreenWidget, ItemMenuVM> Menus = new ConditionalWeakTable<InventoryScreenWidget, ItemMenuVM>();
        private static readonly FieldInfo TargetItem = typeof(ItemMenuVM).GetField("_targetItem", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo ComparedItem = typeof(ItemMenuVM).GetField("_comparedItem", BindingFlags.Instance | BindingFlags.NonPublic);
        private static bool _installed;
        private static bool _failed;
        private static bool _reportedUiError;

        private sealed class NameBrush
        {
            internal Brush Original;
            internal Brush Colored;
        }

        internal static void TryInstall()
        {
            if (_installed || _failed)
                return;

            object harmony = null;
            Type harmonyType = null;
            try
            {
                harmonyType = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType("HarmonyLib.Harmony")).FirstOrDefault(t => t != null);
                if (harmonyType == null)
                    return;
                Type harmonyMethodType = harmonyType.Assembly.GetType("HarmonyLib.HarmonyMethod");
                MethodInfo patch = harmonyType.GetMethods().First(m => m.Name == "Patch" && m.GetParameters().Length == 5);
                if (TargetItem == null || ComparedItem == null)
                    throw new MissingFieldException("Inventory tooltip item fields changed.");

                var hooks = new[]
                {
                    Tuple.Create(typeof(CampaignUIHelper).GetMethod("GetItemLockStringID"), nameof(AfterLockId)),
                    Tuple.Create(typeof(GauntletMovie).GetMethod("Load", BindingFlags.Static | BindingFlags.Public), nameof(AfterMovieLoad)),
                    Tuple.Create(typeof(InventoryItemTupleWidget).GetProperty("ItemID").GetSetMethod(), nameof(AfterRowUpdate)),
                    Tuple.Create(typeof(InventoryItemTupleWidget).GetProperty("NameTextWidget").GetSetMethod(), nameof(AfterRowUpdate)),
                    Tuple.Create(typeof(InventoryItemTupleWidget).GetMethod("OnConnectedToRoot", BindingFlags.Instance | BindingFlags.NonPublic), nameof(AfterRowUpdate)),
                    Tuple.Create(typeof(InventoryItemTupleWidget).GetMethod("RefreshState", BindingFlags.Instance | BindingFlags.NonPublic), nameof(AfterRowUpdate)),
                    Tuple.Create(typeof(InventoryScreenWidget).GetMethod("OnLateUpdate", BindingFlags.Instance | BindingFlags.NonPublic), nameof(AfterScreenUpdate))
                };
                if (hooks.Any(h => h.Item1 == null))
                    throw new MissingMethodException("Inventory UI hook changed.");

                LoadItemIds();
                harmony = Activator.CreateInstance(harmonyType, PatchId);
                foreach (var hook in hooks)
                {
                    MethodInfo callback = typeof(EquipmentNameColors).GetMethod(hook.Item2, BindingFlags.Static | BindingFlags.Public);
                    object postfix = Activator.CreateInstance(harmonyMethodType, callback);
                    patch.Invoke(harmony, new[] { (object)hook.Item1, null, postfix, null, null });
                }
                _installed = true;
                Log("Enabled orange inventory names for " + ItemIds.Count + " module items.");
            }
            catch (Exception ex)
            {
                _failed = true;
                if (harmony != null)
                {
                    try { harmonyType.GetMethod("UnpatchAll", new[] { typeof(string) })?.Invoke(harmony, new object[] { PatchId }); }
                    catch (Exception rollbackError) { Log("UI patch rollback: " + rollbackError); }
                }
                Log("Orange names unavailable; item data was not changed. " + ex);
            }
        }

        private static string ModuleDirectory => Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(typeof(EquipmentNameColors).Assembly.Location), "..", ".."));

        private static XmlDocument ReadXml(string path)
        {
            var document = new XmlDocument { XmlResolver = null };
            using (var reader = XmlReader.Create(path, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null }))
                document.Load(reader);
            return document;
        }

        private static void LoadItemIds()
        {
            ItemIds.Clear();
            LockIds.Clear();
            string dataDirectory = Path.Combine(ModuleDirectory, "ModuleData") + Path.DirectorySeparatorChar;
            var module = ReadXml(Path.Combine(ModuleDirectory, "SubModule.xml"));
            foreach (XmlElement entry in module.SelectNodes("/Module/Xmls/XmlNode/XmlName[@id='Items']"))
            {
                string path = Path.GetFullPath(Path.Combine(dataDirectory, entry.GetAttribute("path") + ".xml"));
                if (!path.StartsWith(dataDirectory, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Item XML outside module directory.");
                var items = ReadXml(path);
                foreach (XmlElement item in items.SelectNodes("/Items/Item | /Items/CraftedItem"))
                {
                    string id = item.GetAttribute("id");
                    // Only our namespaced definitions, never vanilla overrides or troop equipment references.
                    if (id.StartsWith("mv_", StringComparison.Ordinal))
                        ItemIds.Add(id);
                }
            }
            LockIds.UnionWith(ItemIds);
        }

        public static void AfterLockId(EquipmentElement equipmentElement, string __result)
        {
            if (equipmentElement.Item != null && ItemIds.Contains(equipmentElement.Item.StringId) && __result != null)
                LockIds.Add(__result);
        }

        public static void AfterMovieLoad(string movieName, IViewModel datasource, IGauntletMovie __result)
        {
            try
            {
                if (movieName != "Inventory" || !(datasource is SPInventoryVM inventory) || __result?.RootWidget == null)
                    return;
                var screen = __result.RootWidget.GetFirstInChildrenAndThisRecursive(w => w is InventoryScreenWidget) as InventoryScreenWidget;
                if (screen != null && inventory.ItemMenu != null)
                {
                    Menus.Remove(screen);
                    Menus.Add(screen, inventory.ItemMenu);
                }
            }
            catch (Exception ex) { ReportUiError(ex); }
        }

        public static void AfterRowUpdate(InventoryItemTupleWidget __instance)
        {
            try { SetOrange(__instance.NameTextWidget, __instance.ItemID != null && LockIds.Contains(__instance.ItemID)); }
            catch (Exception ex) { ReportUiError(ex); }
        }

        public static void AfterScreenUpdate(InventoryScreenWidget __instance)
        {
            try
            {
                Widget tooltip = __instance.InventoryTooltip;
                if (tooltip == null || tooltip.IsHidden || !Menus.TryGetValue(__instance, out var menu))
                    return;
                UpdateTitle(tooltip.FindChild("TargetItemTooltip"), menu.ItemName, TargetItem.GetValue(menu) as ItemVM);
                UpdateTitle(tooltip.FindChild("ComparedItemTooltip"), menu.ComparedItemName,
                    menu.IsComparing ? ComparedItem.GetValue(menu) as ItemVM : null);
            }
            catch (Exception ex) { ReportUiError(ex); }
        }

        private static void UpdateTitle(Widget container, string name, ItemVM item)
        {
            // Only the first title in the known tooltip container, not its colored stat comparisons.
            var title = container?.GetFirstInChildrenRecursive(w => w is TextWidget) as TextWidget;
            if (title == null || title.Text != name)
                return;
            string id = item?.ItemRosterElement.EquipmentElement.Item?.StringId;
            SetOrange(title, id != null && ItemIds.Contains(id));
        }

        private static void SetOrange(TextWidget widget, bool orange)
        {
            if (widget == null)
                return;
            if (NameBrushes.TryGetValue(widget, out var state))
            {
                if (orange && ReferenceEquals(widget.ReadOnlyBrush, state.Colored))
                    return;
                if (ReferenceEquals(widget.ReadOnlyBrush, state.Colored))
                    widget.Brush = state.Original;
                NameBrushes.Remove(widget);
            }
            if (!orange)
                return;

            // Give this label its own brush; never recolor the shared inventory font brush.
            Brush original = widget.ReadOnlyBrush;
            Brush colored = original.Clone();
            foreach (Style style in colored.Styles)
                style.FontColor = Orange;
            widget.Brush = colored;
            NameBrushes.Add(widget, new NameBrush { Original = original, Colored = widget.ReadOnlyBrush });
        }

        private static void ReportUiError(Exception ex)
        {
            if (_reportedUiError)
                return;
            _reportedUiError = true;
            Log("Skipped inventory color update: " + ex);
        }

        private static void Log(string message)
        {
            try { File.AppendAllText(Path.Combine(ModuleDirectory, "equipment_name_colors.log"), DateTime.Now.ToString("s") + " " + message + Environment.NewLine); }
            catch (Exception) { }
        }
    }
}
#endif
