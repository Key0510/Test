// 버프6
using UnityEngine;

public class Boost6 : BlockAbility
{
    [Header("능력 설정")]
    [Tooltip("플레이어 공격력 감소 수치(음수로 표기)")]
    public int attackDecreaseAmount = -5;

    [Tooltip("감소된 공격력 기반 피해 증가 비율 (1.2 = 120%)")]
    public float damageMultiplier = 1.2f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
            Debug.LogWarning("[AttackTradeOffBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player consumes {0} Attack Power and deals {1} + Player’s Attack Power + {2} × the consumed Attack Power damage to the nearest enemy ({3} + Player’s Attack Power on a critical hit).";
    }

    public override void Execute()
    {
        if (playerAbility != null)
        {
            // 1. 플레이어 공격력 감소
            playerAbility.AddAttackPower(attackDecreaseAmount);
            if (playerAbility.attackPower < 0)
                playerAbility.attackPower = 0;

            // 2. 피해량 계산 (감소된 공격력 포함)
            bool isCriticalHit = Random.value < playerAbility.critChance;
            int baseDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;
            int finalDamage = Mathf.FloorToInt(baseDamage * damageMultiplier);

            // 3. 가장 가까운 적 공격
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Enemy enemy = closestEnemy.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(finalDamage);

                    Debug.Log($"[AttackTradeOffBlockAbility] 공격력 {attackDecreaseAmount} 감소 → 최종 피해 {finalDamage} (x{damageMultiplier}) 적용");

                    if (isCriticalHit)
                        Debug.Log("Critical Hit!");
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
        // Description만 변경
        Description = $"플레이어 공격력을 {attackDecreaseAmount} 감소시키고, 감소된 공격력을 기반으로 피해를 {damageMultiplier * 100}% 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDecreaseAmount, attackDamage, attackDecreaseAmount, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player consumes {0} Attack Power and deals {1} + Player’s Attack Power + {2} × the consumed Attack Power damage to the nearest enemy ({3} + Player’s Attack Power on a critical hit).";
        UpdateDescription();
    }
}
