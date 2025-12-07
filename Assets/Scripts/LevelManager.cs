﻿using System.Collections;
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
    public DifficultyLevel difficulty = DifficultyLevel.Normal;

    [Header("Player Stats")]
    public int playerGold;
    public int playerHealth;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Background Settings")]
    [SerializeField] private Sprite backgroundSprite;
    [SerializeField] private Vector2 backgroundScale = new Vector2(2, 2);

    [Header("Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private float musicVolume = 0.1f;

    private void Awake()
    {
        main = this;

        // Pull difficulty chosen from menu
        difficulty = DifficultyManager.CurrentDifficulty;

        InitializePlayerStats();
        CreateBackground();
    }

    private void Start()
    {
        UpdateUI();

        if(AudioManager.Instance != null && backgroundMusic != null)
      {
         AudioManager.Instance.PlayMusic(backgroundMusic, musicVolume);
      }
    }

    private void CreateBackground()
    {
        if (backgroundSprite == null) return;

        GameObject bg = new GameObject("Background");
        var sr = bg.AddComponent<SpriteRenderer>();
        sr.sprite = backgroundSprite;
        sr.sortingOrder = -100;
        bg.transform.position = new Vector3(0, 0, 10);
        bg.transform.localScale = new Vector3(backgroundScale.x, backgroundScale.y, 1f);
    }

    private void InitializePlayerStats()
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                playerGold = 300;
                playerHealth = 200;
                break;

            case DifficultyLevel.Normal:
                playerGold = 200;
                playerHealth = 150;
                break;

            case DifficultyLevel.Hard:
                playerGold = 100;
                playerHealth = 100;
                break;
        }

        Debug.Log($"Difficulty: {difficulty}, Gold: {playerGold}, Health: {playerHealth}");
    }

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
    public DifficultyLevel GetDifficulty()
    {
        return difficulty;
    }

    public int GetPlayerHP()
    {
        return playerHealth;
    }

    public int GetPlayerGold()
    {
        return playerGold;
    }

}