using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(EnemyStats))]
[RequireComponent(typeof(Health))]
public class Slime : MonoBehaviour
{
    [Header("Split Settings")]
    [SerializeField] private int splitNumber = 2;
    [SerializeField] private GameObject slimePrefab;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float scaleReduction = 0.2f;
    [SerializeField] private float cloneStatsRatio = 2f / 3f;
    [SerializeField] private float cloneSpeedRatio = 1.3f;

    private EnemyMovement enemyMovement;
    private Health health;
    private EnemyStats stats;
    private bool hasSplit = false; // prevent double-splitting

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        health = GetComponent<Health>();
        stats = GetComponent<EnemyStats>();

        if (health != null)
            health.OnDeath += Split; // directly bind Split again
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= Split;
    }

    private void Split()
    {
        // Avoid multiple triggers
        if (hasSplit) return;
        hasSplit = true;

        Vector2 currentScale = transform.localScale;
        Vector2 newScale = currentScale - new Vector2(scaleReduction, scaleReduction);

        if (newScale.x < minScale || newScale.y < minScale)
            return;

        for (int i = 0; i < splitNumber; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 0.3f;
            GameObject clone = Instantiate(slimePrefab, spawnPos, Quaternion.identity);
            clone.transform.localScale = newScale;

            // Set inactive temporarily so Start() doesn’t run with wrong values
            clone.SetActive(false);

            EnemyMovement cloneMovement = clone.GetComponent<EnemyMovement>();
            EnemyStats cloneStats = clone.GetComponent<EnemyStats>();
            Health cloneHealth = clone.GetComponent<Health>();

            if (cloneStats != null && stats != null)
            {
                cloneStats.price = Mathf.Max(1, Mathf.RoundToInt(stats.price * cloneStatsRatio));
                cloneStats.hp = Mathf.Max(1, stats.hp * cloneStatsRatio);
                cloneStats.moveSpeed = stats.moveSpeed * cloneSpeedRatio;
            }

            if (cloneMovement != null && enemyMovement != null)
            {
                cloneMovement.SetPathIndex(enemyMovement.GetCurrentPathIndex());
                cloneMovement.SetMoveSpeed(cloneStats.moveSpeed);
            }

            if (cloneHealth != null)
            {
                cloneHealth.InitializeHealth(Mathf.RoundToInt(cloneStats.hp));
            }

            clone.SetActive(true);

            // optional: tell spawner / manager a new enemy appeared
            // EnemySpawner.RegisterEnemy(clone);
        }
    }} 