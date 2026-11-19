using UnityEngine;

/// <summary>
/// 像素风 HUD：分数、生命、操作提示、GameOver 覆盖层。
/// 用 IMGUI 绘制，不依赖 UGUI / TextMeshPro 资源，任何 Unity 版本都能跑。
/// </summary>
public class HUD : MonoBehaviour
{
    GUIStyle scoreStyle;
    GUIStyle smallStyle;
    Texture2D white;
    int lastHeight = -1;

    void EnsureStyles()
    {
        if (white == null) white = Texture2D.whiteTexture;

        if (scoreStyle == null || lastHeight != Screen.height)
        {
            lastHeight = Screen.height;
            float s = Screen.height / 720f;

            scoreStyle = new GUIStyle(GUI.skin.label);
            scoreStyle.fontSize = Mathf.RoundToInt(34 * s);
            scoreStyle.fontStyle = FontStyle.Bold;
            scoreStyle.normal.textColor = Color.white;

            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.fontSize = Mathf.RoundToInt(18 * s);
            smallStyle.normal.textColor = new Color(1f, 1f, 1f, 0.72f);
        }
    }

    void OnGUI()
    {
        EnsureStyles();
        var gm = GameManager.Instance;
        if (gm == null) return;

        float s = Screen.height / 720f;

        GUI.Label(new Rect(20 * s, 10 * s, 500 * s, 44 * s), "SCORE  " + gm.score, scoreStyle);

        float y = 62 * s;
        GUI.Label(new Rect(20 * s, y, 60 * s, 28 * s), "生命", smallStyle);
        for (int i = 0; i < gm.health; i++)
        {
            GUI.color = new Color(1f, 0.38f, 0.52f);
            GUI.DrawTexture(new Rect(72 * s + i * 24 * s, y + 4 * s, 15 * s, 15 * s), white);
        }
        GUI.color = Color.white;

        GUI.Label(new Rect(20 * s + 260 * s, y, 220 * s, 28 * s),
            "炸弹  " + gm.bombs + "  [X]", smallStyle);

        GUI.Label(new Rect(20 * s, Screen.height - 34 * s, 900 * s, 26 * s),
            "WASD / 方向键 移动    Shift 慢速精确移动    空格 / Z / J 射击    X / K 清屏炸弹", smallStyle);

        if (gm.IsGameOver)
        {
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), white);
            GUI.color = Color.white;

            var title = new GUIStyle(scoreStyle);
            title.fontSize = Mathf.RoundToInt(64 * s);
            title.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, Screen.height * 0.32f, Screen.width, 90 * s), "GAME OVER", title);

            var sub = new GUIStyle(smallStyle);
            sub.fontSize = Mathf.RoundToInt(26 * s);
            sub.alignment = TextAnchor.MiddleCenter;
            sub.normal.textColor = Color.white;
            GUI.Label(new Rect(0, Screen.height * 0.48f, Screen.width, 40 * s),
                "最终得分  " + gm.score + "    按 R 重开", sub);
        }
    }
}
