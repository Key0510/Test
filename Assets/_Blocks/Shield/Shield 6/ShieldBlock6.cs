using UnityEngine;

public class ShieldBlock6 : BlockAbility
{
    [Header("방어도 & 추가 피해 설정")]
    [Tooltip("이 블록 사용 시 즉시 획득하는 방어도(실드) 값")]
    public int defenseGain = 5;

    [Tooltip("획득한 방어도 1당 추가로 더해질 피해량 (예: 1.5 => 방어도 10이면 +15 피해)")]
    public float extraDamagePerShield = 1.0f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[DefenseGainAndProportionalStrike] PlayerAbility를 찾을 수 없습니다.");

        descriptionTemplate = "The player gains {0} Defense, and for each Defense gained, deals an additional {1} damage to the nearest enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 1) 방어도(실드) 획득
        int gain = Mathf.Max(0, defenseGain);
        playerAbility.AddShield(gain);

        // 2) 공격 대상: 가장 가까운 적
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // 3) 치명타 판정 (네가 쓰던 규칙)
        bool isCriticalHit = Random.value < playerAbility.critChance;

        // 4) 기본 피해(블록) + 플레이어 공격력
        int baseBlockDamage = isCriticalHit ? CriticalHitDamage : AttackDamage;
        int baseDamage = baseBlockDamage + playerAbility.attackPower;

        // 5) 방금 획득한 방어도에 비례한 추가 피해
        int extraDamage = Mathf.RoundToInt(gain * extraDamagePerShield);

        // 6) 최종 피해 적용 (기본 피해 + 비례 추가 피해)
        int damageToDeal = baseDamage + extraDamage;
        enemy.TakeDamage(damageToDeal);

        if (isCriticalHit)
            Debug.Log($"[DefenseGainAndProportionalStrike] 방어도 +{gain}, 비례추가 {extraDamage} → CRIT! 총 {damageToDeal} 피해");
        else
            Debug.Log($"[DefenseGainAndProportionalStrike] 방어도 +{gain}, 비례추가 {extraDamage} → 총 {damageToDeal} 피해");
    }

    // 가장 x가 작은 적을 찾는 유틸
    private GameObject FindClosestEnemy()
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
        // Description만 변경 (다른 값 변경 금지)
        Description = $"방어도를 {defenseGain} 획득하고, 획득량 × {extraDamagePerShield} 만큼 추가 피해를 더해 가장 가까운 적을 공격합니다.";
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, defenseGain, extraDamagePerShield);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains {0} Defense, and for each Defense gained, deals an additional {1} damage to the nearest enemy.";
        UpdateDescription();
    }
}
