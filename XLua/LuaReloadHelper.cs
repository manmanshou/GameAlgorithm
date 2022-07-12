using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Wod;

[InitializeOnLoad]
public class LuaReloadHelper
{
    static LuaReloadHelper()
    {
        EditorApplication.playModeStateChanged += onPlayModeStateChanged;
    }

    static void onPlayModeStateChanged(PlayModeStateChange state)
    {
    }

    [MenuItem("XLua/Reload")]
    public static void Reload()
    {
        var window = (LuaReloadWindow)EditorWindow.GetWindow(typeof(LuaReloadWindow));
        window.LoadFiles();
        window.Show();
        window.InitFileName();
    }
}


public class LuaReloadWindow : EditorWindow
{
    private string _files;
    string FindLua = "";

    void OnGUI()
    {
        if (!Application.isPlaying)
        {
            GUILayout.Label("The game is NOT running.");
            return;
        }

        if (LuaManager.Instance.Env == null)
        {
            GUILayout.Label("LuaManager.Instance.Env is null.");
            return;
        }

        var oldFiles = _files;
        GUILayout.Label("Lua Files: (example: ui.hero.HeroListView)");
        _files = EditorGUILayout.TextArea(_files, GUILayout.Height(300));
        if (_files != oldFiles)
            SaveFiles();

        if (GUILayout.Button("Reload"))
        {
            var a = _files.Split('\n');
            if (a.Length == 0)
            {
                ShowNotification(new GUIContent("No lua file to reload."));
                return;
            }

            var b = new List<string>();
            for (var i = 0; i < a.Length; i++)
            {
                var filename = a[i].Trim();
                if (filename.EndsWith(".lua"))
                    filename = filename.Substring(0, filename.Length - ".lua".Length);
                if (filename.StartsWith("wod_client/Lua/"))
                    filename = filename.Substring("wod_client/Lua/".Length);
                else if (filename.StartsWith("Lua/"))
                    filename = filename.Substring("Lua/".Length);
                filename = filename.Replace("/", ".");
                if (filename != string.Empty)
                    b.Add(filename);
            }

            if (b.Count == 0)
            {
                ShowNotification(new GUIContent("No lua file to reload."));
                return;
            }

            for (var i = 0; i < b.Count; i++)
            {
                var code1 = $"return hotfix(\"{b[i]}\")";
                var retVals1 = LuaManager.Instance.Env.DoString(code1);
                if (retVals1 == null || retVals1.Length == 0)
                {
                    var str = $"Failed to reload {b[i]}";
                    EditorUtility.DisplayDialog("Lua Reload", str, "OK");
                    return;
                }
            }

            ShowNotification(new GUIContent("Done."));
        }

        GUI.SetNextControlName("Focus");
        FindLua = EditorGUILayout.TextField("查找->点击添加",FindLua);

        if (!string.IsNullOrEmpty(FindLua))
        {
            foreach (var luaName in luaFilePaths)
            {
                if (!luaName.ToLower().Contains(FindLua.ToLower()))
                {
                    continue;
                }

                if (GUILayout.Button(luaName))
                {
                    if (!_files.Contains(luaName))
                    {
                        _files += luaName + '\n';
                    }
                    GUI.FocusControl("Focus");
                    Repaint();
                }
            }
        }
    }

    public void LoadFiles()
    {
        _files = EditorPrefs.GetString("LuaReloadWindow.Files");
    }

    public void SaveFiles()
    {
        EditorPrefs.SetString("LuaReloadWindow.Files", _files);
    }

    private string[] luaFilePaths;

    public void InitFileName()
    {
        string luaRootPath = Application.dataPath + "/../Lua/";

        List<string> luas = new List<string>();

        getdir(luaRootPath, ref luas);

        luaFilePaths = luas.ToArray();
    }

    private string CheckPath = "wod_client/Lua/";
    
    private void getdir(string path, ref List<string> luas)
    {
        try
        {
            string[] dir = Directory.GetDirectories(path);
            DirectoryInfo fdir = new DirectoryInfo(path);
            FileInfo[] file = fdir.GetFiles();
            if (file.Length != 0 || dir.Length != 0)    
            {
                foreach (FileInfo f in file)
                {
                    if (f.Extension.ToLower().Contains("lua"))
                    {
                        string lua = f.FullName;

                        int index = lua.IndexOf(CheckPath, StringComparison.Ordinal) + CheckPath.Length; 
                        
                        luas.Add(lua.Substring(index));
                    }
                }

                foreach (string d in dir)
                {
                    getdir(d,ref luas);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            throw ex;
        }
    }
}
