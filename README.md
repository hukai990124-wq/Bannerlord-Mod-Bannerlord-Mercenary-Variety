# Mercenary Variety — 佣兵多样性

《骑马与砍杀 II：霸主》雇佣兵与地方武装扩展模组。

点击语言标题展开或收起。 / Click a language to expand or collapse. / 言語名をクリックして開閉してください。

---

<details>
<summary><strong>简体中文</strong></summary>

模组版本 v1.0.0。目标游戏版本 Bannerlord v1.4.8。本仓库为**源码与数据仓库**，不含模型、贴图与编译缓存，详见下文「本仓库的范围」。

## 这个模组做什么

在原版的雇佣兵体系之外，加入数支有独立背景、据点、对话与任务线的武装团体。它们不是商店里的兵种列表，而是有名字、有历史、会向你提出条件的人。

## 主要内容

### 道路守卫（Hodophylakes）

由 Theodora 率领的帝国公路巡逻队。她的父亲 Nikandros 在护送难民时遇伏身亡，Theodora 接过旗帜，拒绝把队伍的长矛卖给诸侯的战争，坚持「道路首先属于必须走它的人」。

四名核心成员各有专长与对话：Theodora（指挥官）、Leontios（老兵军士）、Irene（追踪者）、Niketas（弩手队长）。通过公会菜单接取巡逻、护送与清剿任务，累积进度后解锁新的委托。

### 老维吉亚卫队（Old Vaegir Guards）

先皇 Arenicos 的私人卫队。皇帝意外驾崩后，卫队被追究护驾不力、逐出军队，失去旗帜，在斯特吉亚与北帝国之间流浪。队长 Vasevolod 把散落的旧部重新聚拢，只为在找到值得他们再次宣誓的事业前，让这支队伍不散。

包含补给、总督、寻回服役记录、清剿海掠者等任务线，以及可雇佣他们的避难所菜单。

### 海掠者公会

以劫掠为业的海上武装，提供独立于领主政治的公会菜单与攻城玩法。

### 西部佣兵前哨

边境上的佣兵据点，带自己的公会进度与前哨菜单，可作为长期雇主的支线。

### 阿尔泰尔的试炼

酒馆里的刺客引路人线。三道试炼各有独立场景与判定：

- **入会试炼**：潜入与身份伪装，配套的伪装识别模型会改变卫兵对你的反应
- **武艺试炼**：实战对抗
- **智慧试炼**：对话与推理

### 装备名称着色

按品质与类型给装备名上色，便于在长列表中快速辨认。可在游戏内开关。

## 安装

将整个 `MercenaryVariety` 文件夹放入游戏的 `Modules` 目录。

启用顺序：

```
Bannerlord.Harmony（可选）
Native
SandBoxCore
Sandbox
CustomBattle
StoryMode
NavalDLC（可选）
MercenaryVariety
```

`NavalDLC` 为可选依赖，启用后海掠者相关内容才完整。

## 构建

用 Visual Studio 2022 或 MSBuild 打开：

```
Source/MercenaryVariety/MercenaryVariety.csproj
```

输出目标为 `Win64_Shipping_Client`。编译前请确认 `MercenaryVariety.csproj` 中的游戏引用路径指向你本机的 Bannerlord 安装位置。编译产物放入 `bin/Win64_Shipping_Client/`。

## 本仓库的范围

**本仓库不含模型、贴图、`AssetPackages`、`RuntimeDataCache` 等美术与缓存资源。** 这些内容体积庞大且与代码无关，请从正式发布包获取。

正因如此，仓库中的 XML 已被调整为**不依赖任何自定义模型**的版本：

- 自带的罗马主题武器与护甲（含配套锻造部件、模板与武器描述）已全部移除，其定义文件与 `SubModule.xml` 中的加载声明一并删除。
- 原本装备这些自定义物品的英雄与 NPC，已改用原版装备。
- 相关任务奖励也改为发放原版物品，任务流程本身不受影响。

直接克隆本仓库即可编译并进入游戏，不会因缺失模型而出现空手、隐形装备或加载报错。

## 许可

以 **MIT 协议**发布，全文见 [LICENSE](LICENSE)。可自由使用、修改、再分发，包括商业用途，只需保留版权声明。

本模组依赖《骑马与砍杀 II：霸主》及其官方模块，游戏本体与官方资源不属于本仓库，遵循其各自的授权条款。

</details>

---

<details>
<summary><strong>English</strong></summary>

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

</details>

---

<details>
<summary><strong>日本語</strong></summary>

