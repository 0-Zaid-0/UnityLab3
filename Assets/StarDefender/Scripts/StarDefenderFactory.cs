using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

/// <summary>
/// Builds Star Defender using Kenney textures on quads (works even if not imported as Sprite).
/// </summary>
public static class StarDefenderFactory
{
    public static void BuildPlayableScene()
    {
        if (Object.FindFirstObjectByType<PlayerShip>() != null) return;

        SetupCamera();
        SetupLight();

        var systems = new GameObject("GameSystems");
        systems.AddComponent<GameManager>();
        systems.AddComponent<GameInput>();
        systems.AddComponent<AudioHub>();
        systems.AddComponent<HudController>();

        var shipTex = LoadTex("SD/ship");
        Debug.Log(shipTex != null
            ? "Star Defender: Kenney textures loaded OK."
            : "Star Defender WARNING: SD/ship texture missing from Resources.");

        CreateStarfield();
        var playerLaser = CreateLaserPrefab(true);
        var enemyLaser = CreateLaserPrefab(false);
        var powerup = CreatePowerupPrefab();
        CreatePlayer(playerLaser);
        CreateWave1(enemyLaser);

        var spawner = new GameObject("PowerupSpawner");
        spawner.AddComponent<PowerupSpawner>().SetPowerupPrefab(powerup);
        EnsureEventSystem();
    }

