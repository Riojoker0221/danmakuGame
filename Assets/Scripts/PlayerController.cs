using UnityEngine;

/// <summary>
/// 玩家机体：WASD/方向键移动，Shift 慢速精准移动，空格/Z/J 射击。
/// 判定点永远可见（像素小圆点），碰弹由 BulletPool 做圆形距离判定。
/// </summary>
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6.2f;
    public float focusSpeed = 2.6f;
    public float hitRadius = 0.09f;
    public int maxHealth = 3;
    public float invincibleTime = 2f;
    public float fireInterval = 0.08f;

    public SpriteRenderer body;
    public Transform hitPoint;
    public SpriteRenderer hitPointSr;

    Vector2 pos;
    int health;
    float invTimer;
    float fireCd;
    bool dead;

    void Start()
    {
        pos = transform.position;
        health = maxHealth;

        if (body != null && body.sprite == null)
        {
            body.sprite = SpriteFactory.Triangle(12);
            body.color = new Color(0.5f, 0.85f, 1f);
        }
        if (hitPointSr != null && hitPointSr.sprite == null)
            hitPointSr.sprite = SpriteFactory.Circle(6);

        if (GameManager.Instance != null) GameManager.Instance.SetHealth(health);
    }

    void Update()
    {
        if (dead) return;
        if (GameManager.Instance != null &&
            (GameManager.Instance.IsPaused || GameManager.Instance.IsGameOver)) return;

        bool focus = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        UpdateInvincible();
        HandleMove(focus);
        HandleFire(focus);
        UpdateHitPoint(focus);

        if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.K))
        {
            if (GameManager.Instance != null) GameManager.Instance.UseBomb();
        }

        CheckCollision();
    }

    void UpdateInvincible()
    {
        if (invTimer > 0f)
        {
            invTimer -= Time.deltaTime;
            if (body != null)
            {
                float a = Mathf.PingPong(Time.time * 14f, 1f) > 0.5f ? 0.25f : 1f;
                var c = body.color;
                c.a = a;
                body.color = c;
            }
        }
        else if (body != null && body.color.a < 1f)
        {
            var c = body.color;
            c.a = 1f;
            body.color = c;
        }
    }

    void HandleMove(bool focus)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = new Vector2(h, v);
        if (dir.sqrMagnitude > 1f) dir.Normalize();

        float sp = focus ? focusSpeed : moveSpeed;
        pos += dir * sp * Time.deltaTime;
        pos = PlayArea.ClampPlayer(pos);
        transform.position = pos;
    }

    void HandleFire(bool focus)
    {
        fireCd -= Time.deltaTime;
        bool want = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.J);
        if (!want || fireCd > 0f) return;

        fireCd = fireInterval;
        if (BulletPool.Instance == null) return;

        Vector2 muzzle = (Vector2)transform.position + new Vector2(0f, 0.45f);
        float spread = focus ? 0.07f : 0.16f;
        BulletPool.Instance.SpawnPlayer(muzzle + new Vector2(-spread, 0f), new Vector2(0f, 17f), 0.05f, 4);
        BulletPool.Instance.SpawnPlayer(muzzle + new Vector2(spread, 0f), new Vector2(0f, 17f), 0.05f, 4);
    }

    void UpdateHitPoint(bool focus)
    {
        if (hitPointSr == null) return;
        hitPointSr.enabled = true;
        hitPointSr.transform.localPosition = Vector3.zero;
        hitPointSr.transform.localScale = Vector3.one * (focus ? 0.26f : 0.18f);
        var c = hitPointSr.color;
        c.a = focus ? 1f : 0.65f;
        hitPointSr.color = c;
    }

    void CheckCollision()
    {
        if (invTimer > 0f) return;
        if (BulletPool.Instance == null) return;

        Vector2 p = transform.position;
        var hit = BulletPool.Instance.HitEnemyBullet(p, hitRadius);
        if (hit == null) return;
        hit.Kill();
        Damage();
    }

    void Damage()
    {
        health--;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetHealth(health);
            GameManager.Instance.PlayHitShake();
        }
        if (BulletPool.Instance != null) BulletPool.Instance.ClearNear(transform.position, 2.4f);
        invTimer = invincibleTime;
        if (health <= 0) Die();
    }

    void Die()
    {
        dead = true;
        ExplosionFx.Play(transform.position, new Color(0.5f, 0.85f, 1f), 12, 4f);
        if (GameManager.Instance != null) GameManager.Instance.OnPlayerDead();
        gameObject.SetActive(false);
    }
}
