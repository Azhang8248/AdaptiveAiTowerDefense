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
        currentHitPoints = maxHitPoints; // initialize from prefab
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
    }
}
