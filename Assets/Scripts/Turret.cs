using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform firePoint; // empty GameObject for spawn position

    [Header("Attributes")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float fireRate = 1f; // bullets per second

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefab; // bullet prefab reference

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

        Debug.Log("Turret fired!");
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
}
