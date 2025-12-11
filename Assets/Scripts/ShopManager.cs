﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private Button towerShopButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<Button> towerButtons;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 500f;
    [SerializeField] private float openY = 108f;
    [SerializeField] private float closedY = -110f;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color32(136, 136, 136, 255); // #888888
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;

    private bool isOpen = false;
    private Coroutine slideRoutine;
    private int selectedButtonIndex = -1;

    public bool IsOpen => isOpen;

    private void Start()
    {
        // Start hidden
        Vector2 pos = panel.anchoredPosition;
        pos.y = closedY;
        panel.anchoredPosition = pos;

        if (towerShopButton != null)
            towerShopButton.onClick.AddListener(TogglePanel);

        // Hook up tower buttons
        for (int i = 0; i < towerButtons.Count; i++)
        {
            int index = i;
            towerButtons[i].onClick.AddListener(() => OnTowerButtonClicked(index));
        }

        RefreshAllButtonTexts();
        RefreshTowerUnlocks();

        // Disable interaction when closed
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }


    // UI REFRESH

    private void RefreshAllButtonTexts()
    {
        if (BuildManager.main == null) return;

        TowerBase[] towers = BuildManager.main.GetTowers();
        if (towers == null) towers = new TowerBase[0];

        for (int i = 0; i < towerButtons.Count; i++)
        {
            if (i >= towers.Length || towers[i] == null)
            {
                towerButtons[i].gameObject.SetActive(false);
                continue;
            }

            UpdateTowerButtonText(i, towers[i]);
        }
    }

    private void UpdateTowerButtonText(int index, TowerBase tower)
    {
        if (towerButtons[index] == null || tower == null) return;

        Text legacyText = towerButtons[index].GetComponentInChildren<Text>(true);
        TMP_Text tmpText = towerButtons[index].GetComponentInChildren<TMP_Text>(true);

        string displayText = $"{tower.name}\n({GetTowerPrice(tower)} G)";

        if (tmpText != null)
            tmpText.text = displayText;
        else if (legacyText != null)
            legacyText.text = displayText;
        else
            Debug.LogWarning($"Button {index} has no text component!");
    }

    private int GetTowerPrice(TowerBase tower)
    {
        var field = tower?.GetType().GetField("price",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (int)field.GetValue(tower) : 0;
    }


    // SHOP TOGGLE

    public void TogglePanel()
    {
        isOpen = !isOpen;

        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        slideRoutine = StartCoroutine(SlidePanel(isOpen ? openY : closedY));

        if (canvasGroup != null)
        {
            canvasGroup.interactable = isOpen;
            canvasGroup.blocksRaycasts = isOpen;
        }

        if (isOpen)
        {
            RefreshAllButtonTexts();
            RefreshTowerUnlocks();
        }
        else
        {
            ResetButtonVisuals();
        }
    }

    private IEnumerator SlidePanel(float targetY)
    {
        Vector2 startPos = panel.anchoredPosition;
        Vector2 targetPos = new Vector2(startPos.x, targetY);

        while (Vector2.Distance(panel.anchoredPosition, targetPos) > 0.1f)
        {
            panel.anchoredPosition = Vector2.MoveTowards(
                panel.anchoredPosition,
                targetPos,
                slideSpeed * Time.deltaTime
            );
            yield return null;
        }

        panel.anchoredPosition = targetPos;
    }


    // BUTTON HANDLING

    private void OnTowerButtonClicked(int index)
    {
        BuildManager.main.SetSelectedTower(index);
        TowerBase t = BuildManager.main.GetSelectedTower();
        Debug.Log($"Selected Tower: {t.name} ({GetTowerPrice(t)} G)");
        UpdateSelectedButtonVisuals(index);
    }

    private void UpdateSelectedButtonVisuals(int newIndex)
    {
        // Reset old button
        if (selectedButtonIndex >= 0 && selectedButtonIndex < towerButtons.Count)
        {
            Button oldButton = towerButtons[selectedButtonIndex];
            if (oldButton != null)
            {
                var oldImg = oldButton.GetComponent<Image>();
                if (oldImg) oldImg.color = normalColor;

                var oldTMP = oldButton.GetComponentInChildren<TMP_Text>(true);
                var oldText = oldButton.GetComponentInChildren<Text>(true);
                if (oldTMP) oldTMP.color = normalTextColor;
                if (oldText) oldText.color = normalTextColor;
            }
        }

        // Apply new color & text
        selectedButtonIndex = newIndex;
        if (selectedButtonIndex >= 0 && selectedButtonIndex < towerButtons.Count)
        {
            Button newButton = towerButtons[selectedButtonIndex];
            if (newButton != null)
            {
                var newImg = newButton.GetComponent<Image>();
                if (newImg) newImg.color = selectedColor;

                var newTMP = newButton.GetComponentInChildren<TMP_Text>(true);
                var newText = newButton.GetComponentInChildren<Text>(true);
                if (newTMP) newTMP.color = selectedTextColor;
                if (newText) newText.color = selectedTextColor;
            }
        }
    }

    private void ResetButtonVisuals()
    {
        for (int i = 0; i < towerButtons.Count; i++)
        {
            if (towerButtons[i] == null) continue;

            var img = towerButtons[i].GetComponent<Image>();
            if (img) img.color = normalColor;

            var tmp = towerButtons[i].GetComponentInChildren<TMP_Text>(true);
            var txt = towerButtons[i].GetComponentInChildren<Text>(true);
            if (tmp) tmp.color = normalTextColor;
            if (txt) txt.color = normalTextColor;
        }

        selectedButtonIndex = -1;
    }

    // UNLOCK HANDLING

    public void RefreshTowerUnlocks()
    {
        if (BuildManager.main == null || towerButtons == null) return;

        TowerBase[] towers = BuildManager.main.GetTowers();
        if (towers == null) towers = new TowerBase[0];

        int currentWave = FindFirstObjectByType<EnemySpawner>()?.GetCurrentWave() ?? 1;

        for (int i = 0; i < towerButtons.Count; i++)
        {
            if (i >= towers.Length || towers[i] == null)
            {
                towerButtons[i].gameObject.SetActive(false);
                continue;
            }

            TowerBase tower = towers[i];
            bool unlocked = currentWave >= GetTowerUnlockWave(tower);
            towerButtons[i].gameObject.SetActive(unlocked);
        }
    }

    private int GetTowerUnlockWave(TowerBase tower)
    {
        var field = tower?.GetType().GetField("unlockWave",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (int)field.GetValue(tower) : 1;
    }

    public void UnlockTower(int index)
    {
        if (index >= 0 && index < towerButtons.Count)
        {
            TowerBase[] towers = BuildManager.main.GetTowers();
            if (index >= towers.Length || towers[index] == null)
            {
                towerButtons[index].gameObject.SetActive(false);
                return;
            }

            towerButtons[index].gameObject.SetActive(true);
            UpdateTowerButtonText(index, towers[index]);
            Debug.Log($"Unlocked tower button {index}");
        }
    }
}