using System.Collections;
using UnityEngine;

public enum EnemyPattern
{
    RingBurst,   // 环形爆发
    AimedFan,    // 自机狙扇形
    Spiral,      // 螺旋
    Sniper,      // 单发精准狙击
    DoubleRing,  // 双环
}

/// <summary>
/// 魔物敌人：从上方入场 → 停住放弹 → 停留若干秒 → 退场。
/// 受击闪白，死亡爆炸并加分。玩家子弹命中检测也在这里做（敌人数量少，开销可控）。
/// </summary>
public class Enemy : MonoBehaviour
{
    public int hp = 30;
    public int scoreValue = 100;
    public float radius = 0.55f;
    public EnemyPattern pattern = EnemyPattern.RingBurst;
    public float fireInterval = 1.3f;
    public float bulletSpeed = 3.6f;
    public float stayTime = 4.5f;

    public SpriteRenderer sr;

    float stopY;
    float stayTimer;
    Color baseColor = Color.white;
    float flashTimer;
    bool dead;
    bool entered;
    int ringIndex;

    public void Init(float targetStopY, EnemyPattern p, int health)
    {
        stopY = targetStopY;
        pattern = p;
        hp = health;
        if (sr != null) baseColor = sr.color;
        StartCoroutine(EnterRoutine());
    }

    IEnumerator EnterRoutine()
    {
        Vector2 start = transform.position;
        Vector2 target = new Vector2(start.x, stopY);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.8f;
            transform.position = Vector2.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        entered = true;
        StartCoroutine(FireRoutine());
        yield return new WaitForSeconds(stayTime);
        StartCoroutine(ExitRoutine());
    }

    IEnumerator FireRoutine()
    {
        while (!dead)
        {
            yield return new WaitForSeconds(fireInterval);

            if (dead) yield break;
            if (GameManager.Instance != null &&
                (GameManager.Instance.IsGameOver || GameManager.Instance.IsPaused)) continue;

            FireOnce();
        }
    }

    void FireOnce()
    {
        Vector2 pos = transform.position;
        Vector2 playerPos = new Vector2(0f, -3f);
        var player = FindObjectOfType<PlayerController>();
        if (player != null) playerPos = player.transform.position;

        switch (pattern)
        {
            case EnemyPattern.RingBurst:
                BulletPatterns.Ring(pos, 20, bulletSpeed, BulletPatterns.Rose, ringIndex * 10f);
                ringIndex++;
                break;

            case EnemyPattern.AimedFan:
                BulletPatterns.AimedFan(pos, playerPos, 5, 32f, bulletSpeed, BulletPatterns.Orange);
                break;

            case EnemyPattern.Spiral:
                StartCoroutine(SpiralBurst(pos));
                break;

            case EnemyPattern.Sniper:
                BulletPatterns.Aimed(pos, playerPos, bulletSpeed * 1.5f, BulletPatterns.Gold, 0.13f, 2);
                BulletPatterns.Ring(pos, 8, bulletSpeed * 0.6f, BulletPatterns.Gold, 22.5f + ringIndex * 15f);
                ringIndex++;
                break;

            case EnemyPattern.DoubleRing:
                BulletPatterns.Ring(pos, 14, bulletSpeed, BulletPatterns.Violet, 0f);
                BulletPatterns.Ring(pos, 14, bulletSpeed * 0.65f, BulletPatterns.Ice, 12f);
                break;
        }
    }

    IEnumerator SpiralBurst(Vector2 origin)
    {
        float ang = 0f;
        for (int i = 0; i < 46; i++)
        {
            if (dead) yield break;
            BulletPatterns.Straight(origin, BulletPatterns.Deg2Dir(ang), bulletSpeed, BulletPatterns.Violet);
            ang += 21f;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.8f);
    }

    IEnumerator ExitRoutine()
    {
        entered = false;
        Vector2 start = transform.position;
        Vector2 target = new Vector2(start.x * 1.8f, PlayArea.HalfHeight + 3.5f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 1.2f;
            transform.position = Vector2.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        Destroy(gameObject);
    }

    void Update()
    {
        if (dead) return;

        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (sr != null) sr.color = flashTimer > 0f ? Color.white : baseColor;
        }

        CheckPlayerBullets();
    }

    void CheckPlayerBullets()
    {
        if (BulletPool.Instance == null) return;
        var list = BulletPool.Instance.playerBullets;
        Vector2 me = transform.position;

        for (int i = 0; i < list.Count; i++)
        {
            var b = list[i];
            if (!b.active) continue;

            Vector2 d = (Vector2)b.transform.position - me;
            float rr = b.radius + radius;
            if (d.sqrMagnitude <= rr * rr)
            {
                b.Kill();
                TakeDamage(b.damage);
                if (dead) return;
                break;
            }
        }
    }

    void TakeDamage(int dmg)
    {
        hp -= dmg;
        flashTimer = 0.06f;
        if (sr != null) sr.color = Color.white;

        if (hp <= 0) Die();
    }

    void Die()
    {
        dead = true;
        if (GameManager.Instance != null) GameManager.Instance.AddScore(scoreValue);
        ExplosionFx.Play(transform.position, baseColor, 10, 3.2f);
        Destroy(gameObject);
    }
}

/// <summary>敌人工厂：运行时创建，不依赖任何预制体资产。</summary>
public static class EnemyFactory
{
    public static Enemy Spawn(Vector2 pos, float stopY, EnemyPattern pattern, int hp, float stayTime = 4.5f)
    {
        var go = new GameObject("Enemy");
        go.transform.position = pos;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Circle(16);
        sr.sortingOrder = 10;

        switch (pattern)
        {
            case EnemyPattern.RingBurst: sr.color = new Color(0.9f, 0.3f, 0.45f); break;
            case EnemyPattern.AimedFan: sr.color = new Color(0.95f, 0.6f, 0.25f); break;
            case EnemyPattern.Spiral: sr.color = new Color(0.6f, 0.4f, 0.95f); break;
            case EnemyPattern.Sniper: sr.color = new Color(0.95f, 0.85f, 0.3f); break;
            case EnemyPattern.DoubleRing: sr.color = new Color(0.85f, 0.35f, 0.8f); break;
        }

        var e = go.AddComponent<Enemy>();
        e.sr = sr;
        e.pattern = pattern;
        e.stayTime = stayTime;
        e.Init(stopY, pattern, hp);
        return e;
    }
}
