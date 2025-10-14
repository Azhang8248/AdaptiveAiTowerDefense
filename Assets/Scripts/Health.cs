using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int maxHitPoints = 2; // set per prefab
    private int currentHitPoints;

    private void Awake()
    {
        currentHitPoints = maxHitPoints; // initialize from prefab
    }

    public void TakeDamage(int dmg)
    {
        currentHitPoints -= dmg;

        if (currentHitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
