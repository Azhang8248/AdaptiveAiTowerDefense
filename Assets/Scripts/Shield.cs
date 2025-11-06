using System;
using UnityEngine;

public class Shield : MonoBehaviour

{
    [Header("Settings")]
    [SerializeField] private int maxShield = 5;
    [SerializeField] private ShieldBar shieldBar;
    public int currentShield;

    private Shield shield;
    void Awake()
    {
        shield = GetComponent<Shield>();
        currentShield = maxShield;

        if (shieldBar != null)
        {
            shieldBar.UpdateBar(maxShield, currentShield);
            shieldBar.gameObject.SetActive(true);
        }
    }

    public void Absorb()
    {
        // Takes 1 hit per shot
        currentShield -= 1;
        shieldBar.SetCurrentShield(currentShield);

        // Once shield reaches 0, disables the shield
        if (currentShield <= 0)
        {
            shieldBar.gameObject.SetActive(false);
            shield.enabled = false;
            shieldBar.enabled = false;
        }
    }
}