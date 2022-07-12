using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kingdom.Mock;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace WOD.Editor
{
    public class LuaImporter
    {
        private readonly List<string> LuaExcept = new List<string>()
        {
            "exported1.lua",
            "exported2.lua"
        };

        [MenuItem("Tools/Copy Lua")]
        public static void ImportLuaFiles()
        {
            var path = Path.Combine(Application.dataPath, "../Lua");
            var destPath = Path.Combine(Application.dataPath, "_Res/Lua");

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (string newPath in Directory.GetFiles(destPath, "*.*", SearchOption.AllDirectories))
            {
                if (newPath.Contains(".gitkeep")||newPath.Contains(".gitkeep.meta"))
                    continue;
                
                File.Delete(newPath);
            }

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(path, destPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories))
            {
                var dest = newPath.Replace(path, destPath);
                var baseDir = Path.GetFullPath(destPath);
                var dir = Path.GetDirectoryName(dest);
                var fileName = Path.GetFileName(dest);

                if (fileName.Contains("&"))
                {
                    Debug.LogError($"Lua file name can not contain '_' in {newPath}");
                }

                var raletiveDir = dir.Substring(baseDir.Length);
                raletiveDir = raletiveDir.Replace("\\", "/");
                fileName = raletiveDir.Replace("/", "_") + "_" + fileName + ".txt";
                while (fileName.StartsWith("_"))
                    fileName = fileName.Substring(1);

                if (fileName[0] == '\\')
                {  // fix: 在windows系统上,文件名有时会带上 \ ,这会导致下面的路径拼接出错,这里临时修复一下
                    fileName = fileName.Substring(1);
                }


                var finalPath = Path.Combine(dir, fileName);
                var finalDestPath = Path.GetDirectoryName(finalPath);
                if (!Directory.Exists(finalDestPath))
                {  // 确保目标路径存在
                    Directory.CreateDirectory(finalDestPath);
                }

                File.Copy(newPath, finalPath.ToLower(), true);

                var content = File.ReadAllLines(finalPath.ToLower());
                StringBuilder sb = new StringBuilder();
                bool debug = false;
                foreach (var line in content)
                {
                    if (line.Contains("#if DEBUG"))
                    {
                        debug = true;
                    }

                    if (!debug)
                        sb.AppendLine(line);

                    if (debug && line.Contains("#endif"))
                    {
                        debug = false;
                    }
                }

                if (debug != false)
                {
                    Debug.LogError($"DEBUG macro in {newPath} do not closed properly.");
                }

                File.WriteAllText(finalPath, sb.ToString());
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 编译luac，生成通用字节码
        /// </summary>
        /// <param name="force"></param>
        public static void ProcessLuac(string channelName)
        {
            Debug.Log($"chanelName -> {channelName}");
            bool delOrigin = !channelName.Equals("wod");

            if (delOrigin)
            {
                Debug.Log("This is not develop package ,so will delete original lua content");
            }
            #if UNITY_EDITOR_WIN
            string luacPath = $"{Application.dataPath}/../../tools/luac.exe";
            #elif UNITY_EDITOR_OSX
            string luacPath = $"{Application.dataPath}/../../tools/luac";
            #endif
            var srcPath = Path.Combine(Application.dataPath, "_Res/Lua");
            var outputPath = Path.Combine(Application.dataPath, "_Res/Luac");
            
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            var srcFiles = Directory.GetFiles(srcPath, "*.txt", SearchOption.AllDirectories);

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = luacPath,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            var length = srcFiles.Length;
            for (int i = 0; i < length; i++)
            {
                var file = srcFiles[i];
                var srcFile = file.Replace("\\", "/");
                var outputFile = srcFile.Replace(srcPath, outputPath);
                outputFile = outputFile.Replace(".lua.txt", ".lua.bytes");
                var dir = Path.GetDirectoryName(outputFile);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var p = new Process();
                psi.Arguments = $"-o {outputFile} {srcFile}";
                p.StartInfo = psi;
                p.Start();
                using (var stream = p.StandardError)
                {
                    var output = stream.ReadToEnd();
                    p.StandardError.Close();
                    
                    if (delOrigin)
                    {
                        File.Delete(srcFile);
                    }
                    
                    if (!string.IsNullOrEmpty(output))
                    {
                        Debug.LogError(output);
                        continue;
                    }
                }
                
                p.Close();
                p.Dispose();
                
                Debug.Log($"process luac {srcFile}  -> {outputFile}");
                // var bts = File.ReadAllBytes(outputFile);
                // var btsFinal = EncryptHelper.EncryptBytes(bts);
                // File.WriteAllBytes(outputFile,btsFinal);
                
                EditorUtility.DisplayProgressBar("build luac",$"{i}/{length}",i/(float)length);
            }
            EditorUtility.ClearProgressBar();
            
            AssetDatabase.Refresh();
        }
        
        public static void DeleteFiles(string destPath)
        {
            //var destPath = Path.Combine(Application.dataPath, "_Res/Lua");
            if (Directory.Exists(destPath))
            {
                foreach (string newPath in Directory.GetFiles(destPath, "*.*", SearchOption.AllDirectories))
                {
                    if (newPath.Contains(".gitkeep") || newPath.Contains(".gitkeep.meta"))
                        continue;

                    File.Delete(newPath);
                }
                
                DirectoryInfo di = new DirectoryInfo(destPath);
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Hot Fix Luac")]
        public static void CopyLuaProcessLuacFiles()
        {
            //热更新lua，先拷贝lua，再走luc流程
            ImportLuaFiles();
            //chanelName 只要不是wod即可
            string channelName = "wod_sdk_master";
            ProcessLuac(channelName);
        }
    }
}