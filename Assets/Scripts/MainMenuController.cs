using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(OnPlayPressed);
        settingsButton.onClick.AddListener(OnSettingsPressed);
        creditsButton.onClick.AddListener(OnCreditsPressed);
        quitButton.onClick.AddListener(OnQuitPressed);

        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    private void OnPlayPressed()
    {
        SceneManager.LoadScene("LoadingScreen");
    }

    private void OnSettingsPressed()
    {
        settingsPanel.SetActive(true);
    }

    private void OnCreditsPressed()
    {
        creditsPanel.SetActive(true);
    }

    private void OnQuitPressed()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void CloseSettings() => settingsPanel.SetActive(false);
    public void CloseCredits() => creditsPanel.SetActive(false);
}
