using UnityEngine;

// Стріляє горошинами вправо, коли у його ряду на екрані є зомбі.
[RequireComponent(typeof(Plant))]
public class ShooterPlant : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [Tooltip("Звідки вилітає горошина. Пусто - з центру рослини.")]
    [SerializeField] private Transform firePoint;
    [SerializeField, Min(0.1f)] private float fireInterval = 1.5f;

    private Plant plant;
    private Camera mainCamera;
    private float timer;

    private void Awake()
    {
        plant = GetComponent<Plant>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < fireInterval)
            return;

        float screenRightX = mainCamera.ViewportToWorldPoint(Vector3.right).x;
        if (!Zombie.AnyInLane(plant.Row, transform.position.x, screenRightX))
            return;

        timer = 0f;
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
    }
}
