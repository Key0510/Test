using UnityEngine;

public class LifeSteal4 : BlockAbility
{
    private PlayerAbility playerAbility;
    public float currenthealthRatio = 0.5f;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the player’s current HP is {2} or less of their max HP, the attack is guaranteed to be a critical hit.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null && playerAbility != null)
            {
                bool isCriticalHit;

                // 플레이어 체력이 절반 이하라면 무조건 치명타
                if (playerAbility.currentHealth <= playerAbility.maxHealth * currenthealthRatio)
                {
                    isCriticalHit = true;
                }
                else
                {
                    isCriticalHit = Random.value < playerAbility.critChance;
                }

                // 데미지 계산: 블록 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (플레이어 체력: {playerAbility.currentHealth}/{playerAbility.maxHealth})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (플레이어 체력: {playerAbility.currentHealth}/{playerAbility.maxHealth})");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 플레이어 체력이 절반 이하라면 무조건 치명타가 적용되며, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, currenthealthRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the player’s current HP is {2} or less of their max HP, the attack is guaranteed to be a critical hit.";
        UpdateDescription();
    }
}