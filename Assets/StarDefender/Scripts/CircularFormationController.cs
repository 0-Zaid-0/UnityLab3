using UnityEngine;

public class CircularFormationController : MonoBehaviour
{
    public GameObject enemyBulletPrefab;

    public float rotationSpeed = 30f;
    public float moveSpeed = 1.5f;
    public float fallSpeed = 0.25f;
    public float xPosLimit = 3.5f;
    public float shootIntervalMin = 1.5f;
    public float shootIntervalMax = 3.0f;
    public float shootingStartZPos = 7f;

    public Transform[] enemySprites;

    private int direction = 1;
    private float nextShoot;

    void Start()
    {
        nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);
    }

    void Update()
    {
        // Movement stuff
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;
        transform.position += Vector3.back * fallSpeed * Time.deltaTime;

        if (transform.position.x >= xPosLimit)
        {
            direction = -1;
        }
        else if (transform.position.x <= - xPosLimit)
        {
            direction = 1;
        }

        // Rotation stuff
        foreach (Transform sprite in enemySprites)
        {
            if (sprite != null)
            {
                sprite.rotation = Quaternion.Euler(90f, 0, 0f);
            }
        }

        // Shooting stuff
        if (transform.position.z <= shootingStartZPos && enemyBulletPrefab != null && Time.time >= nextShoot)
        {
            nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);

            int num = Random.Range(0, enemySprites.Length);
            Transform shooter = enemySprites[num];

            if (shooter != null)
            {
                var bullet = Instantiate(enemyBulletPrefab, shooter.position + Vector3.back * 0.6f, Quaternion.identity);

                bullet.SetActive(true);

                var eb = bullet.GetComponent<EnemyBullet>();

                if (eb != null)
                {
                    eb.SetDirection(Vector3.back);
                }
            }
        }
    }
}
