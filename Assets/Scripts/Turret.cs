using UnityEngine;
using UnityEngine.UI;                 // <- keep outside #if, since you serialize Button
using UnityEngine.Serialization;      // <- needed for FormerlySerializedAs
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Turret : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private Transform firePoint;         // bullet spawn
    [SerializeField] private GameObject bulletPrefab;     // bullet prefab
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;        // (optional) remove if unused

    [Header("Attributes")]
    // Migrate old 'targetingRange' values from existing prefabs to this single field:
    [FormerlySerializedAs("targetingRange")]
    [SerializeField] protected float range = 5f;          // <-- the only range field
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float fireRate = 1f;         // bullets per second
    [SerializeField] private int baseUpgradeCost = 100;

    [Header("Economy")]
    [SerializeField] private int price = 100;

    private float bpsBase;
    private float targetingRangeBase;

    private Transform target;
    private float fireCooldown;

    private int level = 1;

    private void Start()
    {
        bpsBase = fireRate;
        targetingRangeBase = range;

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
        // Acquire or lose target
        if (target == null || Vector2.Distance(transform.position, target.position) > range || !target.gameObject.activeInHierarchy)
        {
            target = FindTarget();
        }
        if (target == null) return;

        // Rotate toward target
        Vector2 direction = target.position - turretRotationPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        turretRotationPoint.rotation = Quaternion.Lerp(turretRotationPoint.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Firing
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    private Transform FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= range)   //  use base 'range'
            {
                shortestDistance = distance;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    private void Shoot()
{
    if (bulletPrefab == null || firePoint == null) return;

    GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

    BulletBase bullet = bulletObj.GetComponent<BulletBase>();
    if (bullet != null)
    {
        Vector2 dir = (target.position - firePoint.position).normalized;
        bullet.Initialize(target, 1, dir);  // 1 = penetration count, adjust if needed
    }
}


    public virtual void OpenUpgradeUI()
    {
        if (upgradeUI != null) upgradeUI.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        if (upgradeUI != null) upgradeUI.SetActive(false);
        UIManager.main.SetHoveringState(false);
    }

    public void Upgrade()
    {
        int cost = CalculateCost();

        // Check playerGold instead of currency
        if (cost > LevelManager.main.playerGold) 
        {
            Debug.Log("Not enough gold!");
            return;
        }

        LevelManager.main.SpendGold(cost);  
        level++;

        fireRate = CalculateBPS();
        range = CalculateRange();

        CloseUpgradeUI();

        Debug.Log($"New BPS: {fireRate}, New Range: {range}, New Cost: {CalculateCost()}");
    }


    private int CalculateCost(){
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 0.8f));
    }

    private float CalculateBPS(){
        return bpsBase * Mathf.Pow(level, 0.5f);
    }

    private float CalculateRange(){
        return targetingRangeBase * Mathf.Pow(level, 0.4f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, Vector3.forward, range); // <-- use base 'range'
    }
#endif

    public int GetPrice() => price;
}