using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

public static class FixStarDefenderSprites
{
    const string Primary = "Assets/StarDefender/Resources/SD";
    const string Backup = "Assets/Resources/SD";
    const string Cdn = "https://cdn.jsdelivr.net/gh/kefik/kenney/Shooter/";

    static readonly (string file, string urlPath)[] Downloads =
    {
        ("ship.png", "playerShip1_blue.png"),
        ("enemy1.png", "enemies/enemyRed1.png"),
        ("enemy2.png", "enemies/enemyRed2.png"),
        ("laserPlayer.png", "lasers/laserBlue01.png"),
        ("laserEnemy.png", "lasers/laserRed01.png"),
        ("powerup.png", "powerups/powerupYellow_bolt.png"),
        ("bg.png", "playerShip1_blue.png"), // placeholder bg if dedicated bg missing
    };

    [MenuItem("Tools/Star Defender/Fix Sprites Now")]
    static void FixSpritesNow()
    {
        EnsureFolders();
        int downloaded = DownloadMissingInto(Primary);
        DownloadMissingInto(Backup);
        MirrorFolder(Primary, Backup);

        int updated = Reimport(Primary) + Reimport(Backup);
        AssetDatabase.Refresh();

        var tex = Resources.Load<Texture2D>("SD/ship");
        var spr = Resources.Load<Sprite>("SD/ship");
        bool ok = tex != null || spr != null;
        string msg = ok
            ? "SUCCESS — SD/ship loaded.\nDownloaded: " + downloaded + "\nReimported: " + updated
            : "Still failing. Check Console.\nDownloaded: " + downloaded + "\nFolder on disk: " + DiskPath(Primary);

        if (ok) Debug.Log(msg);
        else Debug.LogError(msg);

        EditorUtility.DisplayDialog("Star Defender — Fix Sprites", msg + "\n\nThen: Build Playable Scene → Play", "OK");
    }

    static void EnsureFolders()
    {
        CreateFolder("Assets", "StarDefender");
        CreateFolder("Assets/StarDefender", "Resources");
        CreateFolder("Assets/StarDefender/Resources", "SD");
        CreateFolder("Assets", "Resources");
        CreateFolder("Assets/Resources", "SD");
    }

    static void CreateFolder(string parent, string name)
    {
        string path = parent + "/" + name;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, name);
        // Also ensure on disk
        string disk = DiskPath(path);
        if (!Directory.Exists(disk))
            Directory.CreateDirectory(disk);
    }

    static string DiskPath(string assetPath) =>
        Path.Combine(Path.GetDirectoryName(Application.dataPath)!, assetPath.Replace('/', Path.DirectorySeparatorChar));

    static int DownloadMissingInto(string folder)
    {
        int count = 0;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        foreach (var (file, urlPath) in Downloads)
        {
            string destAsset = folder + "/" + file;
            string destDisk = DiskPath(destAsset);
            Directory.CreateDirectory(Path.GetDirectoryName(destDisk)!);

            if (File.Exists(destDisk) && new FileInfo(destDisk).Length > 100)
                continue;

            string url = Cdn + urlPath;
            try
            {
                using (var wc = new WebClient())
                {
                    wc.DownloadFile(url, destDisk);
                }
                if (File.Exists(destDisk) && new FileInfo(destDisk).Length > 100)
                {
                    count++;
                    Debug.Log("Downloaded " + destAsset + " (" + new FileInfo(destDisk).Length + " bytes)");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Download failed " + url + " — " + e.Message);
            }
        }

        AssetDatabase.Refresh();
        return count;
    }

    static void MirrorFolder(string from, string to)
    {
        string fromDisk = DiskPath(from);
        string toDisk = DiskPath(to);
        if (!Directory.Exists(fromDisk)) return;
        Directory.CreateDirectory(toDisk);
        foreach (var file in Directory.GetFiles(fromDisk, "*.png"))
        {
            string dest = Path.Combine(toDisk, Path.GetFileName(file));
            File.Copy(file, dest, true);
        }
        AssetDatabase.Refresh();
    }

    static int Reimport(string folder)
    {
        if (!AssetDatabase.IsValidFolder(folder)) return 0;
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folder });
        int n = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 50f;
            importer.alphaIsTransparency = true;
            importer.isReadable = true;
            importer.mipmapEnabled = false;
            EditorUtility.SetDirty(importer);
            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            n++;
        }
        return n;
    }
}
