using UnityEngine;

public class ShieldBlock3 : BlockAbility
{
    [Header("피해 비례 계수")]
    [Tooltip("소진한 방어도 1당 추가 피해량 (예: 1.5 = 1당 1.5 피해)")]
    public float damagePerShield = 1.0f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[ShieldConsumeAttackBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player attacks the nearest enemy for {0} + Player’s Attack Power (or {1} + Player’s Attack Power if it is a critical hit), and deals an additional {2} damage for every 1 Player's Defense.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 1) 현재 방어도 전부 소진
        int consumedShield = Mathf.Max(0, playerAbility.shield);
        if (consumedShield > 0)
        {

        }

        // 2) 공격 대상: 가장 가까운 적
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // 3) 치명타 판정 (네 규칙 그대로)
        bool isCriticalHit = Random.value < playerAbility.critChance;

        // 4) 기본 피해 + 플레이어 공격력
        int baseBlockDamage = isCriticalHit ? CriticalHitDamage : AttackDamage;
        int baseDamage = baseBlockDamage + playerAbility.attackPower;

        // 5) 소진한 방어도 비례 추가 피해
        int extraDamage = Mathf.RoundToInt(consumedShield * damagePerShield);

        int damageToDeal = baseDamage + extraDamage;

        // 6) 피해 적용
        enemy.TakeDamage(damageToDeal);

        if (isCriticalHit)
            Debug.Log($"[ShieldConsumeAttack] CRIT! (소진:{consumedShield}, 추가:{extraDamage}) 총 피해 {damageToDeal}");
        else
            Debug.Log($"[ShieldConsumeAttack] (소진:{consumedShield}, 추가:{extraDamage}) 총 피해 {damageToDeal}");
    }

    // 예시와 동일: x가 가장 작은 적 탐색
    public GameObject FindClosestEnemy()
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
        // Description만 변경 (다른 스탯 변경 금지)
        Description = $"플레이어의 방어도를 모두 소진하고, 소진량 × {damagePerShield} 만큼 추가 피해를 더해 가장 가까운 적을 공격합니다.";
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, damagePerShield);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player attacks the nearest enemy for {0} + Player’s Attack Power (or {1} + Player’s Attack Power if it is a critical hit), and deals an additional {2} damage for every 1 Player's Defense.";
        UpdateDescription();
    }
}
