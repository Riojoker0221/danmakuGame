using UnityEngine;

/// <summary>
/// 游戏区域工具：世界坐标边界由正交相机实时计算，横竖屏都能自适应。
/// </summary>
public static class PlayArea
{
    public static Camera Cam
    {
        get
        {
            var cam = Camera.main;
            return cam;
        }
    }

    public static float HalfHeight
    {
        get
        {
            var cam = Cam;
            return cam != null ? cam.orthographicSize : 5f;
        }
    }

    public static float HalfWidth
    {
        get
        {
            var cam = Cam;
            if (cam == null) return 5f * 16f / 9f;
            return cam.orthographicSize * cam.aspect;
        }
    }

    /// <summary>玩家活动区域（留边距）。</summary>
    public static Vector2 ClampPlayer(Vector2 p)
    {
        float hw = HalfWidth - 0.35f;
        float hh = HalfHeight - 0.35f;
        p.x = Mathf.Clamp(p.x, -hw, hw);
        p.y = Mathf.Clamp(p.y, -hh + 1.1f, hh - 0.6f);
        return p;
    }
}
