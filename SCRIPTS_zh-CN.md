[English](SCRIPTS.md)

本文件介绍了随工具一同提供的内置脚本。

许多脚本仍处于实验阶段，使用时请务必小心。**请始终勤做备份。**
如遇到任何 bug 或问题，请及时反馈。

## 资源导出脚本

这些脚本用于从游戏中提取/转储资源，既可用于分析，也可用于之后重新导入。

大多数脚本会提示你选择一个文件夹/目录，文件将*直接*导出到该位置。请注意不要意外覆盖已有数据。

- `ExportAllAssembly.csx`：导出 GML 汇编代码。
- `ExportAllCode.csx`：导出反编译后的 GML 代码。
- `ExportAllEmbeddedTextures.csx`：将所有纹理页导出为 PNG 文件。
- `ExportAllFonts.csx`：导出所有字体的 PNG 图像及字形信息。
- `ExportAllMasks.csx`：将精灵的碰撞遮罩导出为黑白 PNG 图像。
- `ExportAllRoomsToPNG.csx`：将所有房间的预览图导出为 PNG 文件。
	* （注意：目前仅在 Windows 图形界面版本中支持。）
- `ExportAllShaders.csx`：导出着色器的元数据、源代码以及编译后的（二进制）代码。
- `ExportAllSounds.csx`：将声音导出为 WAV/OGG 文件。
- `ExportAllSprites.csx`：将精灵导出为 PNG 文件。可选择是否包含内边距（即精灵帧未占满其完整尺寸时的留白部分）。
- `ExportAllTextures.csx`：将精灵、字体和背景（图块集）所引用的全部纹理页项导出为 PNG 文件。
- `ExportAllTexturesGrouped.csx`：与上一个脚本相同，但会为每个资源单独创建子目录。
- `ExportAllTilesets.csx`：将图块集（或背景）导出为 PNG 文件。
- `ExportAllStrings.csx`：将所有字符串导出到一个文本文件。可选择是否包含含有换行符的字符串（这类字符串无法重新导入）。
- `ExportAllStringsJSON.csx`：将所有字符串导出到一个 JSON 文本文件。始终包含含有换行符的字符串。
- `ExportSpecificCode.csx`：根据给定的对象名、脚本名或代码条目名，导出指定的代码条目。
- `ExportSpecificSprites.csx`：根据给定的名称导出指定的精灵。可选择是否包含内边距。
- `ExportSpritesAsGIF.csx`：将精灵导出为 GIF 动图。始终保留内边距。
- `ExportTextureGroups.csx`：将精灵、字体和图块集所引用的全部纹理页项导出为 PNG 文件。按纹理组分目录存放，同时也会导出完整的纹理页。可选择是否包含内边距。

## 资源导入脚本

这些脚本用于将资源导入到游戏中，通常需要特定类型的文件/数据作为输入。

- `ApplyBasicGraphicsMod.csx`：不使用纹理打包器导入图像：只要尺寸相同，就直接在纹理表上原位替换精灵。
- `ImportAllStrings.csx`：从文本文件导入字符串，前提是这些字符串来自同一个未经修改的游戏，且其中不包含含有换行符的字符串。
- `ImportAllStringsJSON.csx`：从 JSON 文本文件导入字符串，前提是这些字符串来自同一个未经修改的游戏。
- `ImportAllTilesets.csx`：从文件夹导入图块集（或背景）图像，前提是游戏数据中已存在对应的资源。
- `ImportAssembly.csx`：导入 GML 汇编代码。
- `ImportGML.csx`：导入并编译 GML 代码。在可能的情况下会自动创建某些类型的资源。代码还可以引用同一次导入中定义的脚本/函数。
- `ImportGMS2FontData.csx`：直接从 GMS2 的 .yy 字体项目文件导入字体数据。在某些 GameMaker 版本上可能无法正常工作。
- `ImportGraphics.csx`：从文件夹中以 PNG 文件形式导入精灵或背景。精灵文件需采用 `filename_N.png` 的命名格式，其中 N 为从 0 开始的帧编号。将文件放入名为 "Backgrounds" 的文件夹中即可作为背景导入。
- `ImportGraphicsAdvanced.csx`：从文件夹中以 PNG 或 GIF 文件形式导入精灵或背景。支持多种格式（帧编号从 1 或 0 开始、GameMaker 的 `_stripN` 格式、GIF 动图、单帧精灵）。导入完成后还会弹出一个额外的对话框，用于设置精灵偏移量；对于 GMS2 游戏，还可设置动画速度。
	* （注意：目前仅在 Windows 图形界面版本中支持。）
- `ImportMasks.csx`：从黑白 PNG 文件导入精灵的碰撞遮罩。
- `ImportShaders.csx`：导入着色器数据，要求其格式与 `ExportShaders.csx` 导出的格式一致。
- `ImportSingleSound.csx`：导入单个声音资源，支持 WAV 或 OGG 格式。
- `ImportSounds.csx`：导入声音资源，支持 WAV 或 OGG 格式。
- `NewTextureRepacker.csx`：使用纹理打包器重新生成所有纹理页，可通过编辑脚本来调整相关设置。详情请阅读脚本中的注释。
	* （使用此脚本时请格外小心，数据很容易损坏或出错。）
