using System.Collections;
using UnityEngine;

/// <summary>
/// 一次性爆散特效：几颗像素小方块向外飞、缩小、淡出。
/// </summary>
public class ExplosionFx : MonoBehaviour
{
    public static void Play(Vector2 pos, Color color, int count = 8, float power = 3f)
    {
        var go = new GameObject("ExplosionFx");
        go.transform.position = pos;
        var fx = go.AddComponent<ExplosionFx>();
        fx.Run(color, count, power);
    }

    void Run(Color color, int count, float power)
    {
        var sprite = SpriteFactory.Square(6);

        var parts = new SpriteRenderer[count];
        var dirs = new Vector2[count];
        var speeds = new float[count];

        for (int i = 0; i < count; i++)
        {
            var p = new GameObject("p");
            p.transform.SetParent(transform, false);

            var sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = color;
            sr.sortingOrder = 25;

            float ang = Random.Range(0f, 360f);
            dirs[i] = BulletPatterns.Deg2Dir(ang);
            speeds[i] = Random.Range(power * 0.5f, power);

            p.transform.localScale = Vector3.one * Random.Range(0.14f, 0.28f);
            parts[i] = sr;
        }

        StartCoroutine(Routine(parts, dirs, speeds));
    }

    IEnumerator Routine(SpriteRenderer[] parts, Vector2[] dirs, float[] speeds)
    {
        const float duration = 0.35f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);

            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == null) continue;
                var tr = parts[i].transform;
                tr.position += (Vector3)(dirs[i] * speeds[i] * Time.deltaTime);
                tr.localScale *= 0.9f;
                var c = parts[i].color;
                c.a = 1f - k;
                parts[i].color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
