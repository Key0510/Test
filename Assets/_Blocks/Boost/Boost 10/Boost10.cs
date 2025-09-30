// 버프10
using UnityEngine;

public class Boost10 : BlockAbility
{
    [Header("적 처치 시 증가할 공격력")]
    public int attackPowerIncrease = 10; // 인스펙터에서 수정 가능

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[KillBoostAttackPowerBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the enemy is killed by this block, the player gains {2} Attack Power.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;

                // 피해 계산: 블록 기본 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                // 적 체력 저장 (죽었는지 판별용)
                int beforeHP = (int)enemy.currentHealth;

                // 데미지 적용
                enemy.TakeDamage(damageToDeal);

                // 적 처치 확인
                if (beforeHP > 0 && enemy.currentHealth <= 0)
                {
                    playerAbility.AddAttackPower(attackPowerIncrease);
                    Debug.Log($"[KillBoostAttackPowerBlockAbility] 적 처치! 플레이어 공격력 +{attackPowerIncrease} ▶ 현재 공격력: {playerAbility.attackPower}");
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
        Description = $"이 블록으로 적을 처치하면 플레이어 공격력이 {attackPowerIncrease} 증가합니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, attackPowerIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the enemy is killed by this block, the player gains {2} Attack Power.";
        UpdateDescription();
    }
}
