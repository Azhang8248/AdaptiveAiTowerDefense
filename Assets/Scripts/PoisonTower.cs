using UnityEngine;

public class PoisonTower : TowerBase
{
    [Header("Poison Settings")]
    [SerializeField] private GameObject poisonZonePrefab;
    [SerializeField] private float zoneOffset = 0.2f; // small forward offset if needed

    protected override void Fire()
    {
        // Spawn Poison Zone at the fire point
        if (poisonZonePrefab != null && firePoint != null)
        {
            Vector3 pos = firePoint.position + firePoint.up * zoneOffset;
            Instantiate(poisonZonePrefab, pos, Quaternion.identity);
        }

        // OPTIONAL: call base Fire() if you still want bullets
        // base.Fire();
    }
}