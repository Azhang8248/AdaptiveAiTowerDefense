using System.Collections.Generic;
using UnityEngine;

public class Healer : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float healDelay = 3f;
    [SerializeField] private float healRadius = 2.5f;
    [SerializeField] private float healAmount = 2f;
    [SerializeField] private int maxTargets = 3;
    private Animator animator;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= healDelay)
        {
            Heal();
            timer = 0f;
        }
    }

    private void Heal()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("Heal");

        // Finds nearby enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var healList = new List<(Health health, float current, float max, float distance)>();

        foreach (GameObject enemy in enemies)
        {
            if (enemy == gameObject) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > healRadius) continue;

            Health health = enemy.GetComponent<Health>();
            if (health != null)
            {
                float current = health.GetCurrentHitPoints();
                float max = health.GetMaxHitPoints();

                if (current <= 0f || current >= max) continue;

                healList.Add((health, current, max, distance));
            }
        }

        if (healList.Count == 0) return;

        // Sorts by max HP � prioritizes tankier allies
        healList.Sort((a, b) => b.max.CompareTo(a.max));

        for (int i = 0; i < healList.Count && i < maxTargets; i++)
        {
            var target = healList[i];

            float missing = target.max - target.current;
            float healValue = Mathf.Min(healAmount, missing);

            // Use negative damage as healing
            target.health.TakeDamage(-healValue);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, healRadius);
    }
#endif
}
