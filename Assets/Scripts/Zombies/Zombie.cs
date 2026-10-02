using System.Collections.Generic;
using UnityEngine;

// Йде справа наліво по своєму ряду. Наткнувся на рослину - зупиняється і кусає її.
[RequireComponent(typeof(Health), typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Zombie : MonoBehaviour
{
    private static readonly List<Zombie> active = new List<Zombie>();

    [SerializeField, Min(0f)] private float speed = 0.4f;
    [SerializeField, Min(1)] private int attackDamage = 20;
    [SerializeField, Min(0.1f)] private float attackInterval = 1f;

    private Health target;
    private float attackTimer;

    // Усі живі зомбі на сцені (для перевірки Win/Game Over і для стрільби рослин).
    public static IReadOnlyList<Zombie> Active => active;

    public Health Health { get; private set; }
    public int Row { get; private set; } = -1;

    private void Awake()
    {
        Health = GetComponent<Health>();

        // Dynamic без гравітації - щоб спрацьовували тригери і з рослинами, і з горошинами.
        var body = GetComponent<Rigidbody2D>();
        body.bodyType = RigidbodyType2D.Dynamic;
        body.gravityScale = 0f;
        body.freezeRotation = true;
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnEnable() => active.Add(this);
    private void OnDisable() => active.Remove(this);

    // Викликає спавнер одразу після створення.
    public void Init(int row)
    {
        Row = row;
        GetComponentInChildren<SpriteRenderer>().sortingOrder = 10 + row;
    }

    private void Update()
    {
        // target стає null сам, коли рослину знищено - тоді йдемо далі.
        if (target != null)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                target.TakeDamage(attackDamage);
            }
            return;
        }

        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (target != null || !other.TryGetComponent(out Plant plant) || plant.Row != Row)
            return;

        target = plant.GetComponent<Health>();
        attackTimer = 0f;
    }

    // Чи є зомбі в ряду row між minX і maxX.
    public static bool AnyInLane(int row, float minX, float maxX)
    {
        foreach (Zombie zombie in active)
        {
            float x = zombie.transform.position.x;
            if (zombie.Row == row && x > minX && x < maxX)
                return true;
        }
        return false;
    }
}
