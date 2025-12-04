using System.Collections.Generic;
using UnityEngine;

public class FireRateAuraTower : TowerBase
{
    [Header("Aura Settings")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private float fireRateMultiplier = 1.25f;  // +25% fire rate
    [SerializeField] private LayerMask towerMask;

    private List<TowerBase> buffedTowers = new List<TowerBase>();

    private void Update()
    {
        ApplyAura();
    }

    private void ApplyAura()
    {
        // Reset buffs from last frame
        foreach (var t in buffedTowers)
        {
            if (t != null)
                t.ResetFireRateBuff();
        }
        buffedTowers.Clear();

        // Detect towers in range
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, radius, towerMask);

        foreach (var col in towers)
        {
            TowerBase tower = col.GetComponent<TowerBase>();
            if (tower == null) 
                continue;

            // Don't buff itself
            if (tower.gameObject == this.gameObject)
                continue;

            tower.ApplyFireRateBuff(fireRateMultiplier);
            buffedTowers.Add(tower);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.2f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}
