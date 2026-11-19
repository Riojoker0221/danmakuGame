using UnityEngine;

/// <summary>
/// 子弹数据容器。运动和回收由 BulletPool 统一驱动，避免成百上千个 Update 开销。
/// </summary>
public class Bullet : MonoBehaviour
{
    public SpriteRenderer sr;

    [System.NonSerialized] public Vector2 velocity;
    [System.NonSerialized] public float radius = 0.1f;
    [System.NonSerialized] public int damage = 1;
    [System.NonSerialized] public bool isEnemy;
    [System.NonSerialized] public bool active;

    public void Reset(Vector2 pos, Vector2 vel, float r, int dmg, Color color, Sprite sprite)
    {
        transform.position = pos;
        velocity = vel;
        radius = r;
        damage = dmg;
        if (sprite != null) sr.sprite = sprite;
        sr.color = color;
        active = true;
        gameObject.SetActive(true);
    }

    public void Kill()
    {
        active = false;
        gameObject.SetActive(false);
    }
}
