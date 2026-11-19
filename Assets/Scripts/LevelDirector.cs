using UnityEngine;

/// <summary>
/// 波次导演：按时间间隔刷出一排魔物，类型和血量随波数递增。
/// </summary>
public class LevelDirector : MonoBehaviour
{
    public float firstWaveDelay = 2f;
    public float waveInterval = 8f;

    int wave;
    float timer;

    void Start()
    {
        timer = firstWaveDelay;
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm != null && (gm.IsGameOver || gm.IsPaused)) return;
        if (BulletPool.Instance == null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        SpawnWave();
        wave++;
        timer = waveInterval;
    }

    void SpawnWave()
    {
        float halfW = PlayArea.HalfWidth;
        int count = Mathf.Min(2 + wave / 2, 6);

        for (int i = 0; i < count; i++)
        {
            float x = Mathf.Lerp(-halfW + 1.4f, halfW - 1.4f, (i + 0.5f) / count);
            x += Random.Range(-0.4f, 0.4f);

            float stopY = Random.Range(-0.6f, 2.6f);
            var pattern = (EnemyPattern)(wave % 5);
            int hp = 26 + wave * 8;
            EnemyFactory.Spawn(new Vector2(x, PlayArea.HalfHeight + 1.5f), stopY, pattern, hp);
        }
    }
}
