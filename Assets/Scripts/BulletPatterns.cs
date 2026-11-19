using UnityEngine;

/// <summary>
/// 弹幕发射工具箱。所有模式最终都走 BulletPool.SpawnEnemy。
/// </summary>
public static class BulletPatterns
{
    public static readonly Color Rose = new Color(1f, 0.32f, 0.55f);
    public static readonly Color Violet = new Color(0.72f, 0.38f, 1f);
    public static readonly Color Orange = new Color(1f, 0.55f, 0.25f);
    public static readonly Color Gold = new Color(1f, 0.82f, 0.3f);
    public static readonly Color Ice = new Color(0.5f, 0.9f, 1f);

    public static Vector2 Deg2Dir(float deg)
    {
        float rad = deg * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static float Dir2Deg(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public static void Straight(Vector2 pos, Vector2 dir, float speed, Color color,
                               float radius = 0.11f, int damage = 1)
    {
        if (BulletPool.Instance == null) return;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.down;
        BulletPool.Instance.SpawnEnemy(pos, dir.normalized * speed, radius, damage, color);
    }

    /// <summary>环形弹幕（向四周均匀散开）。</summary>
    public static void Ring(Vector2 pos, int count, float speed, Color color,
                            float startDeg = 0f, float radius = 0.11f, int damage = 1)
    {
        for (int i = 0; i < count; i++)
        {
            float deg = startDeg + 360f * i / count;
            Straight(pos, Deg2Dir(deg), speed, color, radius, damage);
        }
    }

    /// <summary>扇形弹幕，centerDeg 为中心方向。</summary>
    public static void Fan(Vector2 pos, float centerDeg, float spreadDeg, int count, float speed, Color color,
                           float radius = 0.11f, int damage = 1)
    {
        if (count <= 1)
        {
            Straight(pos, Deg2Dir(centerDeg), speed, color, radius, damage);
            return;
        }
        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);
            float deg = centerDeg - spreadDeg * 0.5f + spreadDeg * t;
            Straight(pos, Deg2Dir(deg), speed, color, radius, damage);
        }
    }

    /// <summary>自机狙：朝玩家当前位置射一发。</summary>
    public static void Aimed(Vector2 pos, Vector2 target, float speed, Color color,
                             float radius = 0.11f, int damage = 1)
    {
        Straight(pos, (target - pos), speed, color, radius, damage);
    }

    /// <summary>自机狙扇形（多方向锁定玩家）。</summary>
    public static void AimedFan(Vector2 pos, Vector2 target, int count, float spreadDeg, float speed, Color color,
                                float radius = 0.11f, int damage = 1)
    {
        float center = Dir2Deg(target - pos);
        Fan(pos, center, spreadDeg, count, speed, color, radius, damage);
    }
}
