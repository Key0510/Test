using UnityEngine;
using System.Linq;
using UnityEditor;

public class EtcBlock16 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the second closest enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length < 2)
        {
            Debug.Log("적이 2개 미만이라 두 번째로 가까운 적을 공격할 수 없음.");
            return;
        }

        // x-position으로 적 정렬 (가장 가까운 적부터)
        var sortedEnemies = enemies.OrderBy(e => e.transform.position.x).ToList();
        GameObject secondClosestEnemy = sortedEnemies[1]; // 두 번째로 가까운 적
        Enemy enemy = secondClosestEnemy.GetComponent<Enemy>();

        if (enemy != null && !enemy.IsDead())
        {
            // 치명타 확률에 따라 치명타 여부 결정
            bool isCriticalHit = Random.value < playerAbility.critChance;
            // 데미지 계산: 블록 데미지 + 플레이어 공격력
            int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

            enemy.TakeDamage(damageToDeal);

            // 디버그 로그
            if (isCriticalHit)
            {
                Debug.Log($"치명타! 두 번째로 가까운 {secondClosestEnemy.name}에게 {damageToDeal} 데미지를 입힘. (잔여 체력: {enemy.currentHealth})");
            }
            else
            {
                Debug.Log($"두 번째로 가까운 {secondClosestEnemy.name}에게 {damageToDeal} 데미지를 입힘. (잔여 체력: {enemy.currentHealth})");
            }
        }
        else
        {
            Debug.Log("두 번째로 가까운 적의 Enemy 컴포넌트를 찾을 수 없음.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("두 번째로 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 입히고, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the second closest enemy.";
        UpdateDescription();
    }
}