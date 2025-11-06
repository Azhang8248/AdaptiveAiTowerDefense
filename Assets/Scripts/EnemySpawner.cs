using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Button startWaveButton; // drag your "Start Wave" button here

    [Header("Wave Settings")]
    [SerializeField] private float enemiesPerSecond = 1f;
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    // Accessible by LevelManager
    public static int CurrentWave { get; private set; } = 0;

    private int currentWave = 0;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawningWave;

    private int aiBudget;
    private int aiSpent;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);

        if (startWaveButton != null)
            startWaveButton.onClick.AddListener(OnStartWaveButtonPressed);
        else
            Debug.LogWarning("Start Wave Button not assigned in EnemySpawner!");
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private void Update()
    {
        if (!isSpawningWave) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond))
        {
            timeSinceLastSpawn = 0f;

            if (enemiesLeftToSpawn > 0)
            {
                SpawnEnemy(enemyPrefabs[0]);
                enemiesLeftToSpawn--;
            }
            else if (aiBudget > aiSpent)
            {
                GameObject chosen = ChooseEnemy();
                EnemyStats stats = chosen.GetComponent<EnemyStats>();
                int cost = stats.price;

                if (aiSpent + cost <= aiBudget)
                {
                    SpawnEnemy(chosen);
                    aiSpent += cost;
                }
                else
                {
                    StopSpawning();
                }
            }
            else if (enemiesAlive == 0)
            {
                StopSpawning();
            }
        }
    }

    private void StopSpawning()
    {
        isSpawningWave = false;
        StartCoroutine(NextWave());
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, LevelManager.main.Startpoint.position, Quaternion.identity);
        enemiesAlive++;
    }

    private IEnumerator NextWave()
    {
        Debug.Log($"Wave {currentWave} complete. Waiting {timeBetweenWaves}s...");
        yield return new WaitForSeconds(timeBetweenWaves);
        // Waits for player to press Start again
    }

    // ==============================
    // START WAVE BUTTON LOGIC
    // ==============================
    public void OnStartWaveButtonPressed()
    {
        if (isSpawningWave) return;
        StartCoroutine(WaveSpawn());
    }

    // ==============================
    // WAVE LOGIC
    // ==============================
    private IEnumerator WaveSpawn()
    {
        currentWave++;
        CurrentWave = currentWave; // keep static in sync
        LevelManager.main.UpdateUI(); // 👈 Update wave UI immediately

        yield return new WaitForSeconds(0.5f);

        timeSinceLastSpawn = 0f;
        aiSpent = 0;
        enemiesLeftToSpawn = 0;
        aiBudget = 0;

        Debug.Log($"Starting Wave {currentWave}");

        if (currentWave <= 3)
        {
            switch (currentWave)
            {
                case 1: enemiesLeftToSpawn = 5; break;
                case 2: enemiesLeftToSpawn = 7; break;
                case 3: enemiesLeftToSpawn = 10; break;
            }
        }
        else
        {
            int scale = GetDifficultyScale();
            aiBudget = Mathf.FloorToInt(10 + (currentWave * scale));
            Debug.Log($"AI Budget for Wave {currentWave}: {aiBudget}");
        }

        isSpawningWave = true;
    }

    private int GetDifficultyScale()
    {
        switch (LevelManager.main.difficulty)
        {
            case LevelManager.Difficulty.Easy: return 1;
            case LevelManager.Difficulty.Medium: return 2;
            case LevelManager.Difficulty.Hard: return 3;
            default: return 2;
        }
    }

    private GameObject ChooseEnemy()
    {
        // For now, always spawn prefab 0.
        // Later: add logic for AI enemy selection based on cost and difficulty
        return enemyPrefabs[0];
    }
}
