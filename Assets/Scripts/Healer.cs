using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] private float healDelay = 3f;
    [SerializeField] private float healRadius = 2.5f;
    [SerializeField] private int healAmount= 2;
    [SerializeField] private int maxTargets = 3;
    private float timer = 0f;
    private void Update()
    {
        timer += Time.deltaTime;

        if(timer >= healDelay) {
            Heal();
            timer = 0f;
      }

    }

    private void Heal()
    {
        // Finds other enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var healList = new List<(Health health, int current, int max, float distance)>();

        foreach (GameObject enemy in enemies)
        {
            if (enemy == gameObject) continue;
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > healRadius) continue;

            Health health = enemy.GetComponent<Health>();
            if (health != null)
            {
                int current = health.getCurrentHitPoints();
                int max = health.getMaxHitPoints();

                if (current <= 0 || current >= max) continue;

                healList.Add((health, current, max, distance));
            }
        }

        if (healList.Count == 0) return;

        // Sorts by maxHitPoints | Prioritizes tankier enemies
        healList.Sort((a, b) => b.max.CompareTo(a.max));

        for (int i = 0; i < healList.Count && i < maxTargets; i++)
        {
            var target = healList[i];

            int missing = target.max - target.current;
            int healValue = Mathf.Min(healAmount, missing);

            target.health.TakeDamage(-healValue);
        }
    }
}
