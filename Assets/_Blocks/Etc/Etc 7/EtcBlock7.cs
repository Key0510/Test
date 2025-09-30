using UnityEngine;

public class EtcBlock7 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to all enemies.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int affectedEnemies = 0;

        // 치명타 확률에 따라 치명타 여부 결정
        bool isCriticalHit = Random.value < playerAbility.critChance;
        // 데미지 계산: 블록 데미지 + 플레이어 공격력
        int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageToDeal);
                affectedEnemies++;
            }
        }

        if (affectedEnemies > 0)
        {
            if (isCriticalHit)
            {
                Debug.Log($"치명타! {affectedEnemies}명의 적에게 각각 {damageToDeal} 데미지를 입힘.");
            }
            else
            {
                Debug.Log($"{affectedEnemies}명의 적에게 각각 {damageToDeal} 데미지를 입힘.");
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("씬의 모든 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 입히고, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to all enemies.";
        UpdateDescription();
    }
}