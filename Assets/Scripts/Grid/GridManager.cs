using UnityEngine;

// Ігрове поле: rows x columns клітинок.
// Позиція цього об'єкта = ЛІВИЙ ВЕРХНІЙ кут газону. Рухай об'єкт і міняй Cell Size,
// поки жовта сітка (видно у Scene view) не ляже на газон з картинки фону.
public class GridManager : MonoBehaviour
{
    [Header("Size")]
    [SerializeField, Min(1)] private int rows = 5;
    [SerializeField, Min(1)] private int columns = 9;
    [SerializeField] private Vector2 cellSize = new Vector2(1.4f, 1.5f);

    [Header("Look")]
    [Tooltip("Спрайт клітинки. Якщо пусто - простий квадрат.")]
    [SerializeField] private Sprite cellSprite;
    [Tooltip("Шахматка поверх фону. Альфа 0 = клітинок не видно.")]
    [SerializeField] private Color lightColor = new Color(1f, 1f, 1f, 0.12f);
    [SerializeField] private Color darkColor = new Color(0f, 0f, 0f, 0.12f);
    [SerializeField] private int sortingOrder = -50;

    private Cell[,] cells;

    public int Rows => rows;
    public int Columns => columns;
    public Vector2 CellSize => cellSize;

    private void Awake()
    {
        BuildGrid();
    }

    private void BuildGrid()
    {
        cells = new Cell[rows, columns];

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var cellObject = new GameObject($"Cell {row},{column}");
                cellObject.transform.SetParent(transform, false);
                cellObject.transform.position = GetCellCenter(row, column);

                var cell = cellObject.AddComponent<Cell>();
                Color color = (row + column) % 2 == 0 ? lightColor : darkColor;
                cell.Init(row, column, cellSize, cellSprite, color, sortingOrder);

                cells[row, column] = cell;
            }
        }
    }

    // Центр клітинки у світових координатах. Рядок 0 - верхній, колонка 0 - ліва.
    public Vector3 GetCellCenter(int row, int column)
    {
        return transform.position + new Vector3((column + 0.5f) * cellSize.x, -(row + 0.5f) * cellSize.y, 0f);
    }

    public Cell GetCell(int row, int column)
    {
        return cells[row, column];
    }

    // Яка клітинка під точкою світу (наприклад, під курсором). false - якщо точка поза полем.
    public bool TryGetCellAt(Vector2 worldPosition, out Cell cell)
    {
        Vector2 local = worldPosition - (Vector2)transform.position;
        int column = Mathf.FloorToInt(local.x / cellSize.x);
        int row = Mathf.FloorToInt(-local.y / cellSize.y);

        bool inside = row >= 0 && row < rows && column >= 0 && column < columns;
        cell = inside ? cells[row, column] : null;
        return inside;
    }

    // Малює сітку в Scene view навіть без Play - щоб підігнати її під фон.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        float width = columns * cellSize.x;
        float height = rows * cellSize.y;

        for (int column = 0; column <= columns; column++)
        {
            Vector3 top = origin + new Vector3(column * cellSize.x, 0f, 0f);
            Gizmos.DrawLine(top, top + new Vector3(0f, -height, 0f));
        }

        for (int row = 0; row <= rows; row++)
        {
            Vector3 left = origin + new Vector3(0f, -row * cellSize.y, 0f);
            Gizmos.DrawLine(left, left + new Vector3(width, 0f, 0f));
        }
    }
}
