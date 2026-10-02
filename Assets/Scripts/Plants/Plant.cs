using UnityEngine;

// Базовий компонент будь-якої рослини: ціна і клітинка, на якій вона стоїть.
public class Plant : MonoBehaviour
{
    [SerializeField, Min(0)] private int cost = 100;

    private Cell cell;

    public int Cost => cost;
    public int Row => cell != null ? cell.Row : -1;

    // Ставить рослину в центр клітинки і позначає клітинку зайнятою.
    public void Place(Cell targetCell)
    {
        cell = targetCell;
        cell.SetOccupant(gameObject);
        transform.position = cell.transform.position;

        // Нижні ряди малюються поверх верхніх.
        GetComponentInChildren<SpriteRenderer>().sortingOrder = cell.Row;
    }

    // Рослину знищили (зомбі з'їв) - клітинка знову вільна.
    private void OnDestroy()
    {
        if (cell != null)
            cell.Clear();
    }
}
