using UnityEngine;

public class StarDefenderBootstrap : MonoBehaviour
{
    public bool buildOnStart = true;

    void Start()
    {
        if (!buildOnStart) return;
        if (FindFirstObjectByType<PlayerShip>() != null) return;
        ClearPartialBuild();
        StarDefenderFactory.BuildPlayableScene();
    }

    static void ClearPartialBuild()
    {
        string[] names =
        {
            "GameSystems", "Arena", "Background", "Starfield", "Player", "Wave1_Formation", "PowerupSpawner",
            "HUD_Canvas", "PlayerBulletPrefab", "PlayerLaserPrefab", "EnemyBulletPrefab",
            "EnemyLaserPrefab", "WeaponPowerupPrefab", "EventSystem"
        };

        foreach (var n in names)
        {
            var found = GameObject.Find(n);
            while (found != null)
            {
                Destroy(found);
                found = GameObject.Find(n);
            }
        }
    }
}
