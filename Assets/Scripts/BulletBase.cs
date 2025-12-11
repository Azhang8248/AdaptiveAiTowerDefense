﻿using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BulletBase : MonoBehaviour
{
    [Header("General Bullet Stats")]
    [SerializeField] private float speed = 10f;
    [SerializeField] protected float damage = 1;
    [SerializeField] private float aoeRadius = 0f;
    [SerializeField, Range(0f, 1f)] private float homingStrength = 0.3f;
    [SerializeField] private float homingTriggerRange = 1.5f;
    [SerializeField] private float lifeSpan = 3f;
    [SerializeField] private bool pierces = false;

    private Transform target;
    private int remainingPenetration;
    private Vector2 moveDir;

    public void Initialize(Transform newTarget, int penetration, Vector2 startDirection)
    {
        target = newTarget;
        remainingPenetration = penetration;
        moveDir = startDirection.normalized;
        Destroy(gameObject, lifeSpan);
    }

    private void Update()
    {
        if (target != null)
        {
            float dist = Vector2.Distance(transform.position, target.position);
            if (dist <= homingTriggerRange)
            {
                Vector2 desired = (target.position - transform.position).normalized;
                moveDir = Vector2.Lerp(moveDir, desired, homingStrength * Time.deltaTime);
                moveDir.Normalize();
            }
        }

        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        float finalDamage = damage;
        Shield shield = other.GetComponent<Shield>();
        Health hp = other.GetComponent<Health>();

        // --- Plasma bullet does bonus to shields ---
        if (gameObject.name.Contains("Plasma Bullet") && shield != null && shield.IsActive())
            finalDamage *= 2.5f;

        bool hitShield = false;

        // --- Handle shield first ---
        if (shield != null && shield.IsActive())
        {
            hitShield = true;
            shield.TakeShieldDamage(finalDamage);   // 👈 encapsulated call
        }

        // --- Only damage health if shield is gone or nonexistent ---
        if (!hitShield && hp != null)
            hp.TakeDamage(finalDamage);

        TryApplySpecialEffect(other.gameObject);

        // --- AOE damage ---
        if (aoeRadius > 0f)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, aoeRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                Shield s = hit.GetComponent<Shield>();
                Health h = hit.GetComponent<Health>();
                float aoeDamage = damage;

                if (gameObject.name.Contains("Plasma Bullet") && s != null && s.IsActive())
                    aoeDamage *= 2.5f;

                bool aoeHitShield = false;

                if (s != null && s.IsActive())
                {
                    aoeHitShield = true;
                    s.TakeShieldDamage(aoeDamage);   // 👈 encapsulated call
                }

                if (!aoeHitShield && h != null)
                    h.TakeDamage(aoeDamage);

                TryApplySpecialEffect(hit.gameObject);
            }
        }

        if (!pierces || --remainingPenetration <= 0)
            Destroy(gameObject);
    }

    private void TryApplySpecialEffect(GameObject enemy)
    {
        if (gameObject.name.Contains("Frost Bullet"))
        {
            EnemyMovement move = enemy.GetComponent<EnemyMovement>();
            if (move)
                move.ApplySlow(0.5f, 2f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, homingTriggerRange);
    }
#endif
}