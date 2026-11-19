using UnityEngine;

/// <summary>
/// 运行时程序化生成像素风 Sprite，不依赖任何美术资源。
/// 生成的纹理统一为白色 + 透明，实际颜色由 SpriteRenderer.color 染色。
/// </summary>
public static class SpriteFactory
{
    static Texture2D MakeTex(int w, int h, System.Func<float, float, bool> inside)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;
        tex.wrapMode = TextureWrapMode.Clamp;

        var px = new Color[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float fx = (x + 0.5f) / w;
                float fy = (y + 0.5f) / h;
                px[y * w + x] = inside(fx, fy) ? Color.white : Color.clear;
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        return tex;
    }

    static Sprite ToSprite(Texture2D tex)
    {
        // ppu = 像素数，让 sprite 正好 1 个世界单位大
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
    }

    public static Sprite Square(int px = 8)
    {
        return ToSprite(MakeTex(px, px, (fx, fy) => true));
    }

    public static Sprite Circle(int px = 8)
    {
        return ToSprite(MakeTex(px, px, (fx, fy) =>
        {
            float dx = fx * 2f - 1f;
            float dy = fy * 2f - 1f;
            return dx * dx + dy * dy <= 1f;
        }));
    }

    public static Sprite Diamond(int px = 8)
    {
        return ToSprite(MakeTex(px, px, (fx, fy) =>
        {
            float dx = Mathf.Abs(fx * 2f - 1f);
            float dy = Mathf.Abs(fy * 2f - 1f);
            return dx + dy <= 1.05f;
        }));
    }

    /// <summary>朝上的玩家机体三角。</summary>
    public static Sprite Triangle(int px = 10)
    {
        return ToSprite(MakeTex(px, px, (fx, fy) =>
        {
            if (fy < 0.06f) return false;
            float halfW = 0.5f * (1f - fy);
            return Mathf.Abs(fx - 0.5f) <= halfW + 0.02f;
        }));
    }

    /// <summary>带描边的圆（魔物 / 弹幕，外圈比内圈暗一点的感觉靠双色实现不了，这里只出形状）。</summary>
    public static Sprite Ring(int px = 8, float thickness = 0.35f)
    {
        return ToSprite(MakeTex(px, px, (fx, fy) =>
        {
            float dx = fx * 2f - 1f;
            float dy = fy * 2f - 1f;
            float d = Mathf.Sqrt(dx * dx + dy * dy);
            return d <= 1f && d >= 1f - thickness * 2f;
        }));
    }
}
