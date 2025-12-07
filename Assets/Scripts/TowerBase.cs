using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    [Header("General Tower Stats")]
    [SerializeField] private int unlockWave = 1;
    [SerializeField] private int price = 100;

    [Header("Combat Stats")]
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float spread = 0f; // total cone angle
    [SerializeField] private int penetration = 1;
    [SerializeField] private List<GameObject> bulletPrefabs = new List<GameObject>();

    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform firePoint;

    private Transform target;
    private float fireCooldown;
    private float targetRefreshTimer = 0f;

    private void Update()
    {
        // 🔹 If target died or left range → instantly reacquire
        if (target == null || Vector2.Distance(transform.position, target.position) > targetingRange)
            target = FindFurthestTarget();

        // 🔹 Periodic refresh (still useful if enemies reshuffle)
        targetRefreshTimer -= Time.deltaTime;
        if (targetRefreshTimer <= 0f)
        {
            target = FindFurthestTarget();
            targetRefreshTimer = 0.5f; // refresh twice per second
        }

        if (target == null) return;

        RotateTowardsTarget();

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = 1f / attackSpeed;
        }
    }

    private void Fire()
    {
        if (bulletPrefabs.Count == 0 || firePoint == null || target == null)
            return;

        int bulletCount = bulletPrefabs.Count;
        float halfSpread = spread / 2f;

        Vector2 baseDir = (target.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletCount; i++)
        {
            float angleOffset = (bulletCount == 1)
                ? 0f
                : Mathf.Lerp(-halfSpread, halfSpread, (float)i / (bulletCount - 1));

            Quaternion finalRot = Quaternion.Euler(0, 0, baseAngle + angleOffset);

            GameObject bulletObj = Instantiate(bulletPrefabs[i], firePoint.position, finalRot);
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            if (bullet != null)
                bullet.Initialize(target, penetration, finalRot * Vector3.right);
        }
    }

    private void RotateTowardsTarget()
    {
        if (turretRotationPoint == null || target == null) return;

        Vector2 dir = target.position - turretRotationPoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        float rotationSpeed = 20f;
        turretRotationPoint.rotation = Quaternion.Lerp(
            turretRotationPoint.rotation,
            Quaternion.Euler(0, 0, angle),
            Time.deltaTime * rotationSpeed
        );
    }

    private Transform FindFurthestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform furthest = null;
        float maxProgress = -Mathf.Infinity;

        foreach (var e in enemies)
        {
            if (e == null) continue;
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist > targetingRange) continue;

            EnemyMovement movement = e.GetComponent<EnemyMovement>();
            if (movement == null) continue;

            float progress = movement.GetCurrentPathIndex();
            if (progress > maxProgress)
            {
                maxProgress = progress;
                furthest = e.transform;
            }
        }

        return furthest;
    }

    public int GetUnlockWave() => unlockWave;
    public int GetPrice() => price;
}
