using UnityEngine;

public class EtcBlock9 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the furthest enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject farthestEnemy = FindFarthestEnemy();
        if (farthestEnemy != null)
        {
            Enemy enemy = farthestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 치명타 확률에 따라 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 데미지 계산: 블록 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! 가장 멀리 있는 {farthestEnemy.name}에게 {damageToDeal} 데미지를 입힘. (잔여 체력: {enemy.currentHealth})");
                }
                else
                {
                    Debug.Log($"가장 멀리 있는 {farthestEnemy.name}에게 {damageToDeal} 데미지를 입힘. (잔여 체력: {enemy.currentHealth})");
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
        }
    }

    public GameObject FindFarthestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject farthestEnemy = null;
        float maxX = Mathf.NegativeInfinity;
                foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyComp = enemy.GetComponent<Enemy>();
                if (enemyComp != null && !enemyComp.IsDead())
                {
                    float xPos = enemy.transform.position.x;
                    if (xPos > maxX)
            {
                maxX = xPos;
                farthestEnemy = enemy;
            }
                }
            }
        }

        return farthestEnemy;
    }

    public override void Upgrade()
    {
        Description = string.Format("가장 멀리 있는 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 입히고, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the furthest enemy.";
        UpdateDescription();
    }
}