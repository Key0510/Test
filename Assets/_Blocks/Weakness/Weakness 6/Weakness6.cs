using UnityEngine;

public class Weakness6 : BlockAbility
{
    [SerializeField] private float recoveryRatio = 0.5f; // 피해량 회복 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} damage plus the player’s attack power to the nearest enemy. On a Critical Hit, deals {1} plus attack power. If the enemy has 1 or more Weakness stacks, restores {2} of the damage as HP.";
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
                // 데미지 계산: 블록 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                enemy.TakeDamage(damageToDeal);

                // 약화 스택이 1 이상이라면 피해량의 설정된 비율만큼 플레이어 체력 회복
                if (enemy.weakness >= 1)
                {
                    int recoveryAmount = Mathf.FloorToInt(damageToDeal * recoveryRatio);
                    playerAbility.Heal(recoveryAmount);
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 주고, 약화 스택 {enemy.weakness}으로 {recoveryAmount} 체력을 회복함. (비율: {recoveryRatio:P0})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (약화 스택: {enemy.weakness})");
                }
            }
        }
        else
        {
            Debug.Log("적을 찾을 수 없음.");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 약화 스택이 1 이상이라면 피해량의 {2:P0}만큼 플레이어 체력을 회복하며, {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, recoveryRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage,criticalHitDamage, recoveryRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage plus the player’s attack power to the nearest enemy. On a Critical Hit, deals {1} plus attack power. If the enemy has 1 or more Weakness stacks, restores {2} of the damage as HP.";
        UpdateDescription();
    }
}