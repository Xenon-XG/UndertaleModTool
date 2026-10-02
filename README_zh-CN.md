# UndertaleModTool 简体中文版

[English README](README.md)

这是 [UndertaleModTool](https://github.com/UnderminersTeam/UndertaleModTool) 的简体中文汉化版本，汉化内容包括：

* 图形界面（GUI）的全部文本：主窗口、菜单、资源树、所有资源编辑器（精灵、房间、代码、字体、声音、游戏对象、粒子系统等）、设置窗口、代码搜索、查找引用、项目管理等；
* 命令行版（CLI）的帮助信息与提示；
* 更新器（Updater）的提示信息。

汉化版基于原项目 **v0.9.2.0** 的源代码，功能与原版完全一致，仅翻译了用户可见的文本。内置脚本（`Scripts/` 目录下的 `.csx` 文件）为了与原项目及社区脚本保持兼容，保留英文原文。

> *\* （看到这样一个专业的工具，这么多出色贡献者的心血结晶……）*
>
> *\* （你充满了决心。）*

UndertaleModTool 是目前最完整的 Undertale（以及其他 GameMaker 游戏！）修改、反编译与解包工具。

# 快速开始

## Windows

1. 前往本仓库的 [Releases](https://github.com/Xenon-XG/UndertaleModTool/releases) 页面下载最新的汉化版压缩包。
2. 普通用户请下载图形界面版（例如 `UndertaleModTool_v0.9.2.0-zh-CN_GUI-Windows-x64-Normal.zip`）；熟悉命令行的用户可以下载 CLI 版本。
3. 将压缩包**解压到一个文件夹**（不要在压缩包内直接运行！）。
4. 运行 `UndertaleModTool.exe` 启动工具。
5. 通过「文件 → 打开」打开游戏的数据文件（如 `data.win`、`game.ios`、`game.unx` 等）。

## macOS / Linux

目前没有在 macOS 或 Linux 上运行图形界面版的官方方法，可以考虑以下方案：

* 使用命令行版（CLI）。它没有图形界面，但适合自动化与快速处理。
* 使用 [Wine](https://winehq.org) 运行 Windows 版。

# 版本说明

* `GUI`（默认）：带有完整图形界面，便于查看与修改数据文件。
* `CLI`：仅提供命令行界面，适合自动化与批处理，但使用门槛更高。
* `SingleFile`（单文件）：所有依赖都打包进一个可执行文件中，目录更干净，但在个别情况下可能出现稳定性问题。
* `Normal`（默认）：依赖文件不打包进主程序，而是放在主程序旁边。如果不介意在大约 300 个 DLL 中寻找主程序，或者单文件版在你的电脑上有问题，请选择此版本。

# 主要功能

* 可以读取最新版本的 Undertale、Deltarune 以及绝大多数 GameMaker 游戏数据文件中的每一个字节，并从解析出的数据重新生成逐字节一致的文件。
* 正确处理文件中的所有指针，因此增删内容、改变长度、移动位置等操作都不会破坏文件格式。
* 编辑器几乎可以修改每一个数值，包括未知字段。
* 内置简易的房间/关卡编辑器。
* 支持编辑 GML 虚拟机代码：可以使用内置的 GML 编译器或 GML 汇编向游戏中加入任意自定义代码（不支持 YYC）。
* 高级 GML 反编译器与编译器，支持大量 GameMaker 版本以及绝大多数重要的 GML 特性。
* 支持运行脚本来自动修改数据文件（或者完成其他“邪恶”的任务），可用于模组分发。
* 所有核心功能都抽取为独立类库，可供外部工具使用。
* 可以生成供 GM:S 调试器使用的 `.yydebug` 文件，从而实时修改变量。
* 可选的 GameMaker 相关数据文件关联，首次启动时可选择启用，也可以在可执行文件旁放置 `dna.txt` 文件来禁用。

# 内置脚本

UndertaleModTool 自带了一批 C# 脚本来扩展功能，详细说明请参阅 [SCRIPTS.md](SCRIPTS.md)。

# 编译方法

自行编译需要安装 `.NET 10 SDK` 或更高版本。

使用 Git 克隆时请务必递归克隆子模块（例如使用 `--recurse-submodules`），因为 `Underanalyzer` 依赖是以子模块的形式引入的。

```bash
git clone --recurse-submodules https://github.com/Xenon-XG/UndertaleModTool.git
cd UndertaleModTool
dotnet publish UndertaleModTool -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false --output out
```

在 Linux/macOS 上编译 Windows 图形界面版时需要额外加上 `-p:EnableWindowsTargeting=true`。

# 协议与致谢

本项目遵循 [GPLv3](LICENSE.txt) 协议开源。感谢 Underminers 团队以及所有为原项目做出贡献的开发者。汉化版与 Underminers 团队无关，使用汉化版时遇到的问题请在本仓库的 [Issues](https://github.com/Xenon-XG/UndertaleModTool/issues) 中反馈。
