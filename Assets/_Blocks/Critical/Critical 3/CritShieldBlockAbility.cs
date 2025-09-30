using UnityEngine;

public class CritShieldBlockAbility : BlockAbility
{
    [Header("ġ��Ÿ �� �� ����")]
    public int shieldGainOnCrit = 5; // ġ��Ÿ �� ��� �� ��ġ

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player gains {2} Defense.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null && playerAbility != null)
            {
                // �÷��̾��� ġ��Ÿ Ȯ���� ���� ġ��Ÿ ���� ����
                bool isCriticalHit = Random.value < playerAbility.critChance;
                int damageToDeal = isCriticalHit ? CriticalHitDamage : AttackDamage;

                // ������ ����
                enemy.TakeDamage(damageToDeal + playerAbility.attackPower);

                if (isCriticalHit)
                {
                    // ġ��Ÿ�� �� �߰�
                    playerAbility.AddShield(shieldGainOnCrit);
                    Debug.Log($"ġ��Ÿ �߻�! {shieldGainOnCrit} �� ȹ��.");
                }
            }
        }
    }

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
        Description = $"���� �� ġ��Ÿ�� �߻��ϸ� {shieldGainOnCrit} ���� ����ϴ�. " +
                      $"(���ط�: {AttackDamage}, ġ��Ÿ ���ط�: {CriticalHitDamage})";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, shieldGainOnCrit);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player gains {2} Defense.";
        UpdateDescription();
    }
}
