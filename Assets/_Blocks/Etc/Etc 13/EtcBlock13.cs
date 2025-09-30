using UnityEngine;

public class EtcBlock13 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "가장 가까운 적에게 {0} 독 스택을 부여합니다.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 치명타 확률에 따라 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 기본 데미지 계산: 블록 데미지 + 플레이어 공격력
                int baseDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                // "Block" 태그 오브젝트 확인
                GameObject[] blockObjects = GameObject.FindGameObjectsWithTag("Block");
                int damageToDeal = blockObjects.Length == 0 ? baseDamage * 2 : baseDamage;

                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 입힘 (기본: {baseDamage}{(blockObjects.Length == 0 ? ", Block 태그 없음으로 2배" : "")}). (잔여 체력: {enemy.currentHealth})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 입힘 (기본: {baseDamage}{(blockObjects.Length == 0 ? ", Block 태그 없음으로 2배" : "")}). (잔여 체력: {enemy.currentHealth})");
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, Block 태그 오브젝트가 없으면 데미지가 두 배가 되며, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "가장 가까운 적에게 {0} 독 스택을 부여합니다.";
        UpdateDescription();
    }
}