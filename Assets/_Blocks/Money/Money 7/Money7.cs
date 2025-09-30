using UnityEngine;

public class Money7 : BlockAbility
{
    [Header("추가 피해 설정")]
    public int goldToSpend = 10;             // 공격 시 사용할 골드
    public float damagePerGold = 1.5f;       // 1골드당 추가 피해량

    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();

        if (playerAbility == null)
            Debug.LogWarning("[MoneyDamageBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        if (moneyManager == null)
            Debug.LogWarning("[MoneyDamageBlockAbility] MoneyManager를 찾을 수 없습니다.");
        descriptionTemplate = "Spend {0} gold and deal {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit).";
    }

    public override void Execute()
    {
        if (playerAbility == null || moneyManager == null)
            return;

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 치명타 판정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                int baseDamage = isCriticalHit ? CriticalHitDamage : AttackDamage;

                // 기본 피해량 = 기본 블록 피해 + 플레이어 공격력
                int damageToDeal = baseDamage + playerAbility.attackPower;

                // 골드가 충분한 경우 추가 피해 적용
                if (moneyManager.GetMoney() >= goldToSpend)
                {
                    // 골드 소모
                    moneyManager.SpendMoney(goldToSpend);

                    // 추가 피해 = 사용한 골드 × 피해 계수
                    int extraDamage = Mathf.RoundToInt(goldToSpend * damagePerGold);

                    // 최종 피해량에 추가 피해 포함
                    damageToDeal += extraDamage;

                    Debug.Log($"[MoneyDamageBlockAbility] 골드 {goldToSpend} 사용 → 추가 피해 {extraDamage}, 총 피해 {damageToDeal}");
                }
                else
                {
                    Debug.Log($"[MoneyDamageBlockAbility] 골드가 부족하여 기본 공격만 수행, 총 피해 {damageToDeal}");
                }

                // 피해 적용
                enemy.TakeDamage(damageToDeal);
            }
        }
    }

    private GameObject FindClosestEnemy()
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
        Description = $"공격 시 {goldToSpend} 골드를 사용하고, 1골드당 {damagePerGold}의 추가 피해를 줍니다.";
    }

    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, goldToSpend, damagePerGold);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Spend {0} gold and deal {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit).";
        UpdateDescription();
    }
}