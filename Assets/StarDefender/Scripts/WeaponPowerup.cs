using UnityEngine;

public class WeaponPowerup : MonoBehaviour
{
    [SerializeField] float fallSpeed = 3f;

    void Update()
    {
        transform.position += Vector3.back * fallSpeed * Time.deltaTime;
        if (transform.position.z < -12f) Destroy(gameObject);
    }
}
