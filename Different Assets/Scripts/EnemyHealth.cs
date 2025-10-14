using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float currentHealth;

    [SerializeField] public HealthBar healthBar;


    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.UpdateBar(maxHealth, currentHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetCurrentHealth(currentHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
   
   public void Die()
    {
        // Maybe add gaining money here
        // or losing health on touching the end
        Destroy(gameObject);
   }

    // Update is called once per frame
    void Update()
    {
        

    }
}
