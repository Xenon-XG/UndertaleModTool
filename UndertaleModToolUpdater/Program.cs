using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace UndertaleModToolUpdater
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "UndertaleModTool 更新程序";

            // Get all processes named UndertaleModTool
            Process[] utmtInstances = Process.GetProcessesByName("UndertaleModTool");
            
            if (utmtInstances.Length > 0)
            {
                // Found some! Wait for all of them to close
                Console.WriteLine("正在等待 UndertaleModTool 关闭...");
                foreach (var instance in utmtInstances)
                {
                    instance.WaitForExit();
                }
            }

            string basePath = Path.Join(Path.GetTempPath(), "UndertaleModTool") + Path.DirectorySeparatorChar;
            string appPath = null;

            // Check if Update.zip actually exists
            if (!File.Exists(basePath + "Update.zip")) {
                Console.WriteLine("缺少 Update.zip！本程序不应单独运行，请通过 UndertaleModTool 进行更新。");
                Console.WriteLine("按任意键退出...");
                Console.Read();
                Environment.Exit(1);
            }

            // If this exists from a failed update or something, then remove it
            if (Directory.Exists(basePath + "Update"))
            {
                Console.WriteLine("正在移除 Update 文件夹...");
                Directory.Delete(basePath + "Update", true);
            }

            if (!File.Exists("actualAppFolder"))
            {
                Console.WriteLine("缺少 \"actualAppFolder\" 文件！");
                Console.WriteLine("按任意键退出...");
                Console.Read();
                Environment.Exit(1);
            }
            else
            {
                appPath = File.ReadAllText("actualAppFolder");
                File.Delete("actualAppFolder");
            }

            // Extract the update ZIP
            Console.WriteLine("正在解压 Update.zip...");
            ZipFile.ExtractToDirectory(basePath + "Update.zip", basePath + "Update", true);
            Console.WriteLine("正在删除 Update.zip...");
            File.Delete(basePath + "Update.zip");

            Console.WriteLine("正在用更新文件替换原有文件...");
            MoveDirectory(basePath + "Update", appPath);

            Console.WriteLine("更新完成，正在启动 UTMT...");

            Process.Start(new ProcessStartInfo(Path.Join(appPath, "UndertaleModTool.exe"))
            {
                WorkingDirectory = appPath,
                Arguments = "deleteTempFolder"
            });
            
            Environment.Exit(0);
        }

        // source - https://stackoverflow.com/a/2553245/12136394
        static void MoveDirectory(string source, string target)
        {
            var files = Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));

            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(source, target);
                Directory.CreateDirectory(targetFolder);

                foreach (var file in folder)
                {
                    var targetFile = Path.Join(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile))
                        File.Delete(targetFile);
                    
                    File.Move(file, targetFile);
                }
            }

            Directory.Delete(source, true);
        }
    }
}
