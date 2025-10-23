using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class EnemyStats : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int price = 10;
    public float hp = 10f;
    public float moveSpeed = 2f;

    private void Awake()
    {
        EnemyMovement movement = GetComponent<EnemyMovement>();
        if (movement != null)
            movement.SetMoveSpeed(moveSpeed);

        Health health = GetComponent<Health>();
        if (health != null)
            health.InitializeHealth(Mathf.RoundToInt(hp));
    }
}
