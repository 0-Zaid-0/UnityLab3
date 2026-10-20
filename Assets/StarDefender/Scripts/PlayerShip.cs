using System.Collections;
using UnityEngine;

/// <summary>
/// Player ship: horizontal movement clamped to screen, shooting via Fire event,
/// lives/respawn with 2s invulnerability flash, weapon powerup for 10 seconds.
/// </summary>
public class PlayerShip : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 12f;
    [SerializeField] float xMin = -8f;
    [SerializeField] float xMax = 8f;

    [Header("Combat")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireCooldown = 0.25f;
    [SerializeField] float poweredFireCooldown = 0.1f;

    [Header("Lives / Respawn")]
    [SerializeField] float invulnDuration = 2f;
    [SerializeField] Vector3 spawnPosition = new Vector3(0f, 0.5f, -7f);

    [Header("Powerup")]
    [SerializeField] float powerupDuration = 10f;
    [SerializeField] GameObject powerupVisual;

    Rigidbody rb;
    Renderer[] renderers;
    float nextFireTime;
    bool invulnerable;
    bool weaponUpgraded;
    Coroutine powerupRoutine;
    Vector3 startPos;

    public bool IsInvulnerable => invulnerable;
    public bool HasWeaponUpgrade => weaponUpgraded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        renderers = GetComponentsInChildren<Renderer>(true);
        startPos = spawnPosition;
        if (firePoint == null)
        {
            var fp = new GameObject("FirePoint");
            fp.transform.SetParent(transform);
            fp.transform.localPosition = new Vector3(0f, 0f, 0.8f);
            firePoint = fp.transform;
        }
    }

    void OnEnable()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnMove += HandleMove;
            GameInput.Instance.OnFirePressed += HandleFire;
        }
    }

    void Start()
    {
        // Subscribe again in case GameInput awoke after this object
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnMove -= HandleMove;
            GameInput.Instance.OnFirePressed -= HandleFire;
            GameInput.Instance.OnMove += HandleMove;
            GameInput.Instance.OnFirePressed += HandleFire;
        }

        transform.position = startPos;
        if (powerupVisual != null) powerupVisual.SetActive(false);
    }

    void OnDisable()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnMove -= HandleMove;
            GameInput.Instance.OnFirePressed -= HandleFire;
        }
    }

    void HandleMove(float horizontal)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        Vector3 pos = transform.position;
        pos.x += horizontal * moveSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = startPos.y;
        pos.z = startPos.z;
        transform.position = pos;
    }

    void HandleFire()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
        if (Time.time < nextFireTime) return;
        if (bulletPrefab == null || firePoint == null) return;

        float cooldown = weaponUpgraded ? poweredFireCooldown : fireCooldown;
        nextFireTime = Time.time + cooldown;

        var bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(90f, 0f, 0f));
        bullet.SetActive(true);
        var pb = bullet.GetComponent<PlayerBullet>();
        if (pb != null)
        {
            pb.enabled = true;
            pb.SetPowered(weaponUpgraded);
        }

        AudioHub.PlayShoot();
    }

    public void TakeHit()
    {
        if (invulnerable) return;
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.IsGameOver) return;

        AudioHub.PlayLoseLife();
        GameManager.Instance.LoseLife();

        if (GameManager.Instance.IsGameOver)
        {
            gameObject.SetActive(false);
            return;
        }

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        invulnerable = true;
        transform.position = startPos;

        float end = Time.time + invulnDuration;
        while (Time.time < end)
        {
            SetVisible(Time.time % 0.2f > 0.1f);
            yield return null;
        }

        SetVisible(true);
        invulnerable = false;
    }

    void SetVisible(bool visible)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = visible;
        }
    }

    public void CollectWeaponPowerup()
    {
        if (powerupRoutine != null) StopCoroutine(powerupRoutine);
        powerupRoutine = StartCoroutine(WeaponPowerupRoutine());
    }

    IEnumerator WeaponPowerupRoutine()
    {
        weaponUpgraded = true;
        if (powerupVisual != null) powerupVisual.SetActive(true);
        yield return new WaitForSeconds(powerupDuration);
        weaponUpgraded = false;
        if (powerupVisual != null) powerupVisual.SetActive(false);
        powerupRoutine = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyBullet") || other.CompareTag("Enemy"))
        {
            TakeHit();
            if (other.CompareTag("EnemyBullet"))
            {
                Destroy(other.gameObject);
            }
        }
        else if (other.CompareTag("WeaponPowerup"))
        {
            CollectWeaponPowerup();
            Destroy(other.gameObject);
        }
    }
}
