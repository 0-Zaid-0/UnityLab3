using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Menu: Tools > Prototype 4 > Rebuild Scene (Proper Setup)
/// Rebuilds prefabs + scene wiring to match Unit 4 look and playable feel.
/// </summary>
public static class Prototype4AutoSetup
{
    const string ScenePath = "Assets/Scenes/Prototype 4.unity";
    const string EnemyPrefabPath = "Assets/Prefabs/Enemy.prefab";
    const string PowerupPrefabPath = "Assets/Prefabs/Powerup.prefab";
    const string BouncyPath = "Assets/Physics Materials/Bouncy.physicMaterial";

    [MenuItem("Tools/Prototype 4/Rebuild Scene (Proper Setup)")]
    public static void SetupFromMenu()
    {
        EnsureFolders();
        EnsureTags();
        var bouncy = EnsureBouncy();
        var enemy = BuildEnemyPrefab(bouncy);
        var powerup = BuildPowerupPrefab();
        WireScene(enemy, powerup, bouncy);
        Debug.Log("Prototype 4 rebuilt. Stop Play if running, then press Play.");
    }

    static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        if (!AssetDatabase.IsValidFolder("Assets/Physics Materials"))
            AssetDatabase.CreateFolder("Assets", "Physics Materials");
    }

    static void EnsureTags()
    {
        AddTag("Enemy");
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

    static PhysicsMaterial EnsureBouncy()
    {
        var existing = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(BouncyPath);
        if (existing != null) return existing;

        var mat = new PhysicsMaterial("Bouncy")
        {
            bounciness = 1f,
            bounceCombine = PhysicsMaterialCombine.Multiply,
            dynamicFriction = 0.4f,
            staticFriction = 0.4f
        };
        AssetDatabase.CreateAsset(mat, BouncyPath);
        return mat;
    }

    static Material LoadMat(params string[] paths)
    {
        foreach (var p in paths)
        {
            var m = AssetDatabase.LoadAssetAtPath<Material>(p);
            if (m != null) return m;
        }
        return null;
    }

    static GameObject BuildEnemyPrefab(PhysicsMaterial bouncy)
    {
        var enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.name = "Enemy";
        enemy.tag = "Enemy";
        enemy.transform.localScale = Vector3.one * 1.5f;

        var rb = enemy.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = 0.05f;

        var col = enemy.GetComponent<SphereCollider>();
        col.sharedMaterial = bouncy;

        var mat = LoadMat(
            "Assets/Course Library/Materials/PolygonPrototype_Texture_Grid_07.mat",
            "Assets/Course Library/_Source_Files/Materials/PolygonPrototype_Texture_01.mat");
        if (mat != null)
            enemy.GetComponent<MeshRenderer>().sharedMaterial = mat;

        var enemyScript = enemy.AddComponent<Enemy>();
        var so = new SerializedObject(enemyScript);
        so.FindProperty("speed").floatValue = 2f;
        so.ApplyModifiedPropertiesWithoutUndo();

        var prefab = PrefabUtility.SaveAsPrefabAsset(enemy, EnemyPrefabPath);
        Object.DestroyImmediate(enemy);
        return prefab;
    }

    static GameObject BuildPowerupPrefab()
    {
        var source = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Course Library/Pickups/Star_01.prefab");
        GameObject powerup = source != null
            ? (GameObject)PrefabUtility.InstantiatePrefab(source)
            : GameObject.CreatePrimitive(PrimitiveType.Cube);

        powerup.name = "Powerup";
        powerup.tag = "Powerup";
        powerup.transform.localScale = Vector3.one * 2f;

        var box = powerup.GetComponent<BoxCollider>();
        if (box == null) box = powerup.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = Vector3.one;

        if (powerup.GetComponent<RotatePowerup>() == null)
            powerup.AddComponent<RotatePowerup>();

        var prefab = PrefabUtility.SaveAsPrefabAsset(powerup, PowerupPrefabPath);
        Object.DestroyImmediate(powerup);
        return prefab;
    }

    static void WireScene(GameObject enemyPrefab, GameObject powerupPrefab, PhysicsMaterial bouncy)
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("Player missing from Prototype 4 scene.");
            return;
        }

        player.tag = "Player";
        var playerCol = player.GetComponent<SphereCollider>();
        if (playerCol != null) playerCol.sharedMaterial = bouncy;

        var playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.interpolation = RigidbodyInterpolation.Interpolate;
            playerRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            playerRb.linearDamping = 0.05f;
        }

        var playerMat = LoadMat(
            "Assets/Course Library/Materials/PolygonPrototype_Texture_Grid_Glass_01.mat");
        if (playerMat != null)
            player.GetComponent<MeshRenderer>().sharedMaterial = playerMat;

        var pc = player.GetComponent<PlayerController>();
        if (pc == null) pc = player.AddComponent<PlayerController>();
        pc.playerSpeed = 12f;
        pc.powerupDuration = 7f;

        var focal = GameObject.Find("Focal Point");
        if (focal != null)
        {
            var rot = focal.GetComponent<RotateCamera>();
            if (rot == null) rot = focal.AddComponent<RotateCamera>();
            rot.rotationSpeed = 50f;

            var cam = focal.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.orthographic = false;
                cam.fieldOfView = 60f;
                cam.transform.localPosition = new Vector3(0f, 12f, -18f);
                cam.transform.localEulerAngles = new Vector3(30f, 0f, 0f);
            }
        }

        // Powerup Indicator from SelectionRing_06
        var indicator = GameObject.Find("Powerup Indicator");
        if (indicator != null) Object.DestroyImmediate(indicator);

        var ringSource = AssetDatabase.LoadAssetAtPath<GameObject>(
            "Assets/Course Library/Powerup Indicators/SelectionRing_06.prefab");
        if (ringSource != null)
        {
            indicator = (GameObject)PrefabUtility.InstantiatePrefab(ringSource);
            indicator.name = "Powerup Indicator";
        }
        else
        {
            indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            indicator.name = "Powerup Indicator";
            Object.DestroyImmediate(indicator.GetComponent<Collider>());
        }

        indicator.transform.localScale = Vector3.one * 3f;
        indicator.transform.position = player.transform.position + new Vector3(0f, -0.5f, 0f);
        indicator.SetActive(false);
        pc.powerupIndicator = indicator;

        var spawnGo = GameObject.Find("Spawn Manager");
        if (spawnGo == null) spawnGo = new GameObject("Spawn Manager");
        var spawn = spawnGo.GetComponent<SpawnManager>();
        if (spawn == null) spawn = spawnGo.AddComponent<SpawnManager>();
        spawn.enemyPrefab = enemyPrefab;
        spawn.powerupPrefab = powerupPrefab;
        spawn.waveNumber = 1;
        spawn.delayBetweenWaves = 2f;
        spawn.maxEnemiesPerWave = 8;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
    }
}
