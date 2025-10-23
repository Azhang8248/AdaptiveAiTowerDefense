using UnityEngine;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject turret;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (turret != null) return;

        // Get the currently selected turret prefab from the BuildManager
        Turret turretToBuild = BuildManager.main.GetSelectedTurret();

        if (turretToBuild == null)
        {
            Debug.LogWarning("No turret selected!");
            return;
        }

        // Check if the player can afford this turret
        if (turretToBuild.GetPrice() > LevelManager.main.playerGold)
        {
            Debug.Log("You cannot afford this turret!");
            return;
        }

        // Spend gold and build the turret
        LevelManager.main.SpendGold(turretToBuild.GetPrice());
        turret = Instantiate(turretToBuild.gameObject, transform.position, Quaternion.identity);
    }
}
