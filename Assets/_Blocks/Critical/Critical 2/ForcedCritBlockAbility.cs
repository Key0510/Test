using UnityEngine;

public class ForcedCritBlockAbility : BlockAbility
{
    [Header("블록 전용 치명타 설정")]
    [Range(0f, 1f)]
    public float critChance = 0.5f; // 이 블록만의 치명타 확률(기본 50%). 플레이어 critChance는 무시함.

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
        {
            Debug.LogWarning("[PowerOrShieldAbility] PlayerAbility를 찾을 수 없습니다.");
        }
        descriptionTemplate = " The player’s base Critical Hit Chance is ignored, and instead there is a {0}% chance to land a critical hit. The nearest enemy takes {1} + Player’s Attack Power damage ({2} + Player’s Attack Power on a critical hit).";
    }

    public override void Execute()
    {
        // 가장 왼쪽(또는 원하는 기준)의 적을 찾음
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // 플레이어의 critChance는 사용하지 않고, 이 블록의 critChance만 사용
        bool isCriticalHit = Random.value < critChance;
        int damageToDeal = isCriticalHit ? CriticalHitDamage : AttackDamage + playerAbility.attackPower;

        enemy.TakeDamage(damageToDeal);

        // 디버그 로그
        if (isCriticalHit)
            Debug.Log($"[ForcedCritBlock] 치명타! {damageToDeal} 데미지 (확률: {critChance * 100f:0.#}%)");
        else
            Debug.Log($"[ForcedCritBlock] 일반 공격 {damageToDeal} 데미지");
    }

    // 예시와 동일한 적 탐색 로직 (가장 x가 작은 적)
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
        // Description만 변경 (스탯 변경 없음)
        Description = $"플레이어 치명타 확률을 무시하고, {critChance * 100f:0.#}% 확률로 치명타({CriticalHitDamage}) 또는 일반({AttackDamage}) 공격을 합니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, critChance, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = " The player’s base Critical Hit Chance is ignored, and instead there is a {0}% chance to land a critical hit. The nearest enemy takes {1} + Player’s Attack Power damage ({2} + Player’s Attack Power on a critical hit).";
        UpdateDescription();
    }
}
