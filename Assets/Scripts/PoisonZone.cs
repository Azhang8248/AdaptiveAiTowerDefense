using UnityEngine;

public class PoisonZone : MonoBehaviour
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float dps = 3f;
    [SerializeField] private float duration = 5f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
            return;
        }

        ApplyDamage();
    }

    private void ApplyDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Health hp = hit.GetComponent<Health>();
            if (hp)
                hp.TakeDamage(dps * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.2f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
}