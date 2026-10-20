using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 12f;
    public bool hasPowerup;
    public GameObject powerupIndicator;
    public float powerupDuration = 7f;

    [SerializeField] float powerupStrength = 15f;

    InputSystem_Actions controls;
    Rigidbody playerRb;
    Transform focalPoint;
    Coroutine powerupRoutine;

    // Collected powerup waits until Fire (Attack) before it activates.
    bool powerupStored;

    void Awake()
    {
        controls = new InputSystem_Actions();
        playerRb = GetComponent<Rigidbody>();
        var focal = GameObject.Find("Focal Point");
        if (focal != null)
        {
            focalPoint = focal.transform;
        }
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Attack.performed += OnFirePerformed;
    }

    void OnDisable()
    {
        controls.Player.Attack.performed -= OnFirePerformed;
        controls.Player.Disable();
    }

    void FixedUpdate()
    {
        if (focalPoint == null) return;

        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        float forwardInput = moveInput.y;
        playerRb.AddForce(focalPoint.forward * forwardInput * playerSpeed);
    }

    void Update()
    {
        if (powerupIndicator != null && (hasPowerup || powerupStored))
        {
            powerupIndicator.transform.position = transform.position + new Vector3(0f, -0.5f, 0f);
        }
    }

    void OnFirePerformed(InputAction.CallbackContext context)
    {
        if (!powerupStored || hasPowerup) return;
        ActivateStoredPowerup();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Powerup")) return;

        powerupStored = true;
        Destroy(other.gameObject);

        if (powerupIndicator != null)
        {
            powerupIndicator.SetActive(true);
        }
    }

    void ActivateStoredPowerup()
    {
        powerupStored = false;
        hasPowerup = true;

        if (powerupRoutine != null)
        {
            StopCoroutine(powerupRoutine);
        }
        powerupRoutine = StartCoroutine(PowerupCountdownRoutine());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!hasPowerup || !collision.gameObject.CompareTag("Enemy")) return;

        Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
        if (enemyRb == null) return;

        Vector3 awayFromPlayer = (collision.transform.position - transform.position).normalized;
        enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(powerupDuration);
        hasPowerup = false;
        if (powerupIndicator != null)
        {
            powerupIndicator.SetActive(false);
        }
        powerupRoutine = null;
    }
}
