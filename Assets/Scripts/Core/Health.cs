using System;
using UnityEngine;

// HP для рослин і зомбі. При HP <= 0 об'єкт знищується.
public class Health : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 100;

    public int Current { get; private set; }
    public int Max => maxHealth;
    public bool IsDead => Current <= 0;

    // Знадобляться для ефектів у кроках 10-11 (спалах при ударі, анімація смерті, звук).
    public event Action<int> Damaged;
    public event Action Died;

    private void Awake()
    {
        Current = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        Current = Mathf.Max(0, Current - amount);
        Damaged?.Invoke(amount);

        if (IsDead)
        {
            Died?.Invoke();
            Destroy(gameObject);
        }
    }
}
