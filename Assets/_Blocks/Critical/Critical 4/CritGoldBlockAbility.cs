using UnityEngine;

public class CritGoldBlockAbility : BlockAbility
{
    [Header("ġ��Ÿ ���� ����")]
    public int goldOnCrit = 10;  // ġ��Ÿ �߻� �� ȹ���� ��� (�ν����Ϳ��� ����)

    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();
        if (playerAbility == null) Debug.LogWarning("[CritGoldBlockAbility] PlayerAbility�� ã�� �� �����ϴ�.");
        if (moneyManager == null) Debug.LogWarning("[CritGoldBlockAbility] MoneyManager�� ã�� �� �����ϴ�.");
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player gains {2} gold.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null || playerAbility == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // �÷��̾��� ġ��Ÿ Ȯ���� ġ��Ÿ ����
        bool isCriticalHit = Random.value < playerAbility.critChance;
        int damageToDeal = isCriticalHit ? CriticalHitDamage : AttackDamage;

        // ������ ����
        enemy.TakeDamage(damageToDeal + playerAbility.attackPower);

        // ġ��Ÿ�� ��� ����
        if (isCriticalHit && moneyManager != null)
        {
            moneyManager.AddMoney(goldOnCrit);
            Debug.Log($"[CritGoldBlock] ġ��Ÿ! {damageToDeal} ������ + ��� {goldOnCrit} ȹ��");
        }
        else
        {
            Debug.Log($"[CritGoldBlock] �Ϲ� ���� {damageToDeal} ������");
        }
    }

    // ���� x�� ���� ���� ã�� ��ƿ��Ƽ (�ʿ� �� �Ÿ� ������� ���� ����)
    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minX = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyComp = enemy.GetComponent<Enemy>();
                if (enemyComp != null && !enemyComp.IsDead())
                {
                    float xPos = enemy.transform.position.x;
                    if (xPos < minX)
                    {
                        minX = xPos;
                        closestEnemy = enemy;
                    }
                }
            }
        }
        return closestEnemy;
    }

    public override void Upgrade()
    {
        // Description�� ���� (����/�� ���� ����)
        Description = $"���� �� ġ��Ÿ�� �߻��ϸ� ��� {goldOnCrit}�� ȹ���մϴ�. " +
                      $"(����: {AttackDamage}, ġ��Ÿ ����: {CriticalHitDamage})";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, goldOnCrit);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player gains {2} gold.";
        UpdateDescription();
    }
}
