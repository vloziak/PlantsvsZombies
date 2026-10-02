using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Клік лівою кнопкою по вільній клітинці -> там з'являється вибрана рослина (якщо вистачає Sun).
// Вибір рослини: клавіші 1, 2, 3... або SelectPlant(index) з UI-кнопки.
public class PlantPlacer : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    [SerializeField] private SunManager sunManager;
    [Tooltip("Порядок = клавіші 1, 2, 3...")]
    [SerializeField] private Plant[] plantPrefabs;

    private Camera mainCamera;
    private int selectedIndex;

    public Plant SelectedPlant => plantPrefabs[selectedIndex];
    public int SelectedIndex => selectedIndex;

    public Plant GetPlant(int index) => plantPrefabs[index];

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.IsGameOver)
            return;

        HandlePlantSelectionKeys();

        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
            return;

        // Клік по кнопці UI не повинен ставити рослину під нею.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Позиція курсора на екрані (пікселі) -> позиція у світі гри (юніти).
        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);

        // Клік по сонцю - збираємо його, рослину не ставимо.
        if (TryCollectSun(worldPosition))
            return;

        if (!grid.TryGetCellAt(worldPosition, out Cell cell) || cell.IsOccupied)
            return;

        if (!sunManager.TrySpend(SelectedPlant.Cost))
            return;

        Plant plant = Instantiate(SelectedPlant, cell.transform.position, Quaternion.identity);
        plant.Place(cell);
    }

    public void SelectPlant(int index)
    {
        if (index >= 0 && index < plantPrefabs.Length)
            selectedIndex = index;
    }

    private bool TryCollectSun(Vector2 worldPosition)
    {
        foreach (Collider2D hit in Physics2D.OverlapPointAll(worldPosition))
        {
            if (hit.TryGetComponent(out SunPickup sun))
            {
                sun.Collect(sunManager);
                return true;
            }
        }
        return false;
    }

    private void HandlePlantSelectionKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        Key[] keys = { Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4, Key.Digit5 };
        for (int i = 0; i < keys.Length; i++)
        {
            if (keyboard[keys[i]].wasPressedThisFrame)
                SelectPlant(i);
        }
    }
}
