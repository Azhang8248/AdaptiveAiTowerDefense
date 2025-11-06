using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHitPoints = 2; // set per prefab
    [SerializeField] private int dollarAmount = 100;
    private int currentHitPoints;

    private bool isDestroyed = false;

    private void Awake()
    {
        maxHitPoints = newMaxHP;
        currentHitPoints = maxHitPoints;

        if (healthBar != null)
            healthBar.UpdateBar(maxHitPoints, currentHitPoints);
    }

    public void TakeDamage(int dmg)
    {
        currentHitPoints -= dmg;

        if (currentHitPoints <= 0 && !isDestroyed)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseCurrency(dollarAmount);
            isDestroyed = true;
            Destroy(gameObject);
        }

        // Destroy enemy
        Destroy(gameObject);
    }
}
