using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 编辑器一键搭建：菜单栏「弹幕游戏 / 一键搭建场景」。
/// 自动创建相机、玩家、对象池、GameManager、HUD、波次导演，并保存 Main 场景。
/// 所有视觉 Sprite 都在运行时程序化生成，场景里不留任何美术资产引用。
/// </summary>
public static class BuildSceneTool
{
    [MenuItem("弹幕游戏/一键搭建场景")]
    public static void Build()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ---------- 相机 ----------
        var camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.transform.position = new Vector3(0f, 0f, -10f);

        var cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.08f, 0.04f, 0.12f);
        camGo.AddComponent<AudioListener>();

        // ---------- 管理器 ----------
        var gmGo = new GameObject("GameManager");
        gmGo.AddComponent<GameManager>();
        gmGo.AddComponent<HUD>();
        gmGo.AddComponent<LevelDirector>();

        var poolGo = new GameObject("BulletPool");
        poolGo.AddComponent<BulletPool>();

        // ---------- 玩家 ----------
        var playerGo = new GameObject("Player");
        playerGo.transform.position = new Vector3(0f, -3f, 0f);

        var body = playerGo.AddComponent<SpriteRenderer>();
        body.sortingOrder = 12;

        var pc = playerGo.AddComponent<PlayerController>();
        pc.body = body;
        pc.maxHealth = 3;

        var hitGo = new GameObject("HitPoint");
        hitGo.transform.SetParent(playerGo.transform, false);
        var hitSr = hitGo.AddComponent<SpriteRenderer>();
        hitSr.sortingOrder = 30;
        hitGo.transform.localScale = Vector3.one * 0.35f;

        pc.hitPoint = hitGo.transform;
        pc.hitPointSr = hitSr;

        // ---------- 保存场景 ----------
        string scenesDir = Path.Combine(Application.dataPath, "Scenes");
        if (!Directory.Exists(scenesDir)) Directory.CreateDirectory(scenesDir);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/Main.unity", true) };
        AssetDatabase.Refresh();

        Debug.Log("[弹幕游戏] 场景已生成：Assets/Scenes/Main.unity，按 Play 开打。");
    }
}
