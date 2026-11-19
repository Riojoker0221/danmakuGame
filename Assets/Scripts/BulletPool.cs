using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹对象池：统一创建、移动、回收。
/// 碰撞判定全部用圆形距离，不走物理引擎，弹幕量大了也稳。
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    public Sprite enemyBulletSprite;
    public Sprite playerBulletSprite;

    public int enemyCapacity = 900;
    public int playerCapacity = 160;

    public readonly List<Bullet> enemyBullets = new List<Bullet>();
    public readonly List<Bullet> playerBullets = new List<Bullet>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (enemyBulletSprite == null) enemyBulletSprite = SpriteFactory.Circle(8);
        if (playerBulletSprite == null) playerBulletSprite = SpriteFactory.Diamond(8);

        CreatePool(enemyBullets, enemyCapacity, enemyBulletSprite, true);
        CreatePool(playerBullets, playerCapacity, playerBulletSprite, false);
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void CreatePool(List<Bullet> list, int count, Sprite sprite, bool enemy)
    {
        for (int i = 0; i < count; i++)
        {
            var go = new GameObject(enemy ? "EnemyBullet" : "PlayerBullet");
            go.transform.SetParent(transform, false);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = enemy ? 20 : 15;

            var bullet = go.AddComponent<Bullet>();
            bullet.sr = sr;
            bullet.isEnemy = enemy;
            bullet.active = false;

            go.SetActive(false);
            list.Add(bullet);
        }
    }

    void Update()
    {
        Step(enemyBullets);
        Step(playerBullets);
    }

    void Step(List<Bullet> list)
    {
        float dt = Time.deltaTime;
        float hw = PlayArea.HalfWidth + 1.5f;
        float hh = PlayArea.HalfHeight + 1.5f;

        for (int i = 0; i < list.Count; i++)
        {
            var b = list[i];
            if (!b.active) continue;

            Vector2 p = (Vector2)b.transform.position + b.velocity * dt;
            b.transform.position = p;

            if (Mathf.Abs(p.x) > hw || Mathf.Abs(p.y) > hh)
                b.Kill();
        }
    }

    Bullet Take(List<Bullet> list)
    {
        for (int i = 0; i < list.Count; i++)
            if (!list[i].active) return list[i];
        return null;
    }

    public Bullet SpawnEnemy(Vector2 pos, Vector2 vel, float radius, int damage, Color color)
    {
        var b = Take(enemyBullets);
        if (b == null) return null;
        b.isEnemy = true;
        b.Reset(pos, vel, radius, damage, color, enemyBulletSprite);
        return b;
    }

    public Bullet SpawnPlayer(Vector2 pos, Vector2 vel, float radius, int damage)
    {
        var b = Take(playerBullets);
        if (b == null) return null;
        b.isEnemy = false;
        b.Reset(pos, vel, radius, damage, new Color(1f, 0.93f, 0.55f), playerBulletSprite);
        return b;
    }

    /// <summary>玩家判定点与敌弹的碰撞检测，返回命中的那颗弹（没有则 null）。</summary>
    public Bullet HitEnemyBullet(Vector2 point, float pointRadius)
    {
        for (int i = 0; i < enemyBullets.Count; i++)
        {
            var b = enemyBullets[i];
            if (!b.active) continue;
            Vector2 d = (Vector2)b.transform.position - point;
            float rr = b.radius + pointRadius;
            if (d.sqrMagnitude <= rr * rr) return b;
        }
        return null;
    }

    /// <summary>受击时清掉身边一圈弹，给玩家一点喘息空间。</summary>
    public void ClearNear(Vector2 point, float radius)
    {
        float r2 = radius * radius;
        for (int i = 0; i < enemyBullets.Count; i++)
        {
            var b = enemyBullets[i];
            if (!b.active) continue;
            if (((Vector2)b.transform.position - point).sqrMagnitude <= r2)
                b.Kill();
        }
    }

    public void ClearAllEnemyBullets()
    {
        for (int i = 0; i < enemyBullets.Count; i++)
            if (enemyBullets[i].active) enemyBullets[i].Kill();
    }
}
