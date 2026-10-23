using UnityEngine;

/// <summary>
/// Moves the whole Wave 1 formation as one block (left/right, step toward player once per edge).
/// </summary>
public class Wave1FormationController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1.6f;
    [SerializeField] float xLimit = 6.5f;
    [SerializeField] float stepDown = 0.35f;
    [SerializeField] float shootIntervalMin = 1.2f;
    [SerializeField] float shootIntervalMax = 2.8f;
    [SerializeField] GameObject enemyBulletPrefab;

    float dir = 1f;
    float nextShoot;

    void Start()
    {
        nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (transform.childCount == 0) return;

        transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        for (int i = 0; i < transform.childCount; i++)
        {
            float x = transform.GetChild(i).position.x;
            if (x < minX) minX = x;
            if (x > maxX) maxX = x;
        }

        // Only reverse + step once when crossing an edge (not every frame while outside)
        if (dir > 0f && maxX > xLimit)
        {
            dir = -1f;
            transform.position += Vector3.back * stepDown;
        }
        else if (dir < 0f && minX < -xLimit)
        {
            dir = 1f;
            transform.position += Vector3.back * stepDown;
        }

        if (enemyBulletPrefab != null && Time.time >= nextShoot)
        {
            nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);
            int idx = Random.Range(0, transform.childCount);
            Transform shooter = transform.GetChild(idx);
            var bullet = Instantiate(enemyBulletPrefab, shooter.position + Vector3.back * 0.6f, Quaternion.Euler(90f, 0f, 180f));
            bullet.SetActive(true);
            var eb = bullet.GetComponent<EnemyBullet>();
            if (eb != null)
            {
                eb.enabled = true;
                eb.SetDirection(Vector3.back);
            }
        }
    }
}
