using UnityEngine;
using UnityEngine.UI;

// Картка рослини в UI: клік вибирає рослину для постановки.
// Якщо Sun не вистачає - картка неактивна (сіра), вибрана картка трохи збільшена.
[RequireComponent(typeof(Button))]
public class PlantCard : MonoBehaviour
{
    [SerializeField] private PlantPlacer placer;
    [SerializeField] private SunManager sunManager;
    [Tooltip("Номер рослини в списку Plant Prefabs на PlantPlacer (0, 1, 2...).")]
    [SerializeField, Min(0)] private int plantIndex;
    [SerializeField] private float selectedScale = 1.1f;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => placer.SelectPlant(plantIndex));
    }

    private void Update()
    {
        button.interactable = sunManager.CanAfford(placer.GetPlant(plantIndex).Cost);

        bool isSelected = placer.SelectedIndex == plantIndex;
        transform.localScale = Vector3.one * (isSelected ? selectedScale : 1f);
    }
}
