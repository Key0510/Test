using UnityEngine;

public class CritHealBlockAbility : BlockAbility
{
    [Header("ġ��Ÿ ȸ�� ����")]
    public int healOnCrit = 10; // ġ��Ÿ �� ȸ���� ü�� (�ν����Ϳ��� ���� ����)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
        {
            Debug.LogWarning("[CritHealBlockAbility] PlayerAbility�� ã�� �� �����ϴ�.");
        }
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player restores {2} Health.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null || playerAbility == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // �÷��̾��� ġ��Ÿ Ȯ���� ġ��Ÿ ���� ����
        bool isCriticalHit = Random.value < playerAbility.critChance;
        int damageToDeal = isCriticalHit ? CriticalHitDamage : AttackDamage;

        // ������ ������ ����
        enemy.TakeDamage(damageToDeal + playerAbility.attackPower);

        // ġ��Ÿ �߻� �� ü�� ȸ��
        if (isCriticalHit)
        {
            playerAbility.Heal(healOnCrit);
            Debug.Log($"[CritHealBlock] ġ��Ÿ! {damageToDeal} ������ + ü�� {healOnCrit} ȸ��");
        }
        else
        {
            Debug.Log($"[CritHealBlock] �Ϲ� ���� {damageToDeal} ������");
        }
    }

    // ���� x�� ���� �� ã�� (�ʿ� �� �Ÿ� �������� ���� ����)
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
        // Description�� ���� (���� ���� ����)
        Description = $"���� �� ġ��Ÿ�� �߻��ϸ� ü�� {healOnCrit}�� ȸ���մϴ�. " +
                      $"(����: {AttackDamage}, ġ��Ÿ ����: {CriticalHitDamage})";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, healOnCrit);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player restores {2} Health.";
        UpdateDescription();
    }
}
