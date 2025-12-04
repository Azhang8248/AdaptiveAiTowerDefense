using System.Collections;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path References")]
    public Transform Startpoint;
    public Transform Endpoint;
    public Transform[] path;

    [Header("Difficulty Settings")]
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Player Stats")]
    public int playerGold;
    public int playerHealth;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Background Settings")]
    [SerializeField] private Sprite backgroundSprite;   // drag your sprite here in the inspector
    [SerializeField] private Vector2 backgroundScale = new Vector2(2, 2); // adjust as needed

    public enum Difficulty { Easy, Medium, Hard }

    private void Awake()
    {
        main = this;
        InitializePlayerStats();
        CreateBackground();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void CreateBackground()
    {
        if (backgroundSprite == null) return;

        GameObject bg = new GameObject("Background");
        var sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = backgroundSprite;
        sr.sortingOrder = -100; // ensure it’s behind everything
        bg.transform.position = new Vector3(0, 0, 10); // push it behind camera
        bg.transform.localScale = new Vector3(backgroundScale.x, backgroundScale.y, 1f);
    }

    // ==============================
    // INITIALIZATION
    // ==============================
    private void InitializePlayerStats()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                playerGold = 300;
                playerHealth = 200;
                break;
            case Difficulty.Medium:
                playerGold = 1000;
                playerHealth = 150;
                break;
            case Difficulty.Hard:
                playerGold = 100;
                playerHealth = 100;
                break;
            default:
                playerGold = 200;
                playerHealth = 150;
                break;
        }

        Debug.Log($"Difficulty: {difficulty}, Gold: {playerGold}, Health: {playerHealth}");
    }

    // ==============================
    // PLAYER STAT MANAGEMENT
    // ==============================
    public void AddGold(int amount)
    {
        playerGold += amount;
        UpdateUI();
    }

    public void SpendGold(int amount)
    {
        playerGold = Mathf.Max(0, playerGold - amount);
        UpdateUI();
    }

    public void TakePlayerDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth <= 0)
        {
            playerHealth = 0;
            Debug.Log("Game Over!");
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"{playerHealth} HP";
        if (goldText != null)
            goldText.text = $"{playerGold} Gold";
        if (waveText != null)
            waveText.text = $"Wave {EnemySpawner.CurrentWave}";
    }

    public int GetPlayerHP() => playerHealth;
    public int GetPlayerGold() => playerGold;
    public Difficulty GetDifficulty() => difficulty;
}
