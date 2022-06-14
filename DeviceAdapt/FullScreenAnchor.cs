using UnityEngine;
using Wod.Game.Evt;
using Wod.ThirdParty.Util;

public class FullScreenAnchor : MonoBehaviour
{
    RectTransform  trans;
    void Start()
    {
        trans = GetComponent<RectTransform>();
        OrigentationChange();
        Messenger.AddListener(UILayerEvent.SCREEN_ORIGENTATION_CHANGE, OrigentationChange);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener(UILayerEvent.SCREEN_ORIGENTATION_CHANGE, OrigentationChange);
    }

    private void OrigentationChange()
    {
        trans.offsetMin = new Vector2(ScreenSetting.Instance.PixelOffset.x, 0.0f);
        trans.offsetMax = new Vector2(ScreenSetting.Instance.PixelOffset.y,1.0f);
    }
}
