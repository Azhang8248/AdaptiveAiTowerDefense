using UnityEngine;
using UnityEngine.UI;

public class CreditPanelController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creditsPanel;
    public Button backButton;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(CloseCredits);
    }

    private void OnEnable()
    {
        // Reset scroll position or animations here later if needed
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }
}