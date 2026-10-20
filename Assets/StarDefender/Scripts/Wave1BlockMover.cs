using UnityEngine;

/// <summary>
/// Wave 1: invaders move as a block left/right, step down on edge, shoot downward randomly.
/// </summary>
public class Wave1BlockMover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float xLimit = 7.5f;
    [SerializeField] float stepDown = 0.4f;
    [SerializeField] float shootIntervalMin = 1.2f;
    [SerializeField] float shootIntervalMax = 3f;
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

        transform.position += Vector3.right * dir * moveSpeed * Time.deltaTime;

        if (transform.position.x > xLimit)
        {
            dir = -1f;
            transform.position += Vector3.back * stepDown;
        }
        else if (transform.position.x < -xLimit)
        {
            dir = 1f;
            transform.position += Vector3.back * stepDown;
        }

        if (enemyBulletPrefab != null && Time.time >= nextShoot)
        {
            nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);
            var bullet = Instantiate(enemyBulletPrefab, transform.position + Vector3.back * 0.5f, Quaternion.identity);
            var eb = bullet.GetComponent<EnemyBullet>();
            if (eb != null) eb.SetDirection(Vector3.back);
        }
    }
}
