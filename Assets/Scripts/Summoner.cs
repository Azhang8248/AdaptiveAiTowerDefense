using Unity.VisualScripting;
using UnityEngine;

public class Summoner : MonoBehaviour
{
    [Header("References")]
    public GameObject summonPrefab; // Drag summon prefab

    [Header("Settings")]
    [SerializeField] private int numberOfSummons = 3;
    [SerializeField] private float summonDelay = 4f;
    private float timer = 0f;
    private EnemyMovement enemyMovement;
    private void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
   }
    void Update()
    {
    timer += Time.deltaTime;
        
        if(timer >= summonDelay) {

          SummonEnemies();
          timer = 0f;
      }

    }

    void SummonEnemies()
    {
        if (summonPrefab == null) return;
        
        for(int i = 0; i < numberOfSummons; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * .4f;
            GameObject summoned = Instantiate(summonPrefab, spawnPos, Quaternion.identity);
            summoned.transform.localScale *= .5f;

            EnemyMovement summonedMovement = summoned.GetComponent<EnemyMovement>();
            
            // continue the pathing
            if(summonedMovement != null)
         {
                summonedMovement.SetPathIndex(enemyMovement.GetCurrentPathIndex());
         }
      }
    }
}
