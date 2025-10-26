using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed;

    private Transform target;
    private int pathIndex = 0;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float baseSpeed; // remember original speed so we can restore it

    private void Start()
    {
        target = LevelManager.main.path[pathIndex];
    }

    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex >= LevelManager.main.path.Length)
            {
                EnemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = LevelManager.main.path[pathIndex];
            }
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

    // Called by enemies with summons or splits
    public void SetPathIndex(int index)
    {
        pathIndex = index;
        if (index < LevelManager.main.path.Length)
        {
            target = LevelManager.main.path[index];
        }
      else
        {
            target = null;
        }
    }

    // Called by enemeis with summons or splits
    public int GetCurrentPathIndex()
    {
        return pathIndex;
    }
}
