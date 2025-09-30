using UnityEngine;

public class lifeSteal3 : BlockAbility
{
    [Header("자가 피해 & 비례 추가 피해 설정")]
    [Tooltip("플레이어가 스스로 받는 피해량 (실드 우선 소모, 부족 시 체력 감소/사망)")]
    public int selfDamage = 10;

    [Tooltip("실제 감소한 체력+실드 1당 추가로 더해질 피해량")]
    public float extraDamagePerLoss = 1.0f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[SelfDamageProportionalStrike] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "Deals {0} damage to yourself, then deals {0} + Player’s Attack Power + {1} per HP lost damage to the nearest enemy ({2} + Player’s Attack Power + {1} per HP lost on a critical hit).";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        playerAbility.TakeDamage(selfDamage);

        // 사망시 공격 취소 (원하면 제거 가능)
        if (playerAbility.currentHealth <= 0)
        {
            Debug.Log("[SelfDamageProportionalStrike] 플레이어 사망으로 공격 취소됨");
            return;
        }

        // 5) 타겟: 가장 가까운 적
        GameObject target = FindClosestEnemy();
        if (target == null) return;

        Enemy enemy = target.GetComponent<Enemy>();
        if (enemy == null) return;

        // 6) 치명타 판정
        bool isCriticalHit = Random.value < playerAbility.critChance;

        // 7) 기본 피해 = (치명타 ? CriticalHitDamage : AttackDamage) + 플레이어 공격력
        int baseDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

        // 8) 추가 피해 = 총 손실량 × 계수
        int extraDamage = Mathf.RoundToInt(selfDamage * extraDamagePerLoss);

        // 9) 최종 피해
        int totalDamage = baseDamage + extraDamage;
        enemy.TakeDamage(totalDamage);


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
        Description = $"플레이어가 {selfDamage} 자해(실드 우선 소모) 후, 잃은 HP+실드 {extraDamagePerLoss}배만큼 추가 피해를 더해 적을 공격합니다.";
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, selfDamage, attackDamage, extraDamagePerLoss, criticalHitDamage, extraDamagePerLoss);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to yourself, then deals {1} + Player’s Attack Power + {2} per HP lost damage to the nearest enemy ({3} + Player’s Attack Power + {4} per HP lost on a critical hit).";
        UpdateDescription();
    }
}
