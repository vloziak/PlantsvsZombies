using DG.Tweening;
using UnityEngine;

// Сонце на полі: з'являється (з соняшника або з неба), чекає на клік гравця,
// після кліку летить до лічильника і додає Sun. Не зібране за lifetime - зникає.
[RequireComponent(typeof(CircleCollider2D))]
public class SunPickup : MonoBehaviour
{
    [SerializeField, Min(1)] private int value = 25;
    [Tooltip("Скільки секунд сонце лежить, перш ніж зникнути.")]
    [SerializeField, Min(1f)] private float lifetime = 10f;
    [SerializeField, Min(0.1f)] private float collectDuration = 0.5f;

    private Vector3 baseScale;
    private float timer;
    private bool isCollected;

    private void Awake()
    {
        baseScale = transform.localScale;
        GetComponent<CircleCollider2D>().isTrigger = true;

        // Поява: виростає з нуля.
        transform.localScale = Vector3.zero;
        transform.DOScale(baseScale, 0.25f).SetEase(Ease.OutBack);
    }

    private void Update()
    {
        if (isCollected)
            return;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            isCollected = true;
            transform.DOKill();
            transform.DOScale(Vector3.zero, 0.3f).OnComplete(() => Destroy(gameObject));
        }
    }

    // Соняшник "підкидає" сонце, і воно падає поруч.
    public void JumpTo(Vector3 landPosition)
    {
        transform.DOJump(landPosition, 0.8f, 1, 0.6f);
    }

    // Сонце з неба повільно падає на газон.
    public void FallTo(Vector3 landPosition, float duration)
    {
        transform.DOMove(landPosition, duration).SetEase(Ease.Linear);
    }

    // Клік гравця: летить до лічильника, потім додає Sun.
    public void Collect(SunManager sunManager)
    {
        if (isCollected)
            return;

        isCollected = true;
        transform.DOKill();
        transform.DOScale(baseScale * 0.6f, collectDuration);
        transform.DOMove(sunManager.CounterWorldPosition, collectDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                sunManager.Add(value);
                Destroy(gameObject);
            });
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
