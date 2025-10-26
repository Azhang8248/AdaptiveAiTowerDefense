using UnityEngine;

public class Slime : MonoBehaviour
{

    [Header("Settings")]
    private int splitNumber = 2;
    public GameObject slimePrefab; // Drag slime prefab (itself)
    private float minScale = .5f;    
    private float scaleReduction = .2f;
    [SerializeField] private float cloneStatsRatio = (2f / 3f); // clone stats compared to original slime
    [SerializeField] private float cloneSpeedRatio = 1.3f; // clone speed compared to original slime

    private EnemyMovement enemyMovement;
    private Health health;
    private EnemyStats stats;
    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        health = GetComponent<Health>();
        stats = GetComponent<EnemyStats>();
        if(health != null)
      {
        health.OnDeath += Split;
      }

    }

    void Split()
    {
        // Subtracting the scale for each cloned slime
        Vector2 currentScale = transform.localScale;
        Vector2 newScale = currentScale - new Vector2(scaleReduction, scaleReduction);

        // checks if it's larger than the minimum
        if (newScale.x < minScale || newScale.y < minScale) return;

        // For each split slime
        for (int i = 0; i < splitNumber; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * .2f;
            GameObject clone = Instantiate(slimePrefab, spawnPos, Quaternion.identity);

            clone.transform.localScale = newScale;

            EnemyMovement cloneMovement = clone.GetComponent<EnemyMovement>();
            EnemyStats cloneStats = clone.GetComponent<EnemyStats>();
            Health cloneHealth = clone.GetComponent<Health>();

            // Altering the clone's price/hp/movespeed by ratio
            if (cloneStats != null && stats != null)
            {
                cloneStats.price = Mathf.Max(1, Mathf.RoundToInt(stats.price * cloneStatsRatio));
                cloneStats.hp = Mathf.Max(1, stats.hp * cloneStatsRatio);
                cloneStats.moveSpeed = stats.moveSpeed * cloneSpeedRatio;

                if (cloneHealth != null)
                {
                    cloneHealth.InitializeHealth(Mathf.RoundToInt(cloneStats.hp));
                }
            }

            // continue the pathing
            if (cloneMovement != null && enemyMovement != null)
            {
                cloneMovement.SetPathIndex(enemyMovement.GetCurrentPathIndex());
            }
        }
    }
}
