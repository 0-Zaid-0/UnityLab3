using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] GameObject powerupPrefab;
    [SerializeField] float intervalMin = 8f;
    [SerializeField] float intervalMax = 14f;
    [SerializeField] float xRange = 7f;
    [SerializeField] float spawnZ = 6f;

    float nextSpawn;

    public void SetPowerupPrefab(GameObject prefab)
    {
        powerupPrefab = prefab;
    }

    void Start()
    {
        Schedule();
    }

    void Update()
    {
        if (powerupPrefab == null) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (Time.time >= nextSpawn)
        {
            Schedule();
            Vector3 pos = new Vector3(Random.Range(-xRange, xRange), 0.5f, spawnZ);
            Instantiate(powerupPrefab, pos, Quaternion.Euler(90f, 0f, 0f));
        }
    }

    void Schedule()
    {
        nextSpawn = Time.time + Random.Range(intervalMin, intervalMax);
    }
}
