using UnityEngine;

public class Money9 : BlockAbility
{
    [SerializeField] private int poisonMoneyReward = 10; // 독 스택 시 돈 획득량 (인스펙터에서 설정)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If that enemy has 1 or more Poison stacks, gain {2} gold.";
    }

    public override void Execute()
    {
        if (MoneyManager.Instance == null || playerAbility == null)
        {
            Debug.LogWarning("MoneyManager 또는 PlayerAbility가 없습니다.");
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

                // 독 스택 확인 및 돈 증가
                if (enemy.poison > 0)
                {
                    MoneyManager.Instance.AddMoney(poisonMoneyReward);
                    // 공격
                    enemy.TakeDamage(damageToDeal);
                    // 디버그 로그
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 독 스택 {enemy.poison}이 있어 {poisonMoneyReward} 돈을 획득하고 {damageToDeal} 데미지를 줌.");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 독 스택 {enemy.poison}이 있어 {poisonMoneyReward} 돈을 획득하고 {damageToDeal} 데미지를 줌.");
                    }
                }
                else
                {
                    // 독 스택 없음, 공격만
                    enemy.TakeDamage(damageToDeal);
                    // 디버그 로그
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 독 스택이 없어 {damageToDeal} 데미지만 줌.");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 독 스택이 없어 {damageToDeal} 데미지만 줌.");
                    }
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 적에게 독 스택이 있으면 {2} 돈을 획득하며, {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, poisonMoneyReward, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, poisonMoneyReward);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If that enemy has 1 or more Poison stacks, gain {2} gold.";
        UpdateDescription();
    }
}