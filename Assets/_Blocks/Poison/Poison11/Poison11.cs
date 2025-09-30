using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Poison11 : BlockAbility
{
    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If that enemy is killed by this block, transfer their Poison stacks to the enemy behind them.";
    }

    public override void Execute()
    {
        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjs.Length == 0) return;

        // x-position으로 적 정렬 (오름차순: 가장 가까운 적부터)
        List<GameObject> sortedEnemies = enemyObjs.OrderBy(e => e.transform.position.x).ToList();

        GameObject closestEnemy = sortedEnemies[0];
        Enemy closest = closestEnemy.GetComponent<Enemy>();
        if (closest != null)
        {
            // 치명타 확률에 따라 치명타 여부 결정
            bool isCriticalHit = Random.value < playerAbility.critChance;
            // 데미지 계산: 블록 데미지 + 플레이어 공격력
            int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

            // 적이 죽을지 확인 (쉴드 + 체력 <= 데미지)
            bool willDie = (closest.currentShield + closest.currentHealth) <= damageToDeal;
            int transferredPoison = 0;

            if (willDie)
            {
                transferredPoison = closest.poison; // 죽기 전에 poison 값 저장
            }

            // 실제 데미지 적용
            closest.TakeDamage(damageToDeal);

            // 죽었으면 다음 적에게 poison 전달
            if (willDie && sortedEnemies.Count > 1)
            {
                GameObject nextEnemy = sortedEnemies[1];
                Enemy next = nextEnemy.GetComponent<Enemy>();
                if (next != null)
                {
                    next.poison += transferredPoison;
                    Debug.Log($"가장 가까운 적 {closestEnemy.name}이 죽어 {transferredPoison} 독 스택을 다음 적 {nextEnemy.name}에게 이전함. 총 독 스택: {next.poison}");
                }
            }
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 만약 그 적이 죽으면 해당 적의 독 스택을 다음 가까운 적에게 이전하며, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
                    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If that enemy is killed by this block, transfer their Poison stacks to the enemy behind them.";
        UpdateDescription();
    }
}