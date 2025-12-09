using UnityEngine;

public class Shield : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int maxShield = 5;
    [SerializeField] private ShieldBar shieldBar;
    public int currentShield { get; private set; }

    [Header("Audio")]
    [SerializeField] AudioClip shieldSFX;
    [SerializeField] AudioSource shieldSource;
    private Animator animator;

    private void Awake()
    {
        currentShield = maxShield;

        if (shieldBar != null)
        {
            // Auto-link the bar to follow this enemy
            shieldBar.Initialize(transform, maxShield, currentShield);
            shieldBar.gameObject.SetActive(true);
        }
    }

    public void TakeShieldDamage(float dmg)
    {
        currentShield -= Mathf.RoundToInt(dmg);
        currentShield = Mathf.Max(0, currentShield);

        if (shieldBar != null)
            shieldBar.SetCurrentShield(currentShield);

        if (currentShield <= 0)
            DisableShield();
    }

    public bool IsActive()
    {
        return enabled && currentShield > 0;
    }

    private void DisableShield()
    {
        animator = GetComponentInChildren<Animator>();
        animator.SetTrigger("ShieldBroken");
        
        if(shieldSFX != null & shieldSource != null)
        {
            shieldSource.PlayOneShot(shieldSFX, 1f);
        }

        if (shieldBar != null)
        {
            shieldBar.gameObject.SetActive(false);
            shieldBar.enabled = false;
        }

        enabled = false;
    }
}
