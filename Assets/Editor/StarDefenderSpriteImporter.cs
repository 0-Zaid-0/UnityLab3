using UnityEditor;
using UnityEngine;

public class StarDefenderSpriteImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("StarDefender/Art") && !assetPath.Contains("StarDefender/Resources"))
            return;

        var importer = (TextureImporter)assetImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 50f;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
    }

    [MenuItem("Tools/Star Defender/Reimport Art As Sprites")]
    static void Reimport()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/StarDefender/Art" });
        foreach (var g in guids)
            AssetDatabase.ImportAsset(AssetDatabase.GUIDToAssetPath(g), ImportAssetOptions.ForceUpdate);
        Debug.Log("Reimported " + guids.Length + " textures as sprites.");
    }
}
