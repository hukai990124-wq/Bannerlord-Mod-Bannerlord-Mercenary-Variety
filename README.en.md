# Mercenary Variety

[简体中文](README.md) | [English](README.en.md) | [日本語](README.ja.md)

Mod version v1.0.0. Target game version Bannerlord v1.4.8. This is a **source and data repository**: it contains no meshes, textures, or build caches. See "Scope of this repository" below.

## What this mod does

Beyond the vanilla mercenary system, this mod adds several armed companies with their own backgrounds, strongholds, dialogue, and quest lines. They are not troop entries in a shop list — they are people with names, histories, and terms of their own.

## What's inside

### Hodophylakes (Road Wardens)

An imperial road patrol led by Theodora. Her father Nikandros was killed in an ambush while escorting refugees; Theodora took up his banner and refused to sell the company's spears to the wars of princes, holding to his oath that the roads belong first to those who must travel them.

Four core members, each with distinct expertise and dialogue: Theodora (commander), Leontios (veteran sergeant), Irene (tracker), and Niketas (crossbow captain). Guild menus offer patrol, escort, and clearing contracts; accumulated progress unlocks further work.

### Old Vaegir Guards

The personal guard of Emperor Arenicos. After the emperor's unexpected death the guard was blamed for failing him, dismissed from the army, and left without a banner, drifting between Sturgia and the Northern Empire. Captain Vasevolod holds the scattered veterans together, determined to keep the company intact until they find a cause worth swearing to again.

Includes supply, governor, service-record recovery, and sea-raider quest lines, plus a shelter menu where they can be hired.

### Sea Raider Guild

A raiding company offering guild menus and siege gameplay independent of lord politics.

### Western Mercenary Outpost

A mercenary stronghold on the frontier with its own guild progress and outpost menu, functioning as a long-term employer side path.

### The Trials of Altair

An assassin contact line found in taverns. Three trials, each with its own scene and checks:

- **Initiation trial**: infiltration and disguise; a dedicated detection model changes how guards react to you
- **Martial trial**: live combat
- **Wisdom trial**: dialogue and deduction

### Equipment name coloring

Colors item names by quality and type so long inventory lists can be read at a glance. Toggleable in game.

## Installation

Place the entire `MercenaryVariety` folder into the game's `Modules` directory.

Load order:

```
Bannerlord.Harmony (optional)
Native
SandBoxCore
Sandbox
CustomBattle
StoryMode
NavalDLC (optional)
MercenaryVariety
```

`NavalDLC` is an optional dependency; sea raider content is only complete with it enabled.

## Building

Open the following in Visual Studio 2022 or MSBuild:

```
Source/MercenaryVariety/MercenaryVariety.csproj
```

The target is `Win64_Shipping_Client`. Before building, point the game references in `MercenaryVariety.csproj` at your local Bannerlord installation. Build output goes to `bin/Win64_Shipping_Client/`.

## Scope of this repository

**This repository contains no meshes, textures, `AssetPackages`, or `RuntimeDataCache`.** Those assets are large and unrelated to the code; obtain them from the official release package.

For that reason the XML in this repository is a **variant that depends on no custom meshes**:

- The bundled Roman-themed weapons and armor — along with their crafting pieces, templates, and weapon descriptions — have been removed, and their definition files and load declarations in `SubModule.xml` deleted.
- Heroes and NPCs that previously wore those custom items now use vanilla equipment.
- The related quest rewards now grant vanilla items; the quest flow itself is unchanged.

Cloning this repository is enough to build and launch the mod without empty hands, invisible equipment, or load errors caused by missing meshes.

## License

Released under the **MIT License**; see [LICENSE](LICENSE). You may use, modify, and redistribute it freely, including commercially, as long as the copyright notice is retained.

This mod depends on Mount & Blade II: Bannerlord and its official modules. The game and its official assets are not part of this repository and remain under their own terms.
