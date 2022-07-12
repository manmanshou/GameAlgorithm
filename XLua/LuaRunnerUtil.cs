using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using Wod;

public class LuaRunnerUtil
{
    [MenuItem("XLua/LuaRunner")]
    public static void LuaCodeInputer()
    {
        var window = (LuaRunnerWindow)EditorWindow.GetWindow(typeof(LuaRunnerWindow));
        window.Show();
        window.Init();
        window.minSize = new Vector2(480,350);
    }
}


public class LuaRunnerWindow : EditorWindow
{
    private readonly char ETX = (char) 0x03;//换行符
    // private readonly char EOF = (char) 0x04;//文件尾符
    private readonly char nextLine = '\n';
    // private readonly char empty = (char) 0x00;
    
    string command = "";
    
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

        GUILayout.Label("Lua代码");
        command = EditorGUILayout.TextArea(command, GUILayout.Height(300));

        // CS.UnityEngine.GameObject.CreatePrimitive (CS.UnityEngine.PrimitiveType.Cube)
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Run"))
        {
            RunLua(ref command);
        }

        GUI.SetNextControlName("ClearCommand");
        if (GUILayout.Button("Clear"))
        {
            command = "";
            GUI.FocusControl("ClearCommand");
            Repaint();
        }
        
        GUI.SetNextControlName("RunLuaRunner");
        if (GUILayout.Button("Run LuaRunner.lua"))
        {
            var fileName = Application.dataPath + LuaRunnerLua;
            var luaStrs = File.ReadAllLines(fileName);

            string luaRunnerStr = "";

            for (int i = 0; i < luaStrs.Length; i++)
            {
                luaRunnerStr += luaStrs[i] + '\n';
            }

            RunLua(ref luaRunnerStr);
            
            GUI.FocusControl("RunLuaRunner");
            Repaint();
        }

        GUI.SetNextControlName("C# GC");
        if (GUILayout.Button("C# GC"))
        {
            System.GC.Collect();
        }
    }

    void RunLua(ref string luaStr)
    {
        luaStr = luaStr.Replace(ETX, nextLine)/*.Replace(EOF, empty)*/;

        // Debug.LogError(luaStr);
        
        var doStringReturn = LuaManager.Instance.Env.DoString(luaStr);

        if (doStringReturn != null)
        {
            for (int i = 0; i < doStringReturn.Length; i++)
            {
                Debug.LogError(doStringReturn[i]);
            }
        }
    }


    private string LuaRunnerLua = "/../Lua/LuaRunner.lua";
    private void OnDestroy()
    {
        
        // Debug.LogError("OnDestroy");
        
        Debug.LogError("Delete -> " + Application.dataPath + LuaRunnerLua);
        if (File.Exists(Application.dataPath + LuaRunnerLua))
        {
            File.Delete(Application.dataPath + LuaRunnerLua);
        }
    }

    public void Init()
    {
        if (!File.Exists(Application.dataPath + LuaRunnerLua))
        {
            Debug.LogError("Create -> " + Application.dataPath + LuaRunnerLua);
            File.Create(Application.dataPath + LuaRunnerLua);
        }

        // Debug.LogError(File.Exists(Application.dataPath + LuaRunnerLua));
        
    }
}
