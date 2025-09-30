using UnityEngine;

public class Weakness7 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} damage to the nearest enemy. On a Critical Hit, deals {1} damage. Removes the enemy’s Weakness stacks and gains money equal to the stacks removed.";
    }

    public override void Execute()
    {
        if (playerAbility == null || MoneyManager.Instance == null)
        {
            Debug.LogWarning("PlayerAbility 또는 MoneyManager가 없습니다.");
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

                // 약화 스택 기반 돈 획득
                int moneyToAdd = (int)enemy.weakness;
                if (moneyToAdd > 0)
                {
                    MoneyManager.Instance.AddMoney(moneyToAdd);
                }

                // 공격
                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (moneyToAdd > 0)
                {
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 주고, 약화 스택 {moneyToAdd}로 {moneyToAdd} 돈을 획득, 약화 스택을 0으로 초기화.");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 주고, 약화 스택 {moneyToAdd}로 {moneyToAdd} 돈을 획득, 약화 스택을 0으로 초기화.");
                    }
                }
                else
                {
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (약화 스택 0, 돈 획득 없음)");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (약화 스택 0, 돈 획득 없음)");
                    }
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 약화 스택만큼 돈을 획득한 뒤 약화 스택을 0으로 초기화하며, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to the nearest enemy. On a Critical Hit, deals {1} damage. Removes the enemy’s Weakness stacks and gains money equal to the stacks removed.";
        UpdateDescription();
    }
}