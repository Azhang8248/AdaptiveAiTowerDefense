using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHitPoints; // set per prefab
    private int currentHitPoints;
    [SerializeField] public HealthBar healthBar;

    private void Awake()
    {
        currentHitPoints = maxHitPoints; // initialize from prefab
        healthBar.UpdateBar(maxHitPoints, currentHitPoints);
    }

    public void TakeDamage(int dmg)
    {
        currentHitPoints -= dmg;
        healthBar.SetCurrentHealth(currentHitPoints);
        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public void Die()
   {
        Destroy(gameObject);
   }
}
