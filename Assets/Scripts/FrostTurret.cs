using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FrostTurret : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private LayerMask enemyMask;

    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float frostps = 4f; //frost per second
    [SerializeField] private float freezeTime = 1f;

    private float timeUntilFire;

    private void Update(){
        timeUntilFire += Time.deltaTime;

        if(timeUntilFire >= 1f/frostps){
            Debug.Log("Freeze");
            FreezeEnemies();
            timeUntilFire = 0f;
        }
    }

    private void FreezeEnemies(){
        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position, targetingRange, Vector2.zero, 0f, enemyMask
        );

        foreach (var hit in hits) {
            EnemyMovement em = hit.transform.GetComponent<EnemyMovement>();
            if (em != null) {
                em.UpdateSpeed(0.25f);
                StartCoroutine(ResetEnemySpeed(em));
            }
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em){
        yield return new WaitForSeconds(freezeTime);

        em.ResetSpeed();
    }

    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.forward, targetingRange);
    }
}
