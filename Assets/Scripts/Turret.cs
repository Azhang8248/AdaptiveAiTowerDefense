using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform firePoint; // Empty GameObject for bullet spawn
    [SerializeField] private GameObject bulletPrefab; // Bullet prefab reference

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float fireRate = 1f; // bullets per second

    [Header("Economy")]
    [SerializeField] private int price = 100; // 💰 cost to build this turret

    private Transform target;
    private float fireCooldown;

    private void Update()
    {
        // Acquire or lose target
        if (target == null || Vector2.Distance(transform.position, target.position) > targetingRange || !target.gameObject.activeInHierarchy)
        {
            target = FindTarget();
        }

        if (target == null) return;

        // Rotate toward target
        Vector2 direction = target.position - turretRotationPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        turretRotationPoint.rotation = Quaternion.Lerp(turretRotationPoint.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Firing logic
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    private Transform FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= targetingRange)
            {
                shortestDistance = distance;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletObj.GetComponent<Bullet>();

        if (bullet != null)
        {
            bullet.SetTarget(target);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.forward, targetingRange);
    }
#endif

    // 👇 Simple getter so other scripts (Plot, BuildManager) can check cost
    public int GetPrice()
    {
        return price;
    }
}
