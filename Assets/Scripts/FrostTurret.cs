// FrostTurret.cs
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FrostTurret : Turret
{
    [Header("Frost")]
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private float frostps = 4f;
    [SerializeField] private float freezeTime = 1f;
    [SerializeField] private float slowMultiplier = 0.25f;

    private float timeUntilFire;

    private void Update()
    {
        timeUntilFire += Time.deltaTime;
        if (timeUntilFire >= 1f / frostps)
        {
            FreezeEnemies();
            timeUntilFire = 0f;
        }
    }

    private void FreezeEnemies()
{
    RaycastHit2D[] hits = Physics2D.CircleCastAll(
        transform.position,
        range,
        Vector2.zero,
        0f,
        enemyMask
    );

    if (hits.Length > 0)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            EnemyMovement em = hits[i].transform.GetComponent<EnemyMovement>();
            if (em != null)
            {
                // Apply 25% movement (75% slow) for freezeTime seconds
                em.ApplySlow(0.25f, freezeTime);
            }
        }
    }
}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.forward, range); // ← base field
    }
#endif
}


