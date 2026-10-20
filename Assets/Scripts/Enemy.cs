using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    Rigidbody enemyRb;
    Transform player;

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        var playerGo = GameObject.Find("Player");
        if (playerGo != null)
        {
            player = playerGo.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0f;
        enemyRb.AddForce(lookDirection * speed);
    }

    void Update()
    {
        if (transform.position.y < -10f)
        {
            Destroy(gameObject);
        }
    }
}
