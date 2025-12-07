using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Buttons")]
    public Button backButton;
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;

    [Header("Colors")]
    public Color selectedColor = Color.gray;
    public Color normalColor = Color.white;

    private void Awake()
    {
        backButton.onClick.AddListener(ClosePanel);

        easyButton.onClick.AddListener(() => SetDifficulty(DifficultyLevel.Easy));
        normalButton.onClick.AddListener(() => SetDifficulty(DifficultyLevel.Normal));
        hardButton.onClick.AddListener(() => SetDifficulty(DifficultyLevel.Hard));
    }

    private void OnEnable()
    {
        RefreshVisuals();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void SetDifficulty(DifficultyLevel diff)
    {
        DifficultyManager.CurrentDifficulty = diff;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        ResetButton(easyButton);
        ResetButton(normalButton);
        ResetButton(hardButton);

        switch (DifficultyManager.CurrentDifficulty)
        {
            case DifficultyLevel.Easy:
                HighlightButton(easyButton);
                break;
            case DifficultyLevel.Normal:
                HighlightButton(normalButton);
                break;
            case DifficultyLevel.Hard:
                HighlightButton(hardButton);
                break;
        }
    }

    private void HighlightButton(Button btn)
    {
        btn.image.color = selectedColor;
    }

    private void ResetButton(Button btn)
    {
        btn.image.color = normalColor;
    }
}