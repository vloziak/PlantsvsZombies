using DG.Tweening;
using UnityEngine;

// Соняшник: раз на interval "підкидає" сонце поруч із собою. Гравець збирає його кліком.
[RequireComponent(typeof(Plant))]
public class SunPlant : MonoBehaviour
{
    [SerializeField] private SunPickup sunPrefab;
    [SerializeField, Min(0.1f)] private float interval = 8f;
    [Tooltip("Куди падає сонце відносно соняшника.")]
    [SerializeField] private Vector2 dropOffset = new Vector2(0.35f, -0.4f);

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < interval)
            return;

        timer = 0f;
        SunPickup sun = Instantiate(sunPrefab, transform.position, Quaternion.identity);
        sun.JumpTo(transform.position + (Vector3)dropOffset);
        transform.DOPunchScale(Vector3.one * 0.15f, 0.3f);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
