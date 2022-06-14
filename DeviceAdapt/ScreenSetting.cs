using UnityEngine;
using Wod.Game.Evt;
using Wod.ThirdParty.Util;
using XLua;

[CSharpCallLua]
public class ScreenSetting : MonoBehaviour
{
    private const string KEY_SCREEN_RESO = "ScreenResolution";
    public static ScreenSetting Instance;

    public int ScreenOriWidth; //屏幕原分辨率宽度
    public int ScreenOriHeight; //屏幕原分辨率高度
    public Vector2 PixelOffset; //刘海屏适配

    int _widthFinal = 0, _heightFinal = 0;
    private float _scaleFactor;
    private int lastCheckFrame =0;
    private ScreenOrientation _lastOrigentation;
    // Start is called before the first frame update
    void Awake()
    {
        //单例
        Instance = this;
        //记录原分辨率
        ScreenOriWidth = Screen.width;
        ScreenOriHeight = Screen.height;
        //基础设置
        SetScreenOrientation();
    }

    private void Update()
    {
        lastCheckFrame++;
        if (lastCheckFrame < 30)
            return;
        lastCheckFrame = 0;
        if (Screen.orientation != _lastOrigentation)
        {
            _lastOrigentation = Screen.orientation;
            GetPixelOffset();
        }
    }

    //sdk拉起后才能设置高中低
    public void Init()
    {
        //根据PlayerPrefs设置高中低
        SetScreenResolution(GetScreenResolution());
        //获取刘海宽度
        GetPixelOffset();
    }

    //屏幕相关基础设置
    void SetScreenOrientation()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    //获取刘海宽度
    private void GetPixelOffset()
    {
        
        var safeArea = Screen.safeArea;
        var realFactor = _scaleFactor;
        if (safeArea.height <= _heightFinal)
        {
            realFactor = 1;
        }
        PixelOffset.x = safeArea.xMin * realFactor;
        PixelOffset.y = safeArea.xMax * realFactor - _widthFinal;
        
        LoggerHelper.Debug($"screen final {_widthFinal},{_heightFinal} PixelOffset {PixelOffset}");
        Messenger.Broadcast(UILayerEvent.SCREEN_ORIGENTATION_CHANGE);
    }
    

    //设置屏幕分辨率
    public void SetScreenResolution(int level)
    {
        
        //各个配置下最低分辨率
        const int LowMinHeight = 720;
        const int MiddleMinHeight = 860;
        const int HighMinHeight = 1080;
        //低配
        if (level == (int) DeviceQualityLevel.Low || level == (int) DeviceQualityLevel.Lite)
        {
            if (ScreenOriHeight <= LowMinHeight)
            {
                _widthFinal = ScreenOriWidth;
                _heightFinal = ScreenOriHeight;
            }
            else if (ScreenOriHeight / 2 < LowMinHeight)
            {
                _heightFinal = LowMinHeight;
                _widthFinal = ScreenOriWidth * LowMinHeight / ScreenOriHeight;
            }
            else
            {
                _widthFinal = ScreenOriWidth / 2;
                _heightFinal = ScreenOriHeight / 2;
            }
        }
        //中配
        else if (level == (int) DeviceQualityLevel.Middle)
        {
            if (ScreenOriHeight <= MiddleMinHeight)
            {
                _widthFinal = ScreenOriWidth;
                _heightFinal = ScreenOriHeight;
            }
            else if (ScreenOriHeight / 4 * 3 < MiddleMinHeight)
            {
                _heightFinal = MiddleMinHeight;
                _widthFinal = ScreenOriWidth * MiddleMinHeight / ScreenOriHeight;
            }
            else
            {
                _widthFinal = ScreenOriWidth / 4 * 3;
                _heightFinal = ScreenOriHeight / 4 * 3;
            }
        }
        //高配
        else
        {
            if (ScreenOriHeight <= HighMinHeight)
            {
                _widthFinal = ScreenOriWidth;
                _heightFinal = ScreenOriHeight;
            }
            else
            {
                _heightFinal = HighMinHeight;
                _widthFinal = ScreenOriWidth * HighMinHeight / ScreenOriHeight;
            }
        }
        
        _scaleFactor =  (float)_heightFinal / ScreenOriHeight;
        
        LoggerHelper.Debug($"[Screen Resolution]: origin width: {ScreenOriWidth}, origin height: {ScreenOriHeight}, width: {_widthFinal}, height: {_heightFinal}");

        //设置分辨率
#if !UNITY_EDITOR && (UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN)
#else
        Screen.SetResolution(_widthFinal, _heightFinal, true);
#endif
        //存档记录
        PlayerPrefs.SetInt(KEY_SCREEN_RESO, level);
    }

    //获取当前分辨率档位
    public int GetScreenResolution()
    {
        return PlayerPrefs.GetInt(KEY_SCREEN_RESO);
    }
}