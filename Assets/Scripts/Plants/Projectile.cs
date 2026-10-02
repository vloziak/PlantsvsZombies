using UnityEngine;

// Горошина: летить вправо, при влучанні в зомбі завдає damage і зникає.
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Min(0f)] private float speed = 6f;
    [SerializeField, Min(1)] private int damage = 20;
    [Tooltip("Через скільки секунд знищити, якщо ні в кого не влучила.")]
    [SerializeField, Min(0.1f)] private float lifetime = 5f;

    private bool hasHit;

    private void Awake()
    {
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // hasHit - щоб одна горошина не вдарила двох зомбі в одному кадрі.
        if (hasHit || !other.TryGetComponent(out Zombie zombie))
            return;

        hasHit = true;
        zombie.Health.TakeDamage(damage);
        Destroy(gameObject);
    }
}
