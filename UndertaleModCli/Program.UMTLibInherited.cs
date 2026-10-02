using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Underanalyzer.Decompiler;
using UndertaleModLib;
using UndertaleModLib.Compiler;
using UndertaleModLib.Decompiler;
using UndertaleModLib.Models;
using UndertaleModLib.Project;
using UndertaleModLib.Scripting;

namespace UndertaleModCli;

// Everything that gets inherited (methods, attributes) from IScriptInterface gets put here
// in order to have the inherited stuff separated from normal stuff.
public partial class Program : IScriptInterface
{
    #region Inherited UMTLib Properties

    /// <inheritdoc/>
    public UndertaleData Data { get; set; }

    /// <inheritdoc/>
    public ProjectContext Project { get; set; }

    /// <inheritdoc/>
    public string FilePath { get; set; }

    /// <inheritdoc/>
    public string ScriptPath { get; set; }

    /// <inheritdoc/>
    public object Highlighted { get; set; }

    /// <inheritdoc/>
    public object Selected { get; set; }

    /// <inheritdoc/>
    public bool CanSave { get; set; }

    /// <inheritdoc/>
    public bool ScriptExecutionSuccess { get; set; }

    /// <inheritdoc/>
    public string ScriptErrorMessage { get; set; }

    /// <inheritdoc/>
    public string ExePath { get; set; }

    /// <inheritdoc/>
    public string ScriptErrorType { get; set; }

    /// <inheritdoc/>
    public bool IsAppClosed { get; set; }

    /// <inheritdoc/>
    public Action<Action> MainThreadAction => static (f) => f();

    #endregion

    #region Inherited UMTLib Methods

    /// <inheritdoc/>
    public void EnsureDataLoaded()
    {
        if (Data is null)
            throw new ScriptException("当前未加载任何数据文件！");
    }

    /// <inheritdoc/>
    public bool MakeNewDataFile()
    {
        Data = UndertaleData.CreateNew();
        Console.WriteLine("已创建新文件。");
        return true;
    }

    /// <inheritdoc/>
    public void ScriptMessage(string message)
    {
        Console.WriteLine(message);
        if (IsInteractive) Pause();
    }

    /// <inheritdoc/>
    public void ScriptWarning(string message)
    {
        Console.WriteLine($"警告：{message}");
        if (IsInteractive) Pause();
    }

    /// <inheritdoc/>
    public void SetUMTConsoleText(string message)
    {
        // Since the UMTConsole text messages are literally just messages that are shown,
        // I'm giving them the same behaviour as the normal ScriptMessage
        ScriptMessage(message);
    }

    /// <inheritdoc/>
    public bool ScriptQuestion(string message)
    {
        while (true)
        {
            Console.Write($"{message} (Y/N)：");
            string input = ReadLineWithFallback()?.Trim().ToLower();

            if (string.IsNullOrEmpty(input))
                Console.WriteLine("请输入 Y 或 N。");

            if (input == "y" || input == "yes")
                return true;

            if (input == "n" || input == "no")
                return false;

            Console.WriteLine("输入无效。请输入 Y 或 N。");
        }
    }

    /// <inheritdoc/>
    public void ScriptError(string error, string title = "错误", bool setConsoleText = true)
    {
        // No need to care about setConsoleText if we're in CLI.

        Console.Error.WriteLine("--------------------------------------------------");
        Console.Error.WriteLine("----------------------错误！----------------------");
        Console.Error.WriteLine("--------------------------------------------------");
        Console.Error.WriteLine(title);
        Console.Error.WriteLine("--------------------------------------------------");
        Console.Error.WriteLine(error);
        Console.Error.WriteLine("--------------------------------------------------");
        Console.Error.WriteLine("----------------------错误！----------------------");
        Console.Error.WriteLine("--------------------------------------------------");
        if (IsInteractive) { Pause(); }
    }

