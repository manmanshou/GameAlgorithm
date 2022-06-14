using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using XLua.LuaDLL;
using Debug = UnityEngine.Debug;

[DefaultExecutionOrder(-9999999)]
public class RenderParallelTasks : MonoBehaviour
{
    private static RenderParallelTasks _instance;

    public static RenderParallelTasks Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("RenderParallelTasks");
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<RenderParallelTasks>();
            }
            return _instance;
        }
    }

    private class TaskInfo
    {
        private readonly Action _action;
        private Task _threadTask;

        public TaskInfo(Action action)
        {
            _action = () =>
            {
                Profiler.BeginSample("RenderParallelTasks");
                action?.Invoke();
                Profiler.EndSample();
            };
        }

        public void Run()
        {
            if (_threadTask == null || _threadTask.IsCompleted)
            {
                _threadTask = Task.Run(_action);
            }
        }

        public void Wait()
        {
            if (_threadTask != null)
            {
                _threadTask.Wait();
            }
        }

        public bool IsRunning()
        {
            return _threadTask != null && !_threadTask.IsCompleted;
        }
    }

    public enum WaitMode
    {
        FixedUpdate, //首先尝试FixedUpdate中等待执行结束
        Update, //首先尝试Update中等待执行结束
    }

    public WaitMode mode = WaitMode.FixedUpdate;

    private Dictionary<Action, TaskInfo> _taskDict = new Dictionary<Action, TaskInfo>();

    private bool _isRegistered;
    private int _startFrame;
    private int _updateFrame;

    public void AddTask(Action action)
    {
        if (!_isRegistered)
        {
            RenderPipelineManager.beginFrameRendering += DoTaskInThread;
            RenderPipelineManager.endFrameRendering += WaitTaskInThread;
            _isRegistered = true;
        }
        
        _taskDict.Add(action, new TaskInfo(action));
    }

    public void RemoveTask(Action action)
    {
        if (_taskDict.TryGetValue(action, out var info))
        {
            info.Wait();
            _taskDict.Remove(action);
        }
        
        if (_isRegistered && _taskDict.Count <= 0)
        {
            RenderPipelineManager.beginFrameRendering -= DoTaskInThread;
            RenderPipelineManager.endFrameRendering -= WaitTaskInThread;
            _isRegistered = false;
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }

        TryWait();

        if (_isRegistered)
        {
            RenderPipelineManager.beginFrameRendering -= DoTaskInThread;
            RenderPipelineManager.endFrameRendering -= WaitTaskInThread;
            _isRegistered = false;
            _taskDict = null;
        }
    }

    private void DoTaskInThread(ScriptableRenderContext context, Camera[] cam)
    {
        if (_startFrame >= Time.frameCount)
        {
            return;
        }
        _startFrame = Time.frameCount;
        
        foreach (var taskInfo in _taskDict)
        {
            taskInfo.Value.Run();
        }
    }

    private void WaitTaskInThread(ScriptableRenderContext context, Camera[] cam)
    {
        TryWait();
    }

    private void TryWait()
    {
        if (_updateFrame >= Time.frameCount)
        {
            return;
        }
        _updateFrame = Time.frameCount;

        foreach (var taskInfo in _taskDict)
        {
            taskInfo.Value.Wait();
        }
    }
}