    static void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            var go = new GameObject("Main Camera");
            cam = go.AddComponent<Camera>();
            go.AddComponent<AudioListener>();
            go.tag = "MainCamera";
        }
        else if (cam.GetComponent<AudioListener>() == null)
            cam.gameObject.AddComponent<AudioListener>();

        cam.transform.SetPositionAndRotation(new Vector3(0f, 20f, 0f), Quaternion.Euler(90f, 0f, 0f));
        cam.orthographic = true;
        cam.orthographicSize = 10f;
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.06f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.enabled = true;
    }

    static void SetupLight()
    {
        var light = Object.FindFirstObjectByType<Light>();
        if (light == null)
        {
            var go = new GameObject("Directional Light");
            light = go.AddComponent<Light>();
            light.type = LightType.Directional;
        }
        light.intensity = 1.5f;
        light.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    static void CreateStarfield()
    {
        // Remove old bad background if present
        var old = GameObject.Find("Background");
        if (old != null) Object.Destroy(old);

        var root = new GameObject("Starfield");
        root.transform.position = new Vector3(0f, -1f, 0f);

        // Soft dark plane (no ship texture)
        var plane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        plane.name = "SpacePlane";
        plane.transform.SetParent(root.transform, false);
        plane.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        plane.transform.localScale = new Vector3(30f, 30f, 1f);
        Object.Destroy(plane.GetComponent<Collider>());
        TintRenderer(plane, new Color(0.04f, 0.04f, 0.1f));

        // Simple stars
        for (int i = 0; i < 60; i++)
        {
            var star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "Star";
            star.transform.SetParent(root.transform, false);
            float x = Random.Range(-12f, 12f);
            float z = Random.Range(-10f, 10f);
            float s = Random.Range(0.04f, 0.12f);
            star.transform.localPosition = new Vector3(x, 0.02f, z);
            star.transform.localScale = Vector3.one * s;
            Object.Destroy(star.GetComponent<Collider>());
            float bright = Random.Range(0.6f, 1f);
            TintRenderer(star, new Color(bright, bright, bright * 0.95f));
        }
    }

    static void TintRenderer(GameObject go, Color color)
    {
        var r = go.GetComponent<Renderer>();
        if (r == null) return;
        var shader =
            Shader.Find("Universal Render Pipeline/Unlit") ??
            Shader.Find("Unlit/Color") ??
            Shader.Find("Sprites/Default") ??
            Shader.Find("Standard");
        if (shader == null) return;
        var mat = new Material(shader);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        mat.color = color;
        r.material = mat;
    }

    static void CreatePlayer(GameObject laserPrefab)
    {
        var root = new GameObject("Player");
        root.tag = "Player";
        root.transform.position = new Vector3(0f, 0.5f, -7f);

        var ship = TexturedQuad("Ship", "SD/ship", 1.8f, 1.8f, new Color(0.25f, 0.75f, 1f));
        ship.transform.SetParent(root.transform, false);
        ship.transform.localPosition = Vector3.zero;
        ship.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        var glow = TexturedQuad("PowerGlow", "SD/powerup", 1f, 1f, Color.yellow);
        glow.transform.SetParent(root.transform, false);
        glow.transform.localPosition = new Vector3(0f, 0.05f, 0f);
        glow.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        glow.SetActive(false);

        var col = root.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(1.6f, 0.6f, 1.6f);

        var rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var ps = root.AddComponent<PlayerShip>();
        SetField(ps, "bulletPrefab", laserPrefab);
        SetField(ps, "powerupVisual", glow);

        var fp = new GameObject("FirePoint");
        fp.transform.SetParent(root.transform);
        fp.transform.localPosition = new Vector3(0f, 0f, 1f);
        SetField(ps, "firePoint", fp.transform);
    }

    static void CreateWave1(GameObject enemyLaser)
    {
        var formation = new GameObject("Wave1_Formation");
        formation.transform.position = new Vector3(0f, 0.5f, 5f);

        string[] tex = { "SD/enemy1", "SD/enemy2", "SD/enemy1" };
        Color[] fb = { new Color(1f, 0.25f, 0.25f), new Color(1f, 0.4f, 0.15f), new Color(0.95f, 0.2f, 0.35f) };
        int cols = 6, rows = 3;
        float sx = 1.8f, sz = 1.5f;
        float startX = -((cols - 1) * sx) * 0.5f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var inv = new GameObject("Invader");
                inv.tag = "Enemy";
                inv.transform.SetParent(formation.transform, false);
                inv.transform.localPosition = new Vector3(startX + c * sx, 0f, -r * sz);

                var vis = TexturedQuad("Vis", tex[r % tex.Length], 1.4f, 1.4f, fb[r % fb.Length]);
                vis.transform.SetParent(inv.transform, false);
                vis.transform.localRotation = Quaternion.Euler(90f, 180f, 0f);

                var col = inv.AddComponent<BoxCollider>();
                col.isTrigger = true;
                col.size = new Vector3(1.2f, 0.5f, 1.2f);
                inv.AddComponent<InvaderHealth>();
            }
        }

        var block = formation.AddComponent<Wave1FormationController>();
        SetField(block, "enemyBulletPrefab", enemyLaser);
        SetField(block, "moveSpeed", 1.5f);
        SetField(block, "xLimit", 6.5f);
        SetField(block, "stepDown", 0.4f);
    }

    static GameObject CreateLaserPrefab(bool player)
    {
        string path = player ? "SD/laserPlayer" : "SD/laserEnemy";
        Color fb = player ? new Color(0.3f, 1f, 0.55f) : new Color(1f, 0.35f, 0.1f);
        var go = TexturedQuad(player ? "PlayerLaserPrefab" : "EnemyLaserPrefab", path, 0.35f, 1.1f, fb);
        go.transform.position = new Vector3(0f, -50f, 0f);
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        var col = go.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = new Vector3(0.4f, 0.4f, 1.2f);

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (player)
        {
            var pb = go.AddComponent<PlayerBullet>();
            pb.enabled = false;
        }
        else
        {
            go.tag = "EnemyBullet";
            go.AddComponent<EnemyBullet>();
        }
        return go;
    }

    static GameObject CreatePowerupPrefab()
    {
        var go = TexturedQuad("WeaponPowerupPrefab", "SD/powerup", 1.1f, 1.1f, Color.yellow);
        go.tag = "WeaponPowerup";
        go.transform.position = new Vector3(-2f, -50f, 0f);
        go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        var col = go.AddComponent<BoxCollider>();
        col.isTrigger = true;
        col.size = Vector3.one;
        go.AddComponent<WeaponPowerup>();
        return go;
    }

    static GameObject TexturedQuad(string name, string resourcesPath, float width, float height, Color fallback)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = name;
        Object.Destroy(go.GetComponent<Collider>());
        go.transform.localScale = new Vector3(width, height, 1f);

        var tex = LoadTex(resourcesPath);
        var r = go.GetComponent<Renderer>();
        var mat = MakeTransparentMat(tex, fallback);
        r.material = mat;
        return go;
    }

    static Material MakeTransparentMat(Texture2D tex, Color fallback)
    {
        Shader shader =
            Shader.Find("Universal Render Pipeline/Unlit") ??
            Shader.Find("Unlit/Transparent") ??
            Shader.Find("Sprites/Default") ??
            Shader.Find("Unlit/Texture") ??
            Shader.Find("Universal Render Pipeline/Lit") ??
            Shader.Find("Standard");

        Material mat;
        if (shader != null)
            mat = new Material(shader);
        else
        {
            // Last resort: clone whatever Unity put on the quad
            mat = new Material(Shader.Find("Hidden/InternalErrorShader") == null
                ? Shader.Find("Standard")
                : Shader.Find("Standard"));
        }

        if (tex != null)
        {
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
            mat.mainTexture = tex;
            Color white = Color.white;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", white);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", white);
            mat.color = white;

            // Alpha blend for PNG transparency
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_ZWrite", 0f);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = 3000;
        }
        else
        {
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", fallback);
            if (mat.HasProperty("_Color")) mat.SetColor("_Color", fallback);
            mat.color = fallback;
        }

        return mat;
    }

    static Texture2D LoadTex(string resourcesPath)
    {
        var tex = Resources.Load<Texture2D>(resourcesPath);
        if (tex != null)
            return tex;

        var sprite = Resources.Load<Sprite>(resourcesPath);
        if (sprite != null)
            return sprite.texture;

        Debug.LogWarning("Star Defender: Resources.Load failed for '" + resourcesPath + "'. Listing Resources/SD folders under Application.dataPath.");
        LogResourcesSdListing();
        return null;
    }

    static void LogResourcesSdListing()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Application.dataPath = " + Application.dataPath);

        foreach (string relative in new[]
                 {
                     "StarDefender/Resources/SD",
                     "Resources/SD",
                 })
        {
            string full = Path.Combine(Application.dataPath, relative);
            sb.AppendLine(relative + " exists=" + Directory.Exists(full));
            if (!Directory.Exists(full))
                continue;

            foreach (string file in Directory.GetFiles(full))
                sb.AppendLine("  " + Path.GetFileName(file) + " (" + new FileInfo(file).Length + " bytes)");
        }

        Debug.LogWarning(sb.ToString());
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<InputSystemUIInputModule>();
    }

    static void SetField(Component target, string field, object value)
    {
        if (target == null) return;
        var f = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (f != null) f.SetValue(target, value);
    }
}
