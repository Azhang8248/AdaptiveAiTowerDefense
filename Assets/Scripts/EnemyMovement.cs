using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 2f;
    private float currentSpeed;

    [Header("Path Data")]
    [SerializeField] private Transform[] pathPoints;
    private int targetIndex = 0;
    private Transform targetPoint;

    [Header("Visual Settings")]
    [SerializeField] private Transform visualRoot;   // drag your Sprite child here (optional)
    [SerializeField] private float rotationOffset = -90f; // 0=right-facing sprite, -90=up-facing

    private EnemyStats stats;
    private float slowTimer = 0f;
    private float slowMultiplier = 1f;

    private Vector2 lastPosition; // 🔹 to track movement direction over time

    private void Start()
    {
        stats = GetComponent<EnemyStats>();
        currentSpeed = baseSpeed;
        lastPosition = transform.position;

        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError($"❌ {name} has no path assigned!");
            return;
        }

        targetPoint = pathPoints[targetIndex];
    }

    private void Update()
    {
        if (pathPoints == null || pathPoints.Length == 0)
            return;

        // Handle slow timer
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
                slowMultiplier = 1f;
        }

        MoveAlongPath();
        RotateTowardsMovement(); // 🔹 separate, cleaner rotation logic
    }

    // =====================================================
    // PUBLIC METHODS
    // =====================================================

    public void SetPath(Transform[] points)
    {
        pathPoints = points;
        targetIndex = 0;

        if (pathPoints != null && pathPoints.Length > 0)
            targetPoint = pathPoints[0];
    }

    public Transform[] GetPath() => pathPoints;

    public void SetPathIndex(int index)
    {
        targetIndex = Mathf.Clamp(index, 0, pathPoints.Length - 1);
        if (pathPoints != null && pathPoints.Length > 0)
            targetPoint = pathPoints[targetIndex];
    }

    public int GetCurrentPathIndex() => targetIndex;

    public void ApplySlow(float multiplier, float duration)
    {
        slowMultiplier = Mathf.Clamp(multiplier, 0f, 1f);
        slowTimer = duration;
    }

    public void SetMoveSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
        currentSpeed = baseSpeed * slowMultiplier;
    }

    // =====================================================
    // MOVEMENT + ROTATION
    // =====================================================

    private void MoveAlongPath()
    {
        if (targetPoint == null) return;

        float moveSpeed = currentSpeed * slowMultiplier;
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Reached waypoint
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            targetIndex++;

            if (targetIndex >= pathPoints.Length)
            {
                OnReachEnd();
                return;
            }

            targetPoint = pathPoints[targetIndex];
        }
    }

    private void RotateTowardsMovement()
    {
        Vector2 currentPos = transform.position;
        Vector2 moveDir = (currentPos - lastPosition).normalized;

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg + rotationOffset;
            Quaternion targetRot = Quaternion.Euler(0f, 0f, angle);

            if (visualRoot != null)
                visualRoot.rotation = Quaternion.Lerp(visualRoot.rotation, targetRot, Time.deltaTime * 10f);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        lastPosition = currentPos;
    }

    private void OnReachEnd()
    {
        if (stats != null)
        {
            int goldReward = stats.price * 10;
            int damageToPlayer = stats.price;

            LevelManager.main.AddGold(goldReward);
            LevelManager.main.TakePlayerDamage(damageToPlayer);

            Debug.Log($"{name} reached end! +{goldReward} Gold | Player takes {damageToPlayer} damage");
        }

        EnemySpawner.onEnemyDestroy.Invoke();
        Destroy(gameObject);
    }
}