MOD バージョン v1.0.0。対象ゲームバージョンは Bannerlord v1.4.8 です。本リポジトリは**ソースとデータのリポジトリ**であり、モデル・テクスチャ・ビルドキャッシュを含みません。詳細は後述の「本リポジトリの範囲」を参照してください。

## この MOD について

バニラの傭兵システムに加えて、独自の背景・拠点・会話・クエストを持つ複数の武装集団を追加します。商店の兵種リストではなく、名前と歴史を持ち、こちらに条件を提示してくる人々です。

## 主な内容

### ホドフィラケス（道路守備隊）

テオドラが率いる帝国の街道パトロール隊。父ニカンドロスは難民護衛中に待ち伏せで命を落とし、テオドラは旗を継ぎました。諸侯の戦争に槍を売ることを拒み、「道はまず、そこを進まねばならぬ者のもの」という父の誓いを守っています。

4 名の主要メンバーがそれぞれ専門と会話を持ちます：テオドラ（指揮官）、レオンティオス（古参軍曹）、イレーネ（追跡者）、ニケタス（弩兵隊長）。ギルドメニューから巡回・護衛・掃討の依頼を受けられ、進行度に応じて新たな依頼が解放されます。

### 旧ヴァギル近衛隊

皇帝アレニコスの私的な近衛隊。皇帝の急逝後、護衛の失敗を問われて軍を追われ、旗を失い、ストゥルギアと北帝国の間を流浪します。隊長ヴァセヴォロドは散った旧部下を集め、再び誓うに値する大義を見つけるまで隊を保とうとしています。

補給・総督・軍務記録の奪還・海賊掃討などのクエスト線に加え、彼らを雇用できる避難所メニューがあります。

### 海賊ギルド

略奪を生業とする海上の武装集団。領主政治から独立したギルドメニューと攻城の遊びを提供します。

### 西部傭兵前哨

辺境の傭兵拠点。独自のギルド進行度と前哨メニューを持ち、長期的な雇い主としてのサブパスになります。

### アルタイルの試練

酒場に現れる暗殺者の導き手。三つの試練にはそれぞれ専用のシーンと判定があります。

- **入会の試練**：潜入と身分の偽装。専用の識別モデルが衛兵の反応を変えます
- **武芸の試練**：実戦
- **知恵の試練**：会話と推理

### 装備名の着色

品質と種類に応じて装備名を着色し、長い所持品リストでも一目で判別できるようにします。ゲーム内で切り替え可能です。

## インストール

`MercenaryVariety` フォルダーごとゲームの `Modules` ディレクトリに配置してください。

ロード順：

```
Bannerlord.Harmony（任意）
Native
SandBoxCore
Sandbox
CustomBattle
StoryMode
NavalDLC（任意）
MercenaryVariety
```

`NavalDLC` は任意の依存先です。有効にすると海賊関連の内容が完全になります。

## ビルド

以下を Visual Studio 2022 または MSBuild で開いてください。

```
Source/MercenaryVariety/MercenaryVariety.csproj
```

出力先は `Win64_Shipping_Client` です。ビルド前に `MercenaryVariety.csproj` 内のゲーム参照パスを、お使いの Bannerlord のインストール先に合わせてください。ビルド成果物は `bin/Win64_Shipping_Client/` に出力されます。

## 本リポジトリの範囲

**本リポジトリにはモデル、テクスチャ、`AssetPackages`、`RuntimeDataCache` を含みません。** これらは容量が大きくコードとは無関係なため、正式な配布パッケージから取得してください。

そのため、リポジトリ内の XML は**独自モデルに依存しない版**に調整されています。

- 同梱されていたローマ風の武器・防具（対応する鍛冶パーツ、テンプレート、武器説明を含む）はすべて削除し、定義ファイルと `SubModule.xml` の読み込み宣言も削除しました。
- これらを装備していた英雄と NPC は、バニラの装備に変更しました。
- 関連するクエスト報酬もバニラのアイテムを渡すよう変更しました。クエストの流れ自体は変わりません。

本リポジトリをクローンするだけでビルドと起動が可能で、モデル欠落による手ぶら・透明装備・読み込みエラーは発生しません。

## ライセンス

**MIT ライセンス**で公開されています。全文は [LICENSE](LICENSE) を参照してください。著作権表示を保持する限り、商用を含めて自由に使用・改変・再配布できます。

本 MOD は Mount & Blade II: Bannerlord およびその公式モジュールに依存します。ゲーム本体と公式アセットは本リポジトリには含まれず、それぞれの条件に従います。

</details>
