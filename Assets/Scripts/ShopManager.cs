using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;        // The shop panel
    [SerializeField] private Button towerShopButton;     // The "Tower Shop" toggle button
    [SerializeField] private CanvasGroup canvasGroup;    // For click blocking and fading
    [SerializeField] private List<Button> towerButtons;  // All tower buttons (11 total)

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 500f;    // Pixels per second
    [SerializeField] private float openY = 108f;         // Target Y when open
    [SerializeField] private float closedY = -110f;      // Target Y when closed

    private bool isOpen = false;
    private Coroutine slideRoutine;

    private void Start()
    {
        // Ensure panel starts hidden
        Vector2 pos = panel.anchoredPosition;
        pos.y = closedY;
        panel.anchoredPosition = pos;

        // Button to open/close panel
        if (towerShopButton != null)
            towerShopButton.onClick.AddListener(TogglePanel);

        // Hide all except first two (unlockables)
        for (int i = 0; i < towerButtons.Count; i++)
        {
            int index = i; // capture index for lambda
            towerButtons[i].onClick.AddListener(() => OnTowerButtonClicked(index));
            towerButtons[i].gameObject.SetActive(i < 2);
        }

        // disable interaction when hidden
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    // Toggle panel visibility
    public void TogglePanel()
    {
        isOpen = !isOpen;

        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        slideRoutine = StartCoroutine(SlidePanel(isOpen ? openY : closedY));

        if (canvasGroup != null)
        {
            canvasGroup.interactable = isOpen;
            canvasGroup.blocksRaycasts = isOpen; // prevent clicks behind
        }
    }

    // Smooth slide animation
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

    // Called when any tower button is clicked
    private void OnTowerButtonClicked(int index)
    {
        BuildManager.main.SetSelectedTurret(index);
        Debug.Log($"Selected Tower: {index}");
    }

    // Unlocks additional tower buttons later
    public void UnlockTower(int index)
    {
        if (index >= 0 && index < towerButtons.Count)
        {
            towerButtons[index].gameObject.SetActive(true);
            Debug.Log($"Unlocked tower button {index}");
        }
    }
}