- `ReduceEmbeddedTexturePages.csx`：与上面的纹理页重新生成脚本类似，但实现方式更为简单/陈旧。
	* （与上一个脚本一样，使用时请格外小心。）

## 示例脚本

这是一组较早的脚本，可用来参考脚本的编写方式，或者纯粹拿来娱乐。

- `DeltaHATE.csx`：在 Deltarune 的 SURVEY_PROGRAM 版本上展示资源随机打乱技术的 Mod，效果十分滑稽。类似于 Undertale 的另一个名为 "HATE" 的修改工具。
- `DeltaMILK.csx`：将 Deltarune 第一章中所有非背景精灵替换为牛奶。
- `HeCanBeEverywhere.csx`：实验性 Mod，在 Deltarune 的 SURVEY_PROGRAM 版本中加入多个 Jevil。
- `MixMod.csx`：截至本文撰写时，此脚本无法正常运行。其设计目的是利用 GameMaker 扩展在 Undertale 中播放来自 YouTube 的音乐。
- `RoomOfDetermination.csx`：在 Undertale 中展示房间编辑和代码修改功能的 Mod。
- `TheWholeWorldRevolving.csx`：使 Deltarune 的 SURVEY_PROGRAM 版本中的画面持续旋转的 Mod。

## UTDR 脚本

这些脚本主要是为 Undertale/Deltarune 开发的。

- `Debug.csx`：在所有受支持的 Undertale/Deltarune 版本中启用或禁用调试模式。
- `DeltaruneClearShaderData.csx`：清除 Deltarune 中的着色器数据，使旧版本的游戏能够在老旧硬件上运行。
- `DeltaruneReloadJSON.csx`：为 Deltarune 添加一个用于重新加载语言 JSON 文件的热键。
- `DeltaruneTTFFonts.csx`：将 Deltarune 中的所有字体标记为从外部加载。不处理日文文本。
- `DisableDogcheck.csx`：禁用 Undertale 和 Deltarune 中的 dogcheck 检查，这样你就可以从存档加载游戏中的任意房间。
- `ShowRoomName.csx`：在 Undertale 或 Deltarune 中，当调试模式启用时在屏幕上显示房间 ID 和名称。
- `TouchControlsEnabler.csx`：为 Undertale 或 Deltarune 添加移动端触屏控制。
- `UndertaleBattlegroupSelector.csx`：为 Undertale 添加一个用于选择战斗组（battlegroup）的调试热键。
- `UndertaleBetterVaporiser.csx`：用一套新的系统替换 Undertale 现有的蒸发（vapor）系统，新系统可根据对应精灵实时生成蒸发效果，并支持蒸发彩色精灵。
- `UndertaleBorderEnabler.csx`：为 Switch 版之前的 PC 版 Undertale 启用边框。
- `UndertaleBorderEnablerv1_11.csx`：为 PC 版 Undertale 的 1.11 版本（Switch）启用边框。
- `UndertaleChangeHomeBattlegroup.csx`：更改在 Undertale 中按下 Home 键时所进入的目标战斗组。
- `UndertaleDebugMsg.csx`：在 Undertale 中启用调试模式时，将 `global.msg` 的内容显示在屏幕上。
- `UndertaleDebugToggler.csx`：使 Undertale 中可以用 F1 键开关调试模式。
- `UndertaleDialogSimulator.csx`：为 Undertale 添加一个对话编辑器。
- `UndertaleFixAlphysLabCrashAndroid.csx`：修复 Undertale 在 Android 设备上于 Alphys 实验室处崩溃的问题。
- `UndertaleGoToRoom.csx`：为 Undertale 添加一个可跳转到指定房间 ID 的调试热键。
- `UndertaleRunButton.csx`：移除 Undertale 中 Backspace 热键加速功能的调试模式检查。
- `UndertaleSafeBlaster.csx`：添加一个新版本的 obj_gasterblaster，它不再专门绑定于 Sans 的 Boss 战。
- `UndertaleSimplifyBattlegroupScript.csx`：移除 Undertale 中 `scr_battlegroup` 的重复代码。
- `UndertaleSwitchAndXboxOnPC.csx`：对 Switch/Xbox 版 Undertale 进行少量修改，使其能够在 PC 上正常运行。
- `UndertaleTTFFonts.csx`：将 Undertale 中的所有字体标记为从外部加载。不处理日文文本。
- `UndertaleWithJSONs.csx`：此脚本将所有支持日文的 Undertale 版本（1.05 及以上）的文本 JSON 化。按 F11 切换语言；按 F12 可随时从 JSON 重新加载当前语言的文本。
- `UndertaleWASD.csx`：为 Undertale 添加 WASD 操作方式。

### 实用工具脚本

