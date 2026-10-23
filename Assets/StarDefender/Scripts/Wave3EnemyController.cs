using UnityEngine;

public class Wave3EnemyController : MonoBehaviour
{
    enum EnemyState
    {
        Waiting,
        Entering,
        Fighting
    }

    enum MovementPattern
    {
        Infinity,
        Horizontal
    }

    EnemyState state = EnemyState.Waiting;
    MovementPattern movementPattern = MovementPattern.Infinity;

    public Transform enemySprite;
    public GameObject enemyBulletPrefab;

    public float entranceZ = 5f;
    public float entranceSpeed = 3f;
    public float movementSpeed = 5f;
    public float verticalRange = 2f;
    public float horizontalRange = 5f;
    public float patternDuration = 6f;
    public float horizontalSpeed = 1f;
    public float rollSpeed = 360f;
    public float shootIntervalMin = 1.5f;
    public float shootIntervalMax = 3.0f;
    public float bulletSpreadAngle = 15f;

    private Vector3 movementCentre;
    private float movementTimer = 0f;
    private float patternTimer = 0f;
    private float rollAmount = 0f;
    private float nextShoot;
    private int horizontalDirection = 1;
    private bool isRolling = false;


    void FireBullet(Vector3 direction)
    {
        var bullet = Instantiate(enemyBulletPrefab, transform.position + Vector3.back * 0.6f, Quaternion.identity);

        bullet.SetActive(true);

        var eb = bullet.GetComponent<EnemyBullet>();

        if (eb != null)
        {
            eb.SetDirection(direction);
        }
    }

    void FireSpread()
    {
        FireBullet(Vector3.back);

        FireBullet(Quaternion.Euler(0f, -bulletSpreadAngle, 0f) * Vector3.back);

        FireBullet(Quaternion.Euler(0f, bulletSpreadAngle, 0f) * Vector3.back);
    }

    void Update()
    {
        if (state == EnemyState.Waiting)
        {  
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            if (enemies.Length == 1)
            {
                state = EnemyState.Entering;
            }
        }
        else if (state == EnemyState.Entering)
        {
            transform.position += Vector3.back * entranceSpeed * Time.deltaTime;

            if (transform.position.z <= entranceZ)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, entranceZ);

                movementCentre = transform.position;
                state = EnemyState.Fighting;
                nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);
            }
        }
        else if (state == EnemyState.Fighting)
        {
            movementTimer += movementSpeed * Time.deltaTime;

            patternTimer += Time.deltaTime;

            if (movementPattern == MovementPattern.Infinity && patternTimer >= patternDuration && transform.position.x > -0.5f && transform.position.x < 0.5f && transform.position.z > movementCentre.z - 0.5f && transform.position.z < movementCentre.z + 0.5f)
            {
                patternTimer = 0f;
                movementPattern = MovementPattern.Horizontal;
            }
            else if (movementPattern == MovementPattern.Horizontal && patternTimer >= patternDuration && transform.position.x > -0.5f && transform.position.x < 0.5f)
            {
                patternTimer = 0f;
                movementTimer = 0f;
                movementPattern = MovementPattern.Infinity;
                enemySprite.localRotation = Quaternion.Euler(90f, 0f, 0);
            }    


            if (movementPattern == MovementPattern.Infinity)
            {
                float x = Mathf.Sin(movementTimer) * horizontalRange;
                float z = Mathf.Sin(movementTimer * 2f) * verticalRange;

                transform.position = new Vector3(movementCentre.x + x, movementCentre.y, movementCentre.z + z);
            }
            else if (movementPattern == MovementPattern.Horizontal)
            {
                transform.position += Vector3.right * horizontalDirection * horizontalSpeed * Time.deltaTime;

                if (transform.position.x >= movementCentre.x + horizontalRange)
                {
                    horizontalDirection = -1;
                    isRolling = true;
                    rollAmount = 0;
                }
                else if (transform.position.x <= movementCentre.x - horizontalRange)
                {
                    horizontalDirection = 1;
                    isRolling = true;
                    rollAmount = 0f;
                }

                if (isRolling)
                {
                    float rollThisFrame = rollSpeed * Time.deltaTime;

                    enemySprite.Rotate(0f, rollThisFrame, 0f, Space.Self);

                    rollAmount += rollThisFrame;

                    if (rollAmount >= 360f)
                    {
                        isRolling = false;
                        rollAmount = 0f;

                        enemySprite.localRotation = Quaternion.Euler(90f, 0f, 0f);
                    }
                }
            }

            if (Time.time >= nextShoot)
            {
                nextShoot = Time.time + Random.Range(shootIntervalMin, shootIntervalMax);
                FireSpread();
            }
        }
    }
}




