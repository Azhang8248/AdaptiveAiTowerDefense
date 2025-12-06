using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private Animator anim;

    private bool isMenuOpen = true;

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        anim.SetBool("Menu Open", isMenuOpen);
    }

    private void Update()
    {
        // Update the UI each frame with the player's current gold
        if (LevelManager.main != null && currencyUI != null)
        {
            currencyUI.text = LevelManager.main.playerGold.ToString();
        }
    }
}
