using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StarDefenderSetup
{
    const string ScenePath = "Assets/StarDefender/Scenes/StarDefender.unity";

    [MenuItem("Tools/Star Defender/Build Playable Scene")]
    public static void BuildScene()
    {
        EnsureTags();
        EnsureFolders();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // DefaultGameObjects already adds Main Camera + Light — configure camera for top-down
        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 10f;
            cam.backgroundColor = new Color(0.05f, 0.07f, 0.12f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.transform.position = new Vector3(0f, 18f, 0f);
            cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }

        var bootstrap = new GameObject("Bootstrap");
        var boot = bootstrap.AddComponent<StarDefenderBootstrap>();
        boot.buildOnStart = true;
        EditorUtility.SetDirty(boot);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AddToBuildSettings(ScenePath);

        // Make sure this scene is the active one
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        EditorUtility.DisplayDialog(
            "Star Defender",
            "Scene is ready and open.\n\n" +
            "1) Click OK\n" +
            "2) Press the Play button at the top\n\n" +
            "Controls: A/D move, Space fire",
            "OK");
    }

    [MenuItem("Tools/Star Defender/Open Scene")]
    public static void OpenScene()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
        {
            BuildScene();
            return;
        }

        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/StarDefender"))
            AssetDatabase.CreateFolder("Assets", "StarDefender");
        if (!AssetDatabase.IsValidFolder("Assets/StarDefender/Scenes"))
            AssetDatabase.CreateFolder("Assets/StarDefender", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/StarDefender/Scripts"))
            AssetDatabase.CreateFolder("Assets/StarDefender", "Scripts");
    }

    static void EnsureTags()
    {
        AddTag("Enemy");
        AddTag("EnemyBullet");
        AddTag("WeaponPowerup");
        AddTag("Powerup");
    }

    static void AddTag(string tag)
    {
        var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (assets == null || assets.Length == 0) return;
        var so = new SerializedObject(assets[0]);
        var tags = so.FindProperty("tags");
        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag) return;
        }
        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        so.ApplyModifiedProperties();
    }

    static void AddToBuildSettings(string path)
    {
        var list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (var s in list)
        {
            if (s.path == path) return;
        }
        list.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = list.ToArray();
    }
}
