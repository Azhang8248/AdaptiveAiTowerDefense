using UnityEngine;

public class BazookaBullet : BulletBase
{
    [Header("Bazooka Specific")]
    [SerializeField] private float explosionRadius = 2.5f;
    [SerializeField] private float explosionDamageMultiplier = 1f; 
    [SerializeField] private bool explodeOnTimer = true;

    private bool hasExploded = false;

    private void OnDestroy()
    {
    if (explodeOnTimer && !hasExploded)
        Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        // Direct hit → then explode
        Explode();
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        float aoeDamage = damage * explosionDamageMultiplier;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Shield shield = hit.GetComponent<Shield>();
            Health hp = hit.GetComponent<Health>();

            // Apply plasma-like shield bonus automatically?
            float finalDamage = aoeDamage;

            if (gameObject.name.Contains("Plasma") && shield != null && shield.IsActive())
                finalDamage *= 2.5f;

            bool hitShield = false;

            if (shield != null && shield.IsActive())
            {
                hitShield = true;
                shield.TakeShieldDamage(finalDamage);
            }

            if (!hitShield && hp != null)
                hp.TakeDamage(finalDamage);
        }

        // Destroy the rocket after applying damage
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, explosionRadius);
    }
#endif
}