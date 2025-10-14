using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Attributes")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 1f;
    [SerializeField] private float timeBetweenWaves = 5f;

    public enum Difficulty { Easy, Medium, Hard }
    [SerializeField] private Difficulty difficulty = Difficulty.Medium;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 1;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawningWave;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private void Start()
    {
        StartWave();
    }

    private void Update()
    {
        if (!isSpawningWave) return;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond) && enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            enemiesLeftToSpawn--;
            timeSinceLastSpawn = 0f;
        }

        if (enemiesLeftToSpawn == 0 && enemiesAlive == 0 && isSpawningWave)
        {
            isSpawningWave = false;
            StartCoroutine(NextWave());
        }
    }

    private void SpawnEnemy()
    {
        GameObject prefabToSpawn = enemyPrefabs[0];
        Instantiate(prefabToSpawn, LevelManager.main.Startpoint.position, Quaternion.identity);
        enemiesAlive++;
    }

    private IEnumerator NextWave()
    {
        Debug.Log("Wave " + currentWave + " complete. Waiting...");
        yield return new WaitForSeconds(timeBetweenWaves);
        currentWave++;
        StartWave();
    }

    private void StartWave()
    {
        enemiesLeftToSpawn = CalculateWavePoints();
        timeSinceLastSpawn = 0f;
        isSpawningWave = true;
        Debug.Log("Starting Wave: " + currentWave);
    }

    private int CalculateWavePoints()
    {
        float scale = GetDifficultyScale();
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, scale));
    }

    private float GetDifficultyScale()
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return 0.75f;
            case Difficulty.Medium: return 1.0f;
            case Difficulty.Hard: return 1.25f;
            default: return 1.0f;
        }
    }

    // placeholder for when you add the UI button later
    public void OnStartWaveButtonPressed()
    {
        if (!isSpawningWave)
            StartWave();
    }
}
