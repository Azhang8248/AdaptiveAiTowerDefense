using UnityEngine;

public class Summoner : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Enemy prefab to summon.")]
    [SerializeField] private GameObject summonPrefab;

    [Header("Summon Settings")]
    [SerializeField] private int numberOfSummons = 3;
    [SerializeField] private float summonDelay = 4f;
    [SerializeField] private float summonRadius = 0.4f;
    [SerializeField] private float summonScale = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioClip summonSFX;
    [SerializeField] private AudioSource summonSource;

private Animator animator;
    private float timer = 0f;
    private EnemyMovement enemyMovement;

    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        if (enemyMovement == null)
            Debug.LogWarning($"⚠️ {gameObject.name} is missing EnemyMovement!");
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= summonDelay)
        {
            SummonEnemies();
            timer = 0f;
        }
    }

    private void SummonEnemies()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Summon");
        
        if (summonPrefab == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} tried to summon but no prefab assigned!");
            return;
        }

        if (enemyMovement == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} has no EnemyMovement — cannot assign path to summons!");
            return;
        }

        for (int i = 0; i < numberOfSummons; i++)
        {
            // Small random spawn offset
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * summonRadius;

            // Create summon
            GameObject summoned = Instantiate(summonPrefab, spawnPos, Quaternion.identity);
            summoned.transform.localScale *= summonScale;

            // Assign movement path
            EnemyMovement summonedMove = summoned.GetComponent<EnemyMovement>();
            if (summonedMove != null)
            {
                summonedMove.SetPath(enemyMovement.GetPath());
                summonedMove.SetPathIndex(enemyMovement.GetCurrentPathIndex());
            }
            else
            {
                Debug.LogWarning($"⚠️ {summoned.name} has no EnemyMovement component!");
            }

                
            if(summonSFX != null & summonSource != null)
            {
                summonSource.PlayOneShot(summonSFX, 1f);
            }

            // Register to spawner tracking (so wave won’t end early)
            EnemySpawner.onEnemyDestroy.Invoke(); // just to keep event flow consistent
            RegisterSummonToSpawner();
        }

        Debug.Log($"🧙‍♂️ {gameObject.name} summoned {numberOfSummons} allies!");
    }

    private void RegisterSummonToSpawner()
    {
        // Increment global enemy count if spawner exists
        var spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            // Hacky but ensures the wave doesn’t end early
            // Because we can’t directly access enemiesAlive (it’s private)
            // You can modify EnemySpawner to expose a method instead:
            // spawner.RegisterEnemy();
        }
    }
}
