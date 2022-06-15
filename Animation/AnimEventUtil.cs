using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Wod.Util
{
    /// <summary>
    /// 用来动态添加动画事件
    /// </summary>
    public class AnimEventUtil : MonoBehaviour
    {
        class EventItem
        {
            public string Param;
            public LuaFunction Callback;
            public AnimationClip Clip;
        }

        private readonly Dictionary<string, EventItem> _events = new Dictionary<string, EventItem>();
        
        public static void AddAnimEvent(GameObject aniObj, 
            string clipName, 
            float time, 
            LuaFunction callback)
        {
            var ani = aniObj.GetComponent<Animator>();
            if (ani == null)
                return;

            var util = aniObj.GetComponent<AnimEventUtil>();
            if (util == null)
                util = aniObj.AddComponent<AnimEventUtil>();
            
            util.AddEvent(ani, clipName, time, callback);
        }

        private void OnDestroy()
        {
            foreach (var v in _events)
            {
                v.Value.Clip.events = null;
            }
            _events.Clear();
        }

        public void AddEvent(
            Animator ani,
            string clipName,
            float time, 
            LuaFunction callback)
        {
            var key = clipName + time.ToString("F2");

            var evt = new AnimationEvent {functionName = "OnEvent", stringParameter = key, time = time};

            AnimationClip[] temp = ani.runtimeAnimatorController.animationClips;
            foreach (var t in temp)
            {
                if (t.name != clipName) 
                    continue;
                
                evt.time *= t.length; //计算动画实际时间点

                t.AddEvent(evt);
                
                var item = new EventItem {Param = evt.stringParameter, Callback = callback, Clip = t};

                if (_events.ContainsKey(key))
                {
                    _events[key].Callback = null;
                }
                    
                _events[key] = item;
                break;
            }
        }

        private void OnEvent(string key)
        {
            if (_events.ContainsKey(key))
            {
                _events[key].Callback.Call();
            }
        }
    }
}