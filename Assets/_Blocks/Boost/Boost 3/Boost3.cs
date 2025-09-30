using UnityEngine;

public class Boost3 : BlockAbility
{
    [Header("���� ����")]
    public float attackMultiplier = 1.5f; // �÷��̾� ���ݷ� ���� (�ν����Ϳ��� ���� ����)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
        {
            Debug.LogWarning("[PlayerAttackScaleBlockAbility] PlayerAbility�� ã�� �� �����ϴ�.");
        }
        descriptionTemplate = "The nearest enemy takes {0} + {1} × Player’s Attack Power damage ({2} + {3} × Player’s Attack Power on a critical hit).";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // �÷��̾� ���ݷ¿� ���� ����
        int scaledPlayerAttack = Mathf.RoundToInt(playerAbility.attackPower * attackMultiplier);

        // ġ��Ÿ ���� ����
        bool isCriticalHit = Random.value < playerAbility.critChance;

        // ������ ���: ���� ������ + (���� ����� �÷��̾� ���ݷ�)
        int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + scaledPlayerAttack;

        // ���� ����
        enemy.TakeDamage(damageToDeal);

        // �α� ���
        if (isCriticalHit)
            Debug.Log($"[PlayerAttackScaleBlock] ġ��Ÿ! {damageToDeal} ����");
        else
            Debug.Log($"[PlayerAttackScaleBlock] �Ϲ�Ÿ�� {damageToDeal} ����");
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
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
                        closest = enemy;
                    }
                }
            }
        }
        return closest;
    }

    public override void Upgrade()
    {
        // Description�� ����
        Description = $"�÷��̾� ���ݷ��� {attackMultiplier}�踦 �߰��Ͽ� �����ϸ� ġ��Ÿ�� ����˴ϴ�.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, attackMultiplier, criticalHitDamage, attackMultiplier);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The nearest enemy takes {0} + {1} × Player’s Attack Power damage ({2} + {3} × Player’s Attack Power on a critical hit).";
        UpdateDescription();
    }
}
