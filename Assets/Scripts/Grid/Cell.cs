using UnityEngine;

// Одна клітинка поля: знає свій рядок/колонку і чи на ній щось стоїть.
public class Cell : MonoBehaviour
{
    private static Sprite defaultSprite;

    private GameObject occupant;

    public int Row { get; private set; }
    public int Column { get; private set; }
    public bool IsOccupied => occupant != null;

    public void Init(int row, int column, Vector2 size, Sprite sprite, Color color, int sortingOrder)
    {
        Row = row;
        Column = column;

        // Колайдер потрібен, щоб пізніше ловити кліки по клітинці.
        var boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = size;

        // Картинка - в окремому дочірньому об'єкті, щоб її scale не впливав на рослину в клітинці.
        var visual = new GameObject("Visual");
        visual.transform.SetParent(transform, false);

        var spriteRenderer = visual.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite != null ? sprite : GetDefaultSprite();
        spriteRenderer.color = color;
        spriteRenderer.sortingOrder = sortingOrder;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        visual.transform.localScale = new Vector3(size.x / spriteSize.x, size.y / spriteSize.y, 1f);
    }

    public void SetOccupant(GameObject newOccupant)
    {
        occupant = newOccupant;
    }

    public void Clear()
    {
        occupant = null;
    }

    // Білий квадрат 1x1 юніт, якщо свій спрайт не задано.
    private static Sprite GetDefaultSprite()
    {
        if (defaultSprite == null)
        {
            Texture2D texture = Texture2D.whiteTexture;
            defaultSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width);
        }
        return defaultSprite;
    }
}
