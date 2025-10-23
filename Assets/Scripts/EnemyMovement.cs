using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    private Transform target;
    private int pathIndex = 0;

    private float moveSpeed = 2f;
    private float baseSpeed; // remember original speed so we can restore it

    private void Start()
    {
        target = LevelManager.main.path[pathIndex];
        baseSpeed = moveSpeed; // ensure base speed is recorded
    }

    private void Update()
    {
        if (target == null) return;

        if (Vector2.Distance(transform.position, target.position) <= 0.1f)
        {
            pathIndex++;
            if (pathIndex >= LevelManager.main.path.Length)
            {
                LevelManager.main.TakePlayerDamage(10);
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }

            target = LevelManager.main.path[pathIndex];
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    // 👇 Frost turret calls this to change enemy speed temporarily
    public void UpdateSpeed(float slowMultiplier)
    {
        moveSpeed = baseSpeed * slowMultiplier;
    }

    // 👇 Frost turret calls this later to restore speed
    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

    // 👇 Called by EnemyStats at spawn time
    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
        baseSpeed = speed; // store original speed for reset
    }
}
