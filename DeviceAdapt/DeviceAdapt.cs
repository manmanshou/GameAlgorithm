
using System;
using System.Collections;
#if USE_SDK
using com.kingsgroup.sdk;
#endif
using conf;
using UnityEngine;
using Wod.ThirdParty.Util;
using XLua;

/// <summary>
/// 设备分级等级
/// </summary>

public enum DeviceQualityLevel
{
    None = 0,       //无
    Lite = 1,       //极低
    Low = 2,       //低配
    Middle = 3,    //中配
    High = 4,      //高配
}


public enum DeviceFrameLevel
{
    None = 0,
    Low = 1,        //30 fps
    High = 2,       //60 fps
}


public enum DeviceMemoryLevel
{
    None = 0,
    Low = 1,        //小于3G
    Middle = 2,     //3G - 4G
    High =  3,      //大于 4G
}

/// <summary>
/// 设备适配类
/// </summary>

public class DeviceAdapt
{
    private const string KEY_ADAPT_FIRST = "DeviceAdaptFirst";
    private const string KEY_ADAPT = "DeviceAdapt";
    private const string KEY_GAME_FRAME = "GameFrame";
    private const string KEY_SCREEN_RESO = "ScreenResolution";

    public const string KEY_DEVICE_QUALITY_INFIXCFG = "DeviceQualityInCfg";
    
    //设备分级
    private static DeviceQualityLevel level = DeviceQualityLevel.None;

    public static bool PowerSaveModeActive = false;
    public static DeviceQualityLevel Level
    {
        get
        {
            if (level == DeviceQualityLevel.None)
            {
                level = (DeviceQualityLevel)PlayerPrefs.GetInt(KEY_ADAPT);
                OnQualityLevelChanged();
            }
            return level;
        }
        set
        {
            if (level != value)
            {
                PlayerPrefs.SetInt(KEY_ADAPT, (int)value);
                PlayerPrefs.Save();
                level = value;
                OnQualityLevelChanged();
            }
        }
    }

    // level 变化时，更新管线相关质量设置
    private static void OnQualityLevelChanged()
    {
        // 设置渲染质量
        if (level <= DeviceQualityLevel.Low)
        {
            QualitySettings.SetQualityLevel(2);
            //Shader.globalMaximumLOD = 100;
        }
        else if (level == DeviceQualityLevel.Middle)
        {
            QualitySettings.SetQualityLevel(1);   
            //Shader.globalMaximumLOD = 300;
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
            //Shader.globalMaximumLOD = 600;
        }
        
        Messenger.Broadcast("device_quality_level_changed");
    }
    
    //内存分级
    private static DeviceMemoryLevel memoryLevel = DeviceMemoryLevel.None;
    public static DeviceMemoryLevel MemoryLevel
    {
        get
        {
            if (memoryLevel == DeviceMemoryLevel.None)
            {
                var mem = SystemInfo.systemMemorySize;
                if (mem < 1024 * 3)
                    memoryLevel = DeviceMemoryLevel.Low;
                else if (mem < 1024 * 4)
                    memoryLevel = DeviceMemoryLevel.Middle;
                else
                    memoryLevel = DeviceMemoryLevel.High;
            }

            return memoryLevel;
        }
    }
    
    //刷新率分级
    private static DeviceFrameLevel gameFrameLevel = 0;
    public static DeviceFrameLevel GameFrameLevel
    {
        get
        {
            if (gameFrameLevel == 0)
            {
                gameFrameLevel = (DeviceFrameLevel)PlayerPrefs.GetInt(KEY_GAME_FRAME);
            }
            return gameFrameLevel;
        }
        set
        {
            if (gameFrameLevel != value)
            {
                PlayerPrefs.SetInt(KEY_GAME_FRAME, (int)value);
                PlayerPrefs.Save();
            }

            gameFrameLevel = value;
        }
    }

    //一系列便捷属性，外部不需要拿到Level等属性去自己比对
    public static bool IsDeviceHigh => level == DeviceQualityLevel.High;
    public static bool IsDeviceMiddle => level == DeviceQualityLevel.Middle;
    public static bool IsDeviceLow => level == DeviceQualityLevel.Low;
    public static bool IsDeviceLite => level == DeviceQualityLevel.Lite;
    
    public static bool IsMemoryHigh => memoryLevel == DeviceMemoryLevel.High;
    public static bool IsMemoryMiddle => memoryLevel == DeviceMemoryLevel.Middle;
    public static bool IsMemoryLow => memoryLevel == DeviceMemoryLevel.Low;
    
