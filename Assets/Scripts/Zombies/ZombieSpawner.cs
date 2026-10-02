using UnityEngine;

// Спавнить задану кількість зомбі по таймеру у випадковий ряд, праворуч за газоном.
public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    [SerializeField] private Zombie zombiePrefab;
    [SerializeField, Min(1)] private int totalZombies = 10;
    [Tooltip("Пауза перед першим зомбі - час поставити перші рослини.")]
    [SerializeField, Min(0f)] private float firstSpawnDelay = 10f;
    [SerializeField, Min(0.1f)] private float spawnInterval = 6f;
    [Tooltip("Наскільки правіше від газону з'являється зомбі.")]
    [SerializeField] private float spawnOffsetX = 1f;

    private float timer;
    private int spawnedCount;

    public int TotalZombies => totalZombies;
    public int SpawnedCount => spawnedCount;
    public bool IsFinished => spawnedCount >= totalZombies;

    private void Start()
    {
        timer = firstSpawnDelay;
    }

    private void Update()
    {
        if (IsFinished)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = spawnInterval;
        }
    }

    private void Spawn()
    {
        int row = Random.Range(0, grid.Rows);

        float rightEdgeX = grid.transform.position.x + grid.Columns * grid.CellSize.x;
        Vector3 position = new Vector3(rightEdgeX + spawnOffsetX, grid.GetCellCenter(row, 0).y, 0f);

        Zombie zombie = Instantiate(zombiePrefab, position, Quaternion.identity);
        zombie.Init(row);
        spawnedCount++;
    }
}
