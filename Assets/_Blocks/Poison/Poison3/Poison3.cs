using UnityEngine;

public class Poison3 : BlockAbility
{
    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If that enemy has 1 or more Poison stacks, the attack is guaranteed to be a critical hit.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                int damageToDeal;
                bool isCriticalHit;

                if (enemy.poison >= 1)
                {
                    isCriticalHit = true;
                }
                else
                {
                    isCriticalHit = Random.value < playerAbility.critChance;
                }

                // 데미지 계산: 블록 데미지 + 플레이어 공격력 (치명타 여부에 따라 다름)
                damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                enemy.TakeDamage(damageToDeal);
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지를 줌 ({1} + 플레이어 공격력 보장: 독 중독 적 또는 치명타 시). {2} 방어력 획득.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If that enemy has 1 or more Poison stacks, the attack is guaranteed to be a critical hit.";
        UpdateDescription();
    }
}