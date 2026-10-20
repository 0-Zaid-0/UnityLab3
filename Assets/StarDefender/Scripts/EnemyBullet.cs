using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float lifeTime = 5f;
    Vector3 direction = Vector3.back;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
