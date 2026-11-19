using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局游戏状态：分数、生命、GameOver、相机震屏。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score;
    public int health = 3;
    public int bombs = 2;
    public bool IsGameOver { get; private set; }
    public bool IsPaused { get; private set; }

    Camera cam;
    Vector3 camBase;
    float shake;

    void Awake()
    {
        Instance = this;
    }

    void EnsureCamera()
    {
        if (cam != null) return;
        cam = Camera.main;
        if (cam != null) camBase = cam.transform.position;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Update()
    {
        EnsureCamera();

        if (IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        if (shake > 0f)
        {
            shake -= Time.deltaTime;
            if (cam != null)
            {
                cam.transform.position = camBase + (Vector3)(Random.insideUnitCircle * 0.15f);
                if (shake <= 0f) cam.transform.position = camBase;
            }
        }
    }

    public void AddScore(int v)
    {
        if (IsGameOver) return;
        score += v;
    }

    public void SetHealth(int v) => health = v;

    public void PlayHitShake() => shake = 0.22f;

    public void OnPlayerDead()
    {
        IsGameOver = true;
        if (cam != null) cam.transform.position = camBase;
    }

    public void UseBomb()
    {
        if (bombs <= 0) return;
        bombs--;
        if (BulletPool.Instance != null) BulletPool.Instance.ClearAllEnemyBullets();
    }
}
