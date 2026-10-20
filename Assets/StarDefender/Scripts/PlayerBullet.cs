using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] float speed = 18f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] int damage = 1;

    bool powered;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        if (powered)
        {
            transform.localScale *= 1.4f;
        }
    }

    void Update()
    {
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    public void SetPowered(bool value)
    {
        powered = value;
        if (powered) damage = 2;
    }

    void OnTriggerEnter(Collider other)
    {
        var invader = other.GetComponent<InvaderHealth>();
        if (invader != null)
        {
            invader.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
