using UnityEngine;

public class Money6 : BlockAbility
{
    [SerializeField] private float executeHealthRatio = 0.1f; // 처형 조건 체력 비율 (인스펙터에서 설정)
    [SerializeField] private int executeMoneyReward = 10; // 처형 시 돈 획득량 (인스펙터에서 설정)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the enemy’s HP is {2} or lower, instantly execute them and gain {3} gold.";
    }

    public override void Execute()
    {
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

                // 사전 계산: 데미지 적용 후 잔여 체력 예측
                int remainingShield = (int)Mathf.Max(0, enemy.currentShield - damageToDeal);
                int overflowDamage = (int)Mathf.Max(0, damageToDeal - enemy.currentShield);
                int remainingHealth = (int)Mathf.Max(0, enemy.currentHealth - overflowDamage);

                // 처형 조건: 잔여 체력이 설정된 비율 이하이거나 데미지로 죽음
                bool willExecute = (remainingHealth <= (int)(enemy.maxHealth * executeHealthRatio));

                // 데미지 적용
                enemy.TakeDamage(damageToDeal);

                // 처형 조건 충족 시 돈 획득
                if (willExecute && MoneyManager.Instance != null)
                {
                    enemy.Die();
                    MoneyManager.Instance.AddMoney(executeMoneyReward);
                    Debug.Log($"처형! {closestEnemy.name}에게 {damageToDeal} 데미지를 주고 처형(체력 {executeHealthRatio:P0} 이하 또는 사망)하여 {executeMoneyReward} 돈을 획득함.");
                }
                else
                {
                    // 디버그 로그 (처형 안 됨)
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌 (잔여 체력: {remainingHealth}).");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌 (잔여 체력: {remainingHealth}).");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 잔여 체력이 {2:P0} 이하이거나 죽으면 처형으로 간주해 {3} 돈을 획득하며, {4} 방어력을 얻음.", AttackDamage, CriticalHitDamage, executeHealthRatio, executeMoneyReward, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, executeHealthRatio, executeMoneyReward);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the enemy’s HP is {2} or lower, instantly execute them and gain {3} gold.";
        UpdateDescription();
    }
}