    public static bool IsFrameHigh => gameFrameLevel == DeviceFrameLevel.High;
    public static bool IsFrameMiddle => gameFrameLevel == DeviceFrameLevel.Low;

    static string GetOSString()
    {
        const string EDITOR = "editor";
        const string ANDORID = "android";
        const string IOS = "ios";
#if UNITY_EDITOR
        return EDITOR;
#elif UNITY_ANDROID
        return ANDORID;
#elif UNITY_IOS
        return IOS;
#endif
    }

    public static int GetDeviceQualityInCfg()
    {
        // 优先读取上次记录的
        int quality = PlayerPrefs.GetInt(KEY_DEVICE_QUALITY_INFIXCFG, -1);
        if (quality != -1)
        {
            return quality;
        }

        // 如果没有记录，优先读取Table里的配置表
        if (Tables.QualityDeviceInfoConf.Ids != null)
        {
            return GetDeviceQualityLevelByConfig();
        }

        // 如果还没有初始化配置表，则读取随包配置里的
        int deviceQuality = (int) DeviceQualityLevel.Low;
        {
            Hashtable loadingStageDeviceInfo = null;
            var textAsset = Resources.Load<TextAsset>("quality_device_info.json");
            if (textAsset != null)
            {
                loadingStageDeviceInfo = (Hashtable) MiniJSON.jsonDecode(textAsset.text);
                if (loadingStageDeviceInfo != null)
                {
                    try
                    {
                        string osName = GetOSString();
                        foreach (Hashtable value in loadingStageDeviceInfo.Values)
                        {
                            string gpu = value["Gpu"].ToString();
                            string Os = value["Os"].ToString();
                            if (SystemInfo.graphicsDeviceName.Equals(gpu) && osName.Equals(Os))
                            {
                                deviceQuality = Convert.ToInt32(value["QualityLevel"]);
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LoggerHelper.Error("read quality_device_info.json failed! "+ e.ToString());
                        //LoggerHelper.Error("read quality_device_info.json failed! "+ e.ToString());
                    }
                }
                Resources.UnloadAsset(textAsset);
            }
        }
        PlayerPrefs.SetInt(KEY_DEVICE_QUALITY_INFIXCFG, deviceQuality);
        return deviceQuality;
    }

    public static void Init()
    {
        //关闭垂直同步
        QualitySettings.vSyncCount = 0;
        //首次适配
        _FirstSetting();
        //屏幕适配
        ScreenSetting.Instance.Init();
        //设置锁帧
        SetGameFrameLevel(GameFrameLevel);
        //设置Unity QualitySettings 
        InterQualitySetting(Level);
        //设置RM中的加载阈值
        SetResourceManagerLoadingThroughput(Level);
        
        int deviceQualityInTbl = DeviceAdapt.GetDeviceQualityLevelByConfig();
        LoggerHelper.Debug($"DeviceQualityInCfg:{deviceQualityInTbl}");
#if USE_SDK
        if (FunctionUtil.UseSdk)
        {
            if (KGSDK.GetInstance().kgAppData != null)
            {
                KGSDK.GetInstance().kgAppData.device_quality_level = deviceQualityInTbl;
            }
        }
#endif
        PlayerPrefs.SetInt(KEY_DEVICE_QUALITY_INFIXCFG,deviceQualityInTbl);
    }
    
    
    
    //设置帧率档位
    public static void SetGameFrameLevel(DeviceFrameLevel level)
    {
        //设置帧率
        if (level == DeviceFrameLevel.Low)
        {
            Application.targetFrameRate = 30;
        }
        else
        {
#if UNITY_EDITOR
            Application.targetFrameRate = -1;
#else
            Application.targetFrameRate = 40;
#endif
        }

        //存档记录
        GameFrameLevel = level;
    }
    
    //设置帧率档位
    public static void InterQualitySetting(DeviceQualityLevel level)
    {
        if (level <= DeviceQualityLevel.Low)
        {
            //3d贴图分辨率压缩一半
            QualitySettings.masterTextureLimit = 1;
        }
    }


    /// <summary>
    /// 设置资源加载队列长度
    /// </summary>
    /// <param name="level"></param>
    public static void SetResourceManagerLoadingThroughput(DeviceQualityLevel level)
    {
        
#if UNITY_EDITOR
        ResourceManager.Instance.LoadingAssetThroughput = 20000; 
        LoggerHelper.Debug($"SetResourceManagerLoadingThroughput UNITY_EDITOR Set LoadingAssetThroughput 20000");
#elif !UNITY_EDITOR && (UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
        ResourceManager.Instance.LoadingAssetThroughput = 20000; 
        LoggerHelper.Debug($"SetResourceManagerLoadingThroughput UNITY_STANDALONE_OSX||UNITY_STANDALONE_WIN Set LoadingAssetThroughput 20000");
#else
        switch (level)
        {
            case DeviceQualityLevel.Lite:
            case DeviceQualityLevel.Low:
                ResourceManager.Instance.LoadingAssetThroughput = 100; 
                break;
            case DeviceQualityLevel.Middle:
                ResourceManager.Instance.LoadingAssetThroughput = 200;
                break;
            case DeviceQualityLevel.High:
                ResourceManager.Instance.LoadingAssetThroughput = 20000;
                break;
            default:
                ResourceManager.Instance.LoadingAssetThroughput = 1000;
                break;
        }
        LoggerHelper.Debug($"SetResourceManagerLoadingThroughput {level} Set LoadingAssetThroughput {ResourceManager.Instance.LoadingAssetThroughput}");
#endif
        
    }
    
    /// <summary>
    /// 根据配置获取当前设备分级
    /// </summary>
    /// <returns></returns>
    public static  int GetDeviceQualityLevelByConfig()
    {
        const string EDITOR = "editor";
        const string ANDORID = "android";
        const string IOS = "ios";
        string osName = "";
#if UNITY_EDITOR
        osName = EDITOR;
#elif UNITY_ANDROID
        osName = ANDORID;
#elif UNITY_IOS
        osName = IOS;
#endif
        if (osName == "" || osName == EDITOR)
        {
            return (int) DeviceQualityLevel.High;
        }

        for (int i = 0; i < Tables.QualityDeviceInfoConf.Ids.Length; i++)
        {
            long id = Tables.QualityDeviceInfoConf.Ids[i];
            QualityDeviceInfoConfItem confItem = Tables.QualityDeviceInfoConf[id];
            if (confItem.Gpu == SystemInfo.graphicsDeviceName && confItem.Os == osName)
            {
                return confItem.QualityLevel;
            }
        }

        return (int) DeviceQualityLevel.Low;
    }
    
    //首次启动读取配置
    private static void _FirstSetting()
    {
        //首次登陆
        if (!PlayerPrefs.HasKey(KEY_ADAPT_FIRST))
        {
            PlayerPrefs.SetInt(KEY_ADAPT_FIRST, 1);
            //根据机型设置分辨率档位
            int level = GetDeviceQualityLevelByConfig();
            PlayerPrefs.SetInt(KEY_DEVICE_QUALITY_INFIXCFG, level);
            LoggerHelper.Debug($"[Device Quality]: GetDeviceQualityLevelByConfig() GraphicsDeviceName is: {SystemInfo.graphicsDeviceName} result is: {(DeviceQualityLevel) level}");
            
            PlayerPrefs.SetInt(KEY_SCREEN_RESO, level);
            PlayerPrefs.SetInt(KEY_ADAPT, level);
            //中配
            PlayerPrefs.SetInt(KEY_GAME_FRAME, (int)(level == (int)DeviceQualityLevel.High ? DeviceFrameLevel.High : DeviceFrameLevel.Low));
            PlayerPrefs.Save();
        }
    }


    public static float TryGetScreenBrightness()
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_IPHONE
        return Screen.brightness;
#elif UNITY_ANDROID
        AndroidJavaObject Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); 
        AndroidJavaObject Window = Activity.Call<AndroidJavaObject>("getWindow"); 
        AndroidJavaObject Attributes = Window.Call<AndroidJavaObject>("getAttributes"); 
        float brightness = Attributes.Get<float>("screenBrightness");
        return brightness;
#else
        return Screen.brightness;
#endif
    }
    public static void TrySetScreenBrightness(float brightness)
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_IPHONE
        Screen.brightness = brightness;
#elif UNITY_ANDROID
        AndroidJavaObject Activity = null;
        Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        Activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            AndroidJavaObject Window = null, Attributes = null;
            Window = Activity.Call<AndroidJavaObject>("getWindow");
            Attributes = Window.Call<AndroidJavaObject>("getAttributes");
            Attributes.Set("screenBrightness", brightness);
            Window.Call("setAttributes", Attributes);
        }));
#else
        Screen.brightness = brightness;
#endif
        
        
    }
}