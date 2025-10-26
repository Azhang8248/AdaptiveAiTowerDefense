using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Erratic : MonoBehaviour
{
    [Header("Settings")]
    public float maxSpeed = 2.5f;
    public float accelerationSpeed = .5f;
    public float restDuration = 3f;
    private bool isResting = false;
    private float timer;
    
    private EnemyMovement enemyMovement;
    private EnemyStats stats;


    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        stats = GetComponent<EnemyStats>();
    }

    void Update()
    {

        if (!isResting) {
            if (stats.moveSpeed <= maxSpeed)
            {
                enemyMovement.SetMoveSpeed(stats.moveSpeed += accelerationSpeed * Time.deltaTime);
                Debug.Log(stats.moveSpeed);
            }
            else
            {
                stats.moveSpeed = 0f;
                enemyMovement.SetMoveSpeed(0f);
                isResting = true;
                timer = 0f;
            }
        }
        else {
            timer += Time.deltaTime;
            if(timer >= restDuration)
            {
                    isResting = false;
                    timer = 0f;
                    enemyMovement.SetMoveSpeed(0f);
            }
        }
    }
}
