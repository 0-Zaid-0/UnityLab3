using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject powerupPrefab;
    public int enemyCount;
    public int waveNumber = 1;
    public float delayBetweenWaves = 2f;
    public int maxEnemiesPerWave = 8;

    [SerializeField] float spawnRange = 9f;
    [SerializeField] float spawnHeight = 0.6f;

    bool spawning;

    void Start()
    {
        StartCoroutine(SpawnWaveRoutine(0.75f));
    }

    void Update()
    {
        enemyCount = FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length;

        if (enemyCount == 0 && !spawning)
        {
            waveNumber++;
            StartCoroutine(SpawnWaveRoutine(delayBetweenWaves));
        }
    }

    IEnumerator SpawnWaveRoutine(float delay)
    {
        spawning = true;
        yield return new WaitForSeconds(delay);

        int count = Mathf.Min(waveNumber, maxEnemiesPerWave);
        SpawnEnemyWave(count);
        Instantiate(powerupPrefab, GenerateSpawnPosition(), powerupPrefab.transform.rotation);

        // Give physics a moment so new enemies are counted before allowing the next wave
        yield return new WaitForSeconds(0.25f);
        spawning = false;
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation);
        }
    }

    Vector3 GenerateSpawnPosition()
    {
        float x = Random.Range(-spawnRange, spawnRange);
        float z = Random.Range(-spawnRange, spawnRange);
        return new Vector3(x, spawnHeight, z);
    }
}