- `ExternalizeAllOGGs.csx`：将游戏中的所有 OGG 音效外置化为外部文件。
- `ExtractEmbeddedDataFile.csx`：从可执行文件或内存转储文件中提取数据文件。不保证能得到可用的结果。
- `FancyRoomSelect.csx`：添加一个 Mod，让在游戏中选择并传送到不同房间变得非常方便。
- `FindAndReplace.csx`：可在整个游戏的 GML 代码中执行查找和替换操作。
- `FindObjectsInRooms.csx`：在游戏的所有房间数据中查找指定对象类型的实例。
- `FindObjectsWithSprite.csx`：在游戏中查找所用精灵符合指定模式的对象类型。
- `FontEditor.csx`：通过自定义图形界面详细编辑字体数据。
	* （注意：目前仅在 Windows 图形界面版本中支持。）
- `GoToRoom_AutoLocatePersistentObj.csx`：与面向 Undertale/Deltarune 的 `GoToRoom.csx` 类似，但会尝试自动定位一个持久化（persistent）对象，因此可移植到其他游戏。效果因游戏而异。
- `MergeImages.csx`：从两个目录加载图像，并将所有对应的图像并排拼接后导出。
- `ScaleAllTextures.csx`：缩放游戏中的所有纹理页。极有可能导致显示问题。

## 技术性脚本

这些脚本技术性很强且用途小众。**使用其中任何一个之前，你必须真正清楚自己在做什么，要求比其他脚本更高。它们很可能会出问题。**

- `13_To_14.csx`、`14_To_16.csx`、`15_To_16.csx`、`15_To_17_To_16.csx`、`16_To_17.csx`、`ConvertFrom17to16.csx`、`ConvertFrom17to16_for_2.3.csx`：尝试转换游戏的字节码版本。
- `AutoBackup.csx`：带图形界面的 data.win 文件自动备份脚本，在你工作时于后台运行。可能不稳定。
- `CheckDecompiler.csx`：检查反编译器/编译器对某个游戏的准确性，并将反编译不准确的脚本输出到游戏目录中。
- `CopySound.csx`、`CopySoundInternal.csx`：分别尝试在不同游戏之间以及同一游戏内部复制声音数据。
- `CopySpriteBgFont.csx`、`CopySpriteBgFontInternal.csx`：分别尝试在不同游戏之间以及同一游戏内部复制精灵/背景/字体数据。
- `ExecutionOrder.csx`：启用后，在受支持的游戏中显示对象事件的执行顺序，以及与 `global.interact` 的交互情况。
- `ExportAllCodeSync.csx`：导出 GML 代码，但不采用并行方式。速度慢得多，但可以避免可能出现的多线程问题。
- 有两个脚本可用于修复资源被意外移离原位的数据文件。
    * `ExportAssetOrder.csx`：将游戏中所有资源的名称（目前不包括 GameMaker 2.3+ 的资源）导出到一个文本文件。
    * `ImportAssetOrder.csx`：读取一个由 `ExportAssetOrder.csx` 导出的文本文件，并据此重新整理当前数据文件中的资源顺序。
        - 如果找不到某些资源，脚本将无法完整执行。文本文件中未列出的资源会被直接移到各自列表的末尾。
- `FindNullsInOffsetMap.csx`：用于在偏移映射表（数据文件的一种内部结构）中查找空对象。
- `FindUnknownFunctions.csx`：用于找出游戏中定义不正确的函数，这类函数会导致游戏无法启动。
- `FindUnusedStrings.csx`：用于查找游戏中未使用的字符串。新版图形界面已具备同样功能，此脚本已过时。
- `GameObjectCopy.csx`、`GameObjectCopyInternal.csx`：分别尝试在不同游戏之间以及同一游戏内部复制对象数据。
- `GetAllChunkNames.csx`：返回包含当前打开的游戏数据文件中所有区块（chunk）名称列表的 C# 代码。
- `GetTempDirectory.csx`：返回当前的基础目录，用于调试本工具的 .NET 打包版本。
- `ImportGraphics_Full_Repack.csx`：与 `ImportGraphics.csx` 类似，但会导出并重新导入游戏中的所有纹理。可能极其缓慢，并占用大量内存/存储空间。
- `LintAllScripts.csx`：调试工具，用于确保各脚本能够正常编译。
- `MatchRoomOrderToInternalOrder.csx`：使房间资源顺序与实际房间顺序一致。
- `Profiler.csx`：基于执行顺序脚本改造而成的性能分析器和堆栈跟踪器，用于定位卡死位置。会执行大量磁盘操作。
- `RealignRoomInternalOrder.csx`：使房间顺序与房间资源顺序一致。图形界面已具备同样功能，此脚本已过时。
- `RemoveUnusedSounds.csx`：尝试移除游戏中未使用的声音。
- `RestoreMissingCodeLocals.csx`：尝试为缺少局部变量信息（code locals）的代码条目恢复这些信息，仅适用于特定的 GameMaker 版本。
- `TestExportAllCode.csx`：记录反编译全部代码时工具崩溃的情况。
- `ToggleAlignment.csx`：切换游戏数据文件中 TPAG 区块的 4 字节对齐。
- `VariableFixer.csx`：尝试修复数据文件中的变量配置，适用于特定的 GameMaker 版本。
