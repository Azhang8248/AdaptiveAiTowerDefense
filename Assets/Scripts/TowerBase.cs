using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBase : MonoBehaviour
{
    [Header("General Tower Stats")]
    [SerializeField] private string towerName = "Basic Tower";
    [SerializeField] private int unlockWave = 1;
    [SerializeField] private int price = 100;

    [Header("Combat Stats")]
    [SerializeField] protected float attackSpeed = 1f;   // base fire rate
    [SerializeField] protected float targetingRange = 5f;
    [SerializeField] private float spread = 0f;
    [SerializeField] private int penetration = 1;
    [SerializeField] private List<GameObject> bulletPrefabs = new List<GameObject>();

    [Header("References")]
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] protected Transform firePoint;
    [SerializeField] private GameObject upgradeUI;
    [SerializeField] private Button upgradeButton;

    // ------------------------
    // INTERNAL SYSTEM VARIABLES
    // ------------------------
    private Transform target;
    private float fireCooldown;
    private float targetRefreshTimer = 0f;

    // Buffs
    protected float fireRateBuff = 1f;
    protected float rangeBuff = 1f;

    // ------------------------
    // UPGRADE SYSTEM VARIABLES
    // ------------------------
    [Header("Upgrade Stats")]
    [SerializeField] private int baseUpgradeCost = 100;
    protected int level = 1;

    private float bpsBase;
    private float targetingRangeBase;

    private void Start()
    {
        // store original stats for upgrade scaling
        bpsBase = attackSpeed;
        targetingRangeBase = targetingRange;

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(Upgrade);
    }

    private void Update()
    {
        // reacquire target if lost or out of range
        if (target == null || Vector2.Distance(transform.position, target.position) > GetBuffedRange())
            target = FindFurthestTarget();

        targetRefreshTimer -= Time.deltaTime;
        if (targetRefreshTimer <= 0f)
        {
            target = FindFurthestTarget();
            targetRefreshTimer = 0.5f;
        }

        if (target == null)
            return;

        RotateTowardsTarget();

        // firing
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Fire();
            fireCooldown = 1f / GetBuffedFireRate();
        }
    }

    // ------------------------------------------
    // MAIN FIRING FUNCTION (OVERRIDABLE)
    // ------------------------------------------
    protected virtual void Fire()
    {
        if (bulletPrefabs.Count == 0 || firePoint == null || target == null)
            return;

        float halfSpread = spread / 2f;
        int count = bulletPrefabs.Count;

        Vector2 baseDir = (target.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < count; i++)
        {
            float angleOffset = (count == 1)
                ? 0f
                : Mathf.Lerp(-halfSpread, halfSpread, (float)i / (count - 1));

            Quaternion finalRot = Quaternion.Euler(0, 0, baseAngle + angleOffset);

            GameObject bulletObj = Instantiate(bulletPrefabs[i], firePoint.position, finalRot);
            BulletBase bullet = bulletObj.GetComponent<BulletBase>();
            if (bullet != null)
                bullet.Initialize(target, penetration, finalRot * Vector3.right);
        }
    }

    // ------------------------------------------
    // TARGETING
    // ------------------------------------------
    private void RotateTowardsTarget()
    {
        if (turretRotationPoint == null || target == null)
            return;

        Vector2 dir = target.position - turretRotationPoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        turretRotationPoint.rotation = Quaternion.Lerp(
            turretRotationPoint.rotation,
            Quaternion.Euler(0, 0, angle),
            Time.deltaTime * 20f
        );
    }

    private Transform FindFurthestTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform best = null;
        float maxProgress = -9999f;

        foreach (var e in enemies)
        {
            if (!e) continue;

            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist > GetBuffedRange()) continue;

            EnemyMovement move = e.GetComponent<EnemyMovement>();
            if (move == null) continue;

            float progress = move.GetCurrentPathIndex();
            if (progress > maxProgress)
            {
                maxProgress = progress;
                best = e.transform;
            }
        }

        return best;
    }

    // ------------------------------------------
    // BUFF SYSTEM
    // ------------------------------------------
    public void ApplyFireRateBuff(float m) => fireRateBuff = m;
    public void ResetFireRateBuff() => fireRateBuff = 1f;

    public float GetBuffedFireRate() => attackSpeed * fireRateBuff;
    public float GetBuffedRange() => targetingRange * rangeBuff;

    // ------------------------------------------
    // UPGRADE UI
    // ------------------------------------------
    public virtual void OpenUpgradeUI()
    {
        if (upgradeUI != null)
            upgradeUI.SetActive(true);
    }

    public void CloseUpgradeUI()
    {
        if (upgradeUI != null)
            upgradeUI.SetActive(false);

        UIManager.main.SetHoveringState(false);
    }

    // ------------------------------------------
    // UPGRADE SYSTEM
    // ------------------------------------------
    public void Upgrade()
    {
        int cost = CalculateCost();

        if (cost > LevelManager.main.playerGold)
        {
            Debug.Log("Not enough gold!");
            return;
        }

        LevelManager.main.SpendGold(cost);
        level++;

        attackSpeed = CalculateBPS();
        targetingRange = CalculateRange();

        CloseUpgradeUI();

        Debug.Log($"Upgraded {towerName} → Level {level}");
    }

    private int CalculateCost()
    {
        return Mathf.RoundToInt(baseUpgradeCost * Mathf.Pow(level, 1.1f));
    }

    private float CalculateBPS()
    {
        return bpsBase * Mathf.Pow(level, 0.5f);
    }

    private float CalculateRange()
    {
        return targetingRangeBase * Mathf.Pow(level, 0.3f);
    }

    // ------------------------------------------
    // PUBLIC GETTERS
    // ------------------------------------------
    public int GetUnlockWave() => unlockWave;
    public int GetPrice() => price;
}
