using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;   // current speed
    private float baseSpeed;                         // unmodified base speed

    private Transform target;
    private int pathIndex = 0;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        baseSpeed = moveSpeed;                       // remember base speed once
        target = LevelManager.main.path[pathIndex];
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

        Vector2 dir = (target.position - transform.position).normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    // --- API used by FrostTurret ---

    public void SetMoveSpeed(float speed)
    {
        baseSpeed = Mathf.Max(0f, speed);
        moveSpeed = baseSpeed;
    }

    public void UpdateSpeed(float multiplier)
    {
        // multiplier: 1f = normal, 0.25f = 25% speed, etc.
        multiplier = Mathf.Max(0f, multiplier);
        moveSpeed = baseSpeed * multiplier;
    }

    public void ResetSpeed()
    {
        moveSpeed = baseSpeed;
    }

    // Optional: expose current speed if you need it elsewhere
    public float Speed
    {
        get => moveSpeed;
        set => moveSpeed = Mathf.Max(0f, value);
    }
}
