using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Button startWaveButton;
    [SerializeField] private Transform[] pathPoints;

    [Header("Wave Settings")]
    [SerializeField] private float enemiesPerSecond = 1f; // Base spawn rate
    [SerializeField] private float timeBetweenWaves = 1f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();
    public static int CurrentWave { get; private set; } = 0;

    private int currentWave = 0;
    private float timeSinceLastSpawn;
    private int enemiesAlive;
    private int enemiesLeftToSpawn;
    private bool isSpawningWave;
    private bool waitingForWaveEnd;

    private int aiBudget;
    private int aiSpent;

    // Hard AI reactivity
    private float hardCheckTimer = 0f;
    private float hardRecheckInterval = 3f;
    private int lastTowerHash = 0;

    // Enemy unlock progression
    private Dictionary<int, List<int>> enemyUnlocks;

    // Wave queues
    private Queue<object> waveQueue = new Queue<object>();
    private Queue<int> activeComboBuffer = new Queue<int>();

    // ==============================
    // Counter definitions
    // ==============================
    private Dictionary<string, List<int>> towerCounters = new Dictionary<string, List<int>>()
    {
        { "pistol",       new List<int> { 0, 1 } }, // weak/generic, Walker/Fast
        { "revolver",     new List<int> { 1, 2 } }, // fast & erratic dodge big shots
        { "frost",        new List<int> { 3, 4 } }, // loses to swarm 
        { "shotgun",      new List<int> { 1, 2 } }, // fast dodges, shield tanks
        { "sniper",       new List<int> { 1, 2 } }, // fast/erratic too quick
        { "flamethrower", new List<int> { 7, 6 } }, // tank/shield resist aoe
        { "plasma",       new List<int> { 7, 5 } }, // slime splits, healer outheals
        { "bazooka",      new List<int> { 2, 7 } }, // fast/erratic outrun splash
        { "minigun",      new List<int> { 7, 3 } }, // tank/healer soak chip dmg
    };

    // ==============================
    // Combos (enemy index pairs)
    // ==============================
    private List<(int, int)> enemyCombos = new List<(int, int)>
    {
        (7, 5), // Tank + Healer
        (1, 2), // Fast + Erratic
        (4, 6), // Slime + Shield
        (3, 4), // Summoner + Slime
    };

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);

        if (startWaveButton != null)
            startWaveButton.onClick.AddListener(OnStartWaveButtonPressed);
        else
            Debug.LogWarning("⚠️ Start Wave Button not assigned in EnemySpawner!");

        InitializeEnemyUnlocks();
    }

    private void InitializeEnemyUnlocks()
    {
        enemyUnlocks = new Dictionary<int, List<int>>()
        {
            { 1,  new List<int> { 0 } }, // Walker
            { 3,  new List<int> { 1 } }, // Fast
            { 5,  new List<int> { 2 } }, // Erratic
            { 8,  new List<int> { 6 } }, // Shield
            { 10, new List<int> { 4 } }, // Slime
            { 12, new List<int> { 5 } }, // Healer
            { 15, new List<int> { 7 } }, // Tank
            { 18, new List<int> { 3 } }, // Summoner
        };
    }

    private void EnemyDestroyed()
    {
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

        if (waitingForWaveEnd && enemiesAlive == 0)
        {
            waitingForWaveEnd = false;
            StartCoroutine(NextWaveDelay());
        }
    }

    private void Update()
    {
        if (!isSpawningWave) return;

        timeSinceLastSpawn += Time.deltaTime;

        // === HARD MODE MID-WAVE ADAPTATION ===
        if (LevelManager.main.GetDifficulty() == LevelManager.Difficulty.Hard && aiBudget > 0)
        {
            hardCheckTimer += Time.deltaTime;
            if (hardCheckTimer >= hardRecheckInterval)
            {
                hardCheckTimer = 0f;
                if (HasTowerLayoutChanged())
                {
                    Debug.Log("🧠 Hard AI adapting mid-wave...");
                    int remaining = Mathf.Max(0, aiBudget - aiSpent);
                    BuildWaveQueue(remaining);
                }
            }
        }

        // =====================================

        if (timeSinceLastSpawn >= (1f / enemiesPerSecond))
        {
            timeSinceLastSpawn = 0f;

            if (enemiesLeftToSpawn > 0)
            {
                SpawnEnemy(enemyPrefabs[0]); // Walker only for early waves
                enemiesLeftToSpawn--;
            }
            else if (aiBudget > aiSpent)
            {
                GameObject chosen = ChooseEnemy();
                if (chosen == null) return;

                EnemyStats stats = chosen.GetComponent<EnemyStats>();
                int cost = stats.price;

                if (aiSpent + cost <= aiBudget)
                {
                    SpawnEnemy(chosen);
                    aiSpent += cost;
                }
                else StopSpawning();
            }
            else if (enemiesAlive == 0)
            {
                StopSpawning();
            }
        }
    }

    private void StopSpawning()
    {
        if (!isSpawningWave) return;

        isSpawningWave = false;
        if (enemiesAlive <= 0)
            StartCoroutine(NextWaveDelay());
        else
            waitingForWaveEnd = true;
    }

    private IEnumerator NextWaveDelay()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        waitingForWaveEnd = false;
        isSpawningWave = false;

        if (startWaveButton != null)
            startWaveButton.interactable = true;
    }

    private void SpawnEnemy(GameObject prefab)
    {
        GameObject e = Instantiate(prefab, LevelManager.main.Startpoint.position, Quaternion.identity);
        enemiesAlive++;

        EnemyMovement move = e.GetComponent<EnemyMovement>();
        if (move != null && pathPoints != null && pathPoints.Length > 0)
            move.SetPath(pathPoints);
    }

    public void OnStartWaveButtonPressed()
    {
        if (isSpawningWave || waitingForWaveEnd)
        {
            Debug.Log("⚠️ Cannot start new wave yet!");
            return;
        }
        StartCoroutine(WaveSpawn());
    }

    private IEnumerator WaveSpawn()
    {
        if (startWaveButton != null)
            startWaveButton.interactable = false;

        currentWave++;
        CurrentWave = currentWave;

        LevelManager.main.UpdateUI();
        LogWaveInfo();

        yield return new WaitForSeconds(0.5f);

        timeSinceLastSpawn = 0f;
        aiSpent = 0;
        enemiesLeftToSpawn = 0;
        aiBudget = 0;
        waveQueue.Clear();
        activeComboBuffer.Clear();

        if (currentWave <= 3)
        {
            enemiesLeftToSpawn = currentWave switch
            {
                1 => 5,
                2 => 7,
                3 => 10,
                _ => 5
            };
        }
        else
        {
            int scale = GetDifficultyScale();
            aiBudget = Mathf.FloorToInt(10 + (currentWave * scale));
            Debug.Log($"AI Budget for Wave {currentWave}: {aiBudget}");
            BuildWaveQueue(aiBudget);
        }

        enemiesPerSecond = Mathf.Min(enemiesPerSecond + 0.01f, 2f);
        Debug.Log($"New spawn rate: {enemiesPerSecond:F2} enemies/sec");

        isSpawningWave = true;
    }

    private void LogWaveInfo()
    {
        string diff = LevelManager.main.GetDifficulty().ToString();
        int hp = LevelManager.main.GetPlayerHP();
        int gold = LevelManager.main.GetPlayerGold();
        int wave = currentWave;

        Dictionary<string, int> towerCounts = new();
        foreach (var tower in BuildManager.PlacedTowers)
        {
            if (tower == null) continue;
            string name = tower.name.Replace("(Clone)", "").Trim();
            if (towerCounts.ContainsKey(name))
                towerCounts[name]++;
            else
                towerCounts[name] = 1;
        }

        Debug.Log("===========================================");
        Debug.Log($"📢 STARTING WAVE {wave}");
        Debug.Log($"Difficulty: {diff}");
        Debug.Log($"Player HP: {hp}");
        Debug.Log($"Player Gold: {gold}");
        foreach (var kvp in towerCounts)
            Debug.Log($"{kvp.Key} x{kvp.Value}");
        Debug.Log("===========================================");
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

    // ==============================
    // AI SELECTION + QUEUE LOGIC
    // ==============================

    private GameObject ChooseEnemy()
    {
        if (activeComboBuffer.Count > 0)
        {
            int idx = activeComboBuffer.Dequeue();
            return SafeEnemy(idx);
        }

        if (waveQueue.Count == 0)
            return SafeEnemy(0);

        object next = waveQueue.Dequeue();

        if (next is int id)
            return SafeEnemy(id);
        else if (next is List<int> combo)
        {
            foreach (var i in combo) activeComboBuffer.Enqueue(i);
            return SafeEnemy(activeComboBuffer.Dequeue());
        }

        return SafeEnemy(0);
    }

    private GameObject SafeEnemy(int index)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return null;
        return enemyPrefabs[Mathf.Clamp(index, 0, enemyPrefabs.Length - 1)];
    }

    private void BuildWaveQueue(int remainingBudget)
    {
        waveQueue.Clear();
        activeComboBuffer.Clear();

        var diff = LevelManager.main.GetDifficulty();
        float randomPercent;
        float comboChance;

        switch (diff)
        {
            case LevelManager.Difficulty.Easy:
                randomPercent = Random.Range(0.5f, 1f);
                comboChance = 0.25f;
                break;
            case LevelManager.Difficulty.Medium:
                randomPercent = Random.Range(0.25f, 0.75f);
                comboChance = 0.5f;
                break;
            case LevelManager.Difficulty.Hard:
            default:
                randomPercent = Random.Range(0f, 0.25f);
                comboChance = 0.75f;
                break;
        }

        int randomBudget = Mathf.FloorToInt(remainingBudget * randomPercent);
        int strategyBudget = Mathf.Max(0, remainingBudget - randomBudget);

        List<int> unlocked = GetUnlockedEnemies();
        if (unlocked.Count == 0)
        {
            waveQueue.Enqueue(0);
            return;
        }

        var towers = CountPlacedTowersNormalized();
        string topTower = null, secondTower = null;

        if (towers.Count > 0)
        {
            var sorted = new List<KeyValuePair<string, int>>(towers);
            sorted.Sort((a, b) => b.Value.CompareTo(a.Value));
            topTower = sorted[0].Key;
            if (sorted.Count > 1) secondTower = sorted[1].Key;
        }

        List<object> chunks = new();

        // Random portion
        SpendRandom(chunks, unlocked, randomBudget);
        // Strategy portion
        SpendStrategy(chunks, unlocked, strategyBudget, comboChance, topTower, secondTower);

        // Shuffle preserving combos
        chunks = ShufflePreservingCombos(chunks);
        foreach (var c in chunks) waveQueue.Enqueue(c);
    }

    private void SpendRandom(List<object> chunks, List<int> unlocked, int budget)
    {
        int spent = 0;
        while (spent < budget)
        {
            int id = unlocked[Random.Range(0, unlocked.Count)];
            int cost = enemyPrefabs[id].GetComponent<EnemyStats>().price;
            if (cost <= 0) cost = 1;
            if (spent + cost > budget) break;

            chunks.Add(id);
            spent += cost;
        }
    }

    private void SpendStrategy(List<object> chunks, List<int> unlocked, int budget, float comboChance, string topTower, string secondTower)
    {
        int spent = 0;

        // Try combos
        if (Random.value < comboChance)
        {
            (int a, int b) combo = enemyCombos[Random.Range(0, enemyCombos.Count)];
            if (unlocked.Contains(combo.a) && unlocked.Contains(combo.b))
            {
                int cost = enemyPrefabs[combo.a].GetComponent<EnemyStats>().price +
                           enemyPrefabs[combo.b].GetComponent<EnemyStats>().price;
                if (spent + cost <= budget)
                {
                    chunks.Add(new List<int> { combo.a, combo.b });
                    spent += cost;
                }
            }
        }

        List<int> counterPool = new();
        if (!string.IsNullOrEmpty(topTower) && towerCounters.ContainsKey(topTower))
            counterPool.AddRange(towerCounters[topTower]);
        if (!string.IsNullOrEmpty(secondTower) && towerCounters.ContainsKey(secondTower))
            counterPool.AddRange(towerCounters[secondTower]);
        counterPool.RemoveAll(e => !unlocked.Contains(e));

        while (spent < budget && counterPool.Count > 0)
        {
            int id = counterPool[Random.Range(0, counterPool.Count)];
            int cost = enemyPrefabs[id].GetComponent<EnemyStats>().price;
            if (cost <= 0) cost = 1;
            if (spent + cost > budget) break;

            chunks.Add(id);
            spent += cost;
        }
    }

    private List<object> ShufflePreservingCombos(List<object> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }

    private List<int> GetUnlockedEnemies()
    {
        List<int> available = new();
        foreach (var kvp in enemyUnlocks)
            if (currentWave >= kvp.Key) available.AddRange(kvp.Value);
        available.RemoveAll(i => i < 0 || i >= enemyPrefabs.Length);
        if (available.Count == 0) available.Add(0);
        return available;
    }

    private Dictionary<string, int> CountPlacedTowersNormalized()
    {
        Dictionary<string, int> counts = new();
        foreach (var t in BuildManager.PlacedTowers)
        {
            if (t == null) continue;
            string name = t.name.Replace("(Clone)", "").Trim().ToLower();
            string key = NormalizeTowerKey(name);
            if (string.IsNullOrEmpty(key)) continue;
            if (!counts.ContainsKey(key)) counts[key] = 0;
            counts[key]++;
        }
        return counts;
    }

    private string NormalizeTowerKey(string name)
    {
        if (name.Contains("pistol")) return "pistol";
        if (name.Contains("revolver")) return "revolver";
        if (name.Contains("frost")) return "frost";
        if (name.Contains("shotgun")) return "shotgun";
        if (name.Contains("sniper")) return "sniper";
        if (name.Contains("flame")) return "flamethrower";
        if (name.Contains("plasma")) return "plasma";
        if (name.Contains("bazooka")) return "bazooka";
        if (name.Contains("mini")) return "minigun";
        return null;
    }

    private bool HasTowerLayoutChanged()
    {
        int hash = 0;
        foreach (var t in BuildManager.PlacedTowers)
        {
            if (t == null) continue;
            string n = NormalizeTowerKey(t.name.Replace("(Clone)", "").Trim().ToLower());
            if (!string.IsNullOrEmpty(n))
                hash ^= n.GetHashCode();
        }
        bool changed = hash != lastTowerHash;
        lastTowerHash = hash;
        return changed;
    }

    public int GetCurrentWave() => currentWave;
}