    /// <inheritdoc/>
    public void SimpleTextOutput(string title, string label, string defaultText, bool allowMultiline)
    {
        // In order to be similar to GUI output, we strip everything past a newline in "defaultValue" should multiline be disabled
        if (!allowMultiline)
            defaultText = defaultText.Remove(defaultText.IndexOf('\n'));

        Console.WriteLine("-----------------------输出-----------------------");
        Console.WriteLine(title);
        Console.WriteLine(label);
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine(defaultText);
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("-----------------------输出-----------------------");
        Console.WriteLine("--------------------------------------------------");

        if (IsInteractive) Pause();
    }

    /// <inheritdoc/>
    public void ScriptOpenURL(string url)
    {
        Process p;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //TODO: why useShellExecute on Windows, but not on the other OS?
            p = Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); // Works ok on windows
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            p = Process.Start("xdg-open", url);  // Works ok on linux, should work on FreeBSD as it's very similar.
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            p = Process.Start("open", url); // Not tested
        }
        else
        {
            throw new InvalidOperationException("无法在此操作系统上打开浏览器：" + RuntimeInformation.OSDescription);
        }

        p?.Dispose();
    }

    /// <inheritdoc/>
    public bool RunUMTScript(string path)
    {
        try
        {
            RunCSharpFile(path);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public bool AreFilesIdentical(string file1, string file2)
    {
        using FileStream fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read, FileShare.Read);
        using FileStream fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read, FileShare.Read);

        if (fs1.Length != fs2.Length) return false; // different size, files can't be the same

        while (true)
        {
            int b1 = fs1.ReadByte();
            int b2 = fs2.ReadByte();
            if (b1 != b2) return false; // different contents, files are not the same
            if (b1 == -1) break; // here both bytes are the same. Thus we only need to check if one is at end-of-file.
        }

        // identical
        return true;
    }

    /// <inheritdoc/>
    public void SetFinishedMessage(bool isFinishedMessageEnabled)
    {
        FinishedMessageEnabled = isFinishedMessageEnabled;
    }

    /// <inheritdoc/>
    public void UpdateProgressBar(string message, string status, double currentValue, double maxValue)
    {
        string evaluatedMessage = !String.IsNullOrEmpty(message) ? $"{message}|" : "";
        Console.WriteLine($"[{evaluatedMessage}{status}] {currentValue}/{maxValue}");
    }

    /// <inheritdoc/>
    public void SetProgressBar(string message, string status, double currentValue, double maxValue)
    {
        savedMsg = message;
        savedStatus = status;
        savedValue = currentValue;
        savedValueMax = maxValue;

        UpdateProgressBar(message, status, currentValue, maxValue);
    }

    /// <inheritdoc/>
    public void UpdateProgressValue(double currentValue)
    {
        UpdateProgressBar(savedMsg, savedStatus, currentValue, savedValueMax);

        savedValue = currentValue;
    }

    /// <inheritdoc/>
    public void UpdateProgressStatus(string status)
    {
        UpdateProgressBar(savedMsg, status, savedValue, savedValueMax);

        savedStatus = status;
    }

    /// <inheritdoc/>
    public void AddProgress(int amount)
    {
        progressValue += amount;
    }

    /// <inheritdoc/>
    public void IncrementProgress()
    {
        progressValue++;
    }

    /// <inheritdoc/>
    public void AddProgressParallel(int amount) //P - Parallel (multi-threaded)
    {
        Interlocked.Add(ref progressValue, amount); //thread-safe add operation (not the same as "lock ()")
    }

    /// <inheritdoc/>
    public void IncrementProgressParallel()
    {
        Interlocked.Increment(ref progressValue); //thread-safe increment
    }

    /// <inheritdoc/>
    public int GetProgress()
    {
        return progressValue;
    }

    /// <inheritdoc/>
    public void SetProgress(int value)
    {
        progressValue = value;
    }


    #region Empty Inherited Methods

    /// <inheritdoc/>
    public void InitializeScriptDialog()
    {
        // CLI has no dialogs to initialize
    }

    /// <inheritdoc/>
    public void SetProgressBar()
    {
        //no progress bar that can be setup to show
    }

    /// <inheritdoc/>
    public void HideProgressBar()
    {
        // nothing to hide..
    }

    /// <inheritdoc/>
    public void EnableUI()
    {
        // nothing to enable...
    }

    #endregion

    /// <inheritdoc/>
    public void StartProgressBarUpdater()
    {
        if (cTokenSource is not null)
            Console.WriteLine("警告 - 后台已有另一个进度更新任务正在运行（可能已挂起）。");

        cTokenSource = new CancellationTokenSource();
        cToken = cTokenSource.Token;

        updater = Task.Run(ProgressUpdater);
    }

    /// <inheritdoc/>
    public async Task StopProgressBarUpdater() // "async" because "Wait()" blocks UI thread
    {
        if (cTokenSource is null) return;


        cTokenSource.Cancel();

        if (await Task.Run(() => !updater.Wait(2000))) // if ProgressUpdater isn't responding
            Console.WriteLine("错误 - 停止进度更新任务失败。");
        else
        {
            cTokenSource.Dispose();
            cTokenSource = null;
        }

        updater.Dispose();
    }

    /// <inheritdoc/>
    public void ChangeSelection(object newSelection, bool inNewTab = false)
    {
        //this does *not* make sense, as CLI does not have any selections
        //however, since Selection is a public object, it could potentially be used by scripts
        Selected = newSelection;
    }

    /// <summary>
    /// Prompts the user for a path with validation.
    /// </summary>
    /// <param name="promptMessage">Message to display when asking for input.</param>
    /// <param name="whitespaceErrorMessage">Error message when a path is only whitespace.</param>
    /// <param name="validate">Validation function that returns an error message, or null if valid.</param>
    /// <returns>The validated path, or null if an empty non-whitespace path is supplied or the console is unavailable.</returns>
    private static string PromptPath(string promptMessage, string whitespaceErrorMessage, Func<string, string> validate)
    {
        string path;
        string validationError;
        do
        {
            Console.WriteLine(promptMessage);
            Console.Write("路径：");

            string input = ReadLineWithFallback();
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            path = RemoveQuotes(input);

            if (string.IsNullOrWhiteSpace(path))
            {
                Console.WriteLine(whitespaceErrorMessage);
                validationError = string.Empty;
                continue;
            }

            validationError = validate(path);

            if (validationError is not null)
            {
                Console.WriteLine(validationError);
            }
        }
        while (validationError is not null);

        return path;
    }

    /// <inheritdoc/>
    public string PromptChooseDirectory()
    {
        return PromptPath(
            "请输入（或拖放）一个有效目录的路径：",
            "错误：路径不能为空白。请输入有效的目录路径。",
            path =>
            {
                try
                {
                    DirectoryInfo directoryInfo = new(path);
                    return directoryInfo.Exists ? null : $"错误：目录 '{path}' 不存在。请重试。";
                }
                catch (Exception ex) when (ex is ArgumentException or PathTooLongException or System.Security.SecurityException)
                {
                    return $"错误：路径 '{path}' 无效。{ex.Message}";
                }
            });
    }

    /// <inheritdoc/>
    public string PromptLoadFile(string defaultExt, string filter)
    {
        return PromptPath(
            "请输入（或拖放）一个有效文件的路径：",
            "错误：路径不能为空白。请输入有效的文件路径。",
            path =>
            {
                try
                {
                    FileInfo fileInfo = new(path);
                    return fileInfo.Exists ? null : $"错误：文件 '{path}' 不存在。请重试。";
                }
                catch (Exception ex) when (ex is ArgumentException or PathTooLongException or System.Security.SecurityException or NotSupportedException)
                {
                    return $"错误：路径 '{path}' 无效。{ex.Message}";
                }
            });
    }

    /// <inheritdoc/>
    public string PromptSaveFile(string defaultExt, string filter)
    {
        return PromptPath(
            "请输入（或拖放）用于保存文件的路径：",
            "错误：路径不能为空白。请输入有效的文件路径。",
            path =>
            {
                try
                {
                    string fullPath = Path.GetFullPath(path);
                    return Directory.Exists(fullPath) ? "错误：该路径已存在一个目录。" : null;
                }
                catch (Exception ex) when (ex is ArgumentException or PathTooLongException or System.Security.SecurityException or NotSupportedException)
                {
                    return $"错误：路径 '{path}' 无效。{ex.Message}";
                }
            });
    }

    /// <inheritdoc/>
    public string GetDecompiledText(string codeName, GlobalDecompileContext context = null, IDecompileSettings settings = null)
    {
        return GetDecompiledText(Data.Code.ByName(codeName), context, settings);
    }

    /// <inheritdoc/>
    public string GetDecompiledText(UndertaleCode code, GlobalDecompileContext context = null, IDecompileSettings settings = null)
    {
        if (code.ParentEntry is not null)
            return $"// This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", decompile that instead.";

        GlobalDecompileContext decompileContext = context is null ? new(Data) : context;
        try
        {
            return code != null
                ? new DecompileContext(decompileContext, code, settings ?? Data.ToolInfo.DecompilerSettings).DecompileToString()
                : "";
        }
        catch (Exception e)
        {
            return "/*\nDECOMPILER FAILED!\n\n" + e + "\n*/";
        }
    }

    /// <inheritdoc/>
    public string GetDisassemblyText(string codeName)
    {
        return GetDisassemblyText(Data.Code.ByName(codeName));
    }

    /// <inheritdoc/>
    public string GetDisassemblyText(UndertaleCode code)
    {
        if (code.ParentEntry is not null)
            return $"; This code entry is a reference to an anonymous function within \"{code.ParentEntry.Name.Content}\", disassemble that instead.";

        try
        {
            return code != null ? code.Disassemble(Data.Variables, Data.CodeLocals?.For(code), Data.CodeLocals is null) : "";
        }
        catch (Exception e)
        {
            return "/*\nDISASSEMBLY FAILED!\n\n" + e + "\n*/"; // Please don't
        }
    }

    /// <inheritdoc/>
    public string ScriptInputDialog(string titleText, string labelText, string defaultInputBoxText, string cancelButtonText, string submitButtonText, bool isMultiline, bool preventClose)
    {
        // I'll ignore the cancelButtonText and submitButtonText as they don't have much use.
        return SimpleTextInput(titleText, labelText, defaultInputBoxText, isMultiline, preventClose);
    }

    /// <inheritdoc/>
    public string SimpleTextInput(string title, string label, string defaultValue, bool allowMultiline, bool showDialog = true)
    {
        // default value gets ignored, as it doesn't really have a use in CLI.

        string result = "";

        Console.WriteLine("-----------------------输入-----------------------");
        Console.WriteLine(title);
        Console.WriteLine(label + (allowMultiline ? "（多行输入，按 SHIFT+ENTER 插入换行）" : ""));
        Console.WriteLine("--------------------------------------------------");

        if (!allowMultiline)
        {
            result = Console.ReadLine();
        }
        else
        {
            bool isEnterWithoutShiftPressed = false;
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey();
                //result += keyInfo.KeyChar;

                // If Enter is pressed without shift
                if (((keyInfo.Modifiers & ConsoleModifiers.Shift) == 0) && (keyInfo.Key == ConsoleKey.Enter))
                    isEnterWithoutShiftPressed = true;

                else
                {
                    // If we have Enter + any other modifier pressed, append newline. Otherwise, just the content.
                    if (keyInfo.Key == ConsoleKey.Enter)
                    {
                        result += "\n";
                        Console.WriteLine();
                    }
                    // If backspace, display new empty char and move one back
                    // TODO: There's some weird bug with ctrl+backspace, i'll ignore it for now.
                    // Also make some of the multiline-backspace better.
                    else if ((keyInfo.Key == ConsoleKey.Backspace) && (result.Length > 0))
                    {
                        Console.Write(' ');
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        result = result.Remove(result.Length - 1);
                    }
                    else
                        result += keyInfo.KeyChar;
                }

            } while (!isEnterWithoutShiftPressed);
        }

        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("-----------------------输入-----------------------");
        Console.WriteLine("--------------------------------------------------");

        return result;
    }

    /// <inheritdoc/>
    public async Task ClickableSearchOutput(string title, string query, int resultsCount, IOrderedEnumerable<KeyValuePair<string, List<(int lineNum, string codeLine)>>> resultsDict, bool editorDecompile, IOrderedEnumerable<string> failedList = null)
    {
        await ClickableSearchOutput(title, query, resultsCount, resultsDict.ToDictionary(pair => pair.Key, pair => pair.Value), editorDecompile, failedList);
    }

    /// <inheritdoc/>
    public async Task ClickableSearchOutput(string title, string query, int resultsCount, IDictionary<string, List<(int lineNum, string codeLine)>> resultsDict, bool editorDecompile, IEnumerable<string> failedList = null)
    {
        await Task.Delay(1); //dummy await

        // If we have failed entries...
        if (failedList is not null)
        {
            // Convert list to array first
            string[] failedArray = failedList.ToArray();

            // ...Print them all out
            Console.ForegroundColor = ConsoleColor.Red;
            if (failedArray.Length == 1)
                Console.Error.WriteLine("有 1 个代码条目在搜索时出错：");
            else
                Console.Error.WriteLine($"有 {failedArray.Length} 个代码条目在搜索时出错：");

            foreach (string failedEntry in failedArray)
                Console.Error.WriteLine(failedEntry);

            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine($"在 {resultsDict.Count} 个代码条目中找到 \"{query}\" 的 {resultsCount} 条结果。");
        Console.WriteLine();

        // Print in a pattern of:
        // Results in code_file
        // Line 3: line of code
        // Line 6: line of code
        //
        // Results in code_file_1
        // etc.
        foreach (var dictEntry in resultsDict)
        {
            Console.WriteLine($"{dictEntry.Key} 中的结果：");
            foreach (var resultEntry in dictEntry.Value)
                Console.WriteLine($"第 {resultEntry.lineNum} 行：{resultEntry.codeLine}");

            Console.WriteLine();
        }

        if (IsInteractive) Pause();
    }

    /// <inheritdoc/>
    public bool LintUMTScript(string path)
    {
        // By Grossley
        if (!File.Exists(path))
        {
            ScriptError(path + " 不存在！");
            return false;
        }
        try
        {
            CancellationTokenSource source = new CancellationTokenSource(100);
            CancellationToken token = source.Token;
            CSharpScript.EvaluateAsync(File.ReadAllText(path, Encoding.UTF8), CliScriptOptions.WithFilePath(path).WithFileEncoding(Encoding.UTF8), this, typeof(IScriptInterface), token);
        }
        catch (CompilationErrorException exc)
        {
            ScriptError(exc.Message, "脚本编译错误");
            ScriptExecutionSuccess = false;
            ScriptErrorMessage = exc.Message;
            ScriptErrorType = "CompilationErrorException";
            return false;
        }
        catch (Exception)
        {
            // Using the 100 MS timer it can time out before successfully running, compilation errors are fast enough to get through.
            ScriptExecutionSuccess = true;
            ScriptErrorMessage = "";
            ScriptErrorType = "";
            return true;
        }
        return true;
    }

    public void ReassignGUIDs(string guid, uint objectIndex)
    {
        int eventIdx = Convert.ToInt32(EventType.Collision);
        for (int i = 0; i < Data.GameObjects.Count; i++)
        {
            UndertaleGameObject obj = Data.GameObjects[i];
            try
            {
                foreach (UndertaleGameObject.Event evnt in obj.Events[eventIdx])
                {
                    foreach (UndertaleGameObject.EventAction action in evnt.Actions)
                    {
                        if (action.CodeId.Name.Content.Contains(guid))
                        {
                            evnt.EventSubtype = objectIndex;
                        }
                    }
                }
            }
            catch
            {
                // Silently ignore, some values can be null along the way
            }
        }
    }

    public uint ReduceCollisionValue(List<uint> possibleValues)
    {
        if (possibleValues.Count == 1)
        {
            if (possibleValues[0] != uint.MaxValue)
                return possibleValues[0];

            // Nothing found, pick new one
            bool objFound = false;
            uint objIndex = 0;
            while (!objFound)
            {
                string objectIndex = SimpleTextInput("未找到对象。请在下方输入对象名称：",
                    "对象名称输入框", "", false).ToLower();
                for (int i = 0; i < Data.GameObjects.Count; i++)
                {
                    if (Data.GameObjects[i].Name.Content.Equals(objectIndex, StringComparison.InvariantCultureIgnoreCase))
                    {
                        objFound = true;
                        objIndex = (uint)i;
                    }
                }
            }
            return objIndex;
        }

        if (possibleValues.Count != 0)
        {
            // 2 or more possible values, make a list to choose from

            string gameObjectNames = "";
            foreach (uint objID in possibleValues)
                gameObjectNames += Data.GameObjects[(int)objID].Name.Content + "\n";

            bool objFound = false;
            uint objIndex = 0;
            while (!objFound)
            {
                string objectIndex = SimpleTextInput("找到多个对象。请从下列对象中选择一个；若均不匹配，也可输入其他对象名称：",
                    "对象名称输入框", gameObjectNames, true).ToLower();
                for (int i = 0; i < Data.GameObjects.Count; i++)
                {
                    if (Data.GameObjects[i].Name.Content.Equals(objectIndex, StringComparison.InvariantCultureIgnoreCase))
                    {
                        objFound = true;
                        objIndex = (uint)i;
                    }
                }
            }
            return objIndex;
        }

        return 0;
    }

    public List<uint> GetCollisionValueFromCodeNameGUID(string codeName)
    {
        int eventIdx = Convert.ToInt32(EventType.Collision);
        List<uint> possibleValues = new List<uint>();
        for (var i = 0; i < Data.GameObjects.Count; i++)
        {
            UndertaleGameObject obj = Data.GameObjects[i];
            try
            {
                foreach (UndertaleGameObject.Event evnt in obj.Events[eventIdx])
                {
                    foreach (UndertaleGameObject.EventAction action in evnt.Actions)
                    {
                        if (action.CodeId.Name.Content == codeName)
                        {
                            if (Data.GameObjects[(int)evnt.EventSubtype] != null)
                            {
                                possibleValues.Add(evnt.EventSubtype);
                                return possibleValues;
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silently ignore, some values can be null along the way
            }
        }
        possibleValues = GetCollisionValueFromGUID(GetGUIDFromCodeName(codeName));
        return possibleValues;
    }

    public List<uint> GetCollisionValueFromGUID(string guid)
    {
        int eventIdx = Convert.ToInt32(EventType.Collision);
        List<uint> possibleValues = new List<uint>();
        for (int i = 0; i < Data.GameObjects.Count; i++)
        {
            UndertaleGameObject obj = Data.GameObjects[i];
            try
            {
                foreach (UndertaleGameObject.Event evnt in obj.Events[eventIdx])
                {
                    foreach (UndertaleGameObject.EventAction action in evnt.Actions)
                    {
                        if (action.CodeId.Name.Content.Contains(guid))
                        {
                            if (!possibleValues.Contains(evnt.EventSubtype))
                            {
                                possibleValues.Add(evnt.EventSubtype);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Silently ignore, some values can be null along the way
            }
        }

        if (possibleValues.Count == 0)
        {
            possibleValues.Add(uint.MaxValue);
            return possibleValues;
        }
        else
        {
            return possibleValues;
        }
    }

    public string GetGUIDFromCodeName(string codeName)
    {
        string afterPrefix = codeName.Substring(11);
        if (afterPrefix.LastIndexOf("_Collision_") != -1)
        {
            string s2 = "_Collision_";
            return afterPrefix.Substring(afterPrefix.LastIndexOf("_Collision_") + s2.Length, afterPrefix.Length - (afterPrefix.LastIndexOf("_Collision_") + s2.Length));
        }
        else
            return "Invalid";
    }

    #endregion

}
