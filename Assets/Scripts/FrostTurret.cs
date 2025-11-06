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
        var hits = Physics2D.CircleCastAll(
            (Vector2)transform.position,
            range,                 // ← use base field
            Vector2.zero,
            0f,
            enemyMask
        );

        foreach (var hit in hits)
        {
            var em = hit.transform.GetComponent<EnemyMovement>();
            if (em == null) continue;

            em.UpdateSpeed(slowMultiplier);
            StartCoroutine(ResetEnemySpeed(em));
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em)
    {
        yield return new WaitForSeconds(freezeTime);
        if (em != null) em.ResetSpeed();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.forward, range); // ← base field
    }
#endif
}


