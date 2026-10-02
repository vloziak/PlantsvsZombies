using System;
using TMPro;
using UnityEngine;

// Ресурс Sun: стартовий запас + автоматична генерація.
// Якщо задано Sky Sun Prefab - сонце падає з неба і його треба зібрати кліком,
// інакше Sun просто додається раз на tickInterval.
public class SunManager : MonoBehaviour
{
    [SerializeField, Min(0)] private int startSun = 150;
    [SerializeField, Min(0)] private int sunPerTick = 25;
    [SerializeField, Min(0.1f)] private float tickInterval = 5f;

    [Header("Sky sun")]
    [SerializeField] private SunPickup skySunPrefab;
    [Tooltip("Потрібен, щоб сонце падало на випадкову клітинку газону.")]
    [SerializeField] private GridManager grid;
    [SerializeField, Min(0.1f)] private float skyFallDuration = 4f;

    [Header("UI")]
    [Tooltip("Текст лічильника (необов'язково).")]
    [SerializeField] private TMP_Text sunText;
    [Tooltip("Іконка лічильника - сюди летять зібрані сонця.")]
    [SerializeField] private RectTransform counterIcon;

    private Camera mainCamera;
    private float timer;

    public int Current { get; private set; }

    // Позиція іконки лічильника у світі гри (UI живе в пікселях екрана).
    public Vector3 CounterWorldPosition
    {
        get
        {
            Vector3 screenPosition = counterIcon != null
                ? counterIcon.position
                : new Vector3(0f, Screen.height, 0f);
            Vector3 world = mainCamera.ScreenToWorldPoint(screenPosition);
            world.z = 0f;
            return world;
        }
    }

    public event Action<int> Changed;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        SetSun(startSun);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < tickInterval)
            return;

        timer = 0f;
        if (skySunPrefab != null && grid != null)
            DropSunFromSky();
        else
            Add(sunPerTick);
    }

    public bool CanAfford(int amount)
    {
        return Current >= amount;
    }

    // Віднімає Sun, якщо його вистачає. false - не вистачає, нічого не змінюється.
    public bool TrySpend(int amount)
    {
        if (!CanAfford(amount))
            return false;

        SetSun(Current - amount);
        return true;
    }

    public void Add(int amount)
    {
        SetSun(Current + amount);
    }

    private void DropSunFromSky()
    {
        Vector3 land = grid.GetCellCenter(UnityEngine.Random.Range(0, grid.Rows), UnityEngine.Random.Range(0, grid.Columns));
        float skyY = mainCamera.transform.position.y + mainCamera.orthographicSize + 1f;

        SunPickup sun = Instantiate(skySunPrefab, new Vector3(land.x, skyY, 0f), Quaternion.identity);
        sun.FallTo(land, skyFallDuration);
    }

    private void SetSun(int value)
    {
        Current = value;
        if (sunText != null)
            sunText.text = Current.ToString();
        Changed?.Invoke(Current);
    }
}
