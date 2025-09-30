using UnityEngine;

public class Money3 : BlockAbility
{
    [SerializeField] private float moneyBonusRatio = 0.3f; // 돈 기반 추가 데미지 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). Then deals additional damage equal to {2} of the player’s current gold.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null && MoneyManager.Instance != null)
            {
                // 치명타 확률에 따라 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 기본 데미지: 블록 데미지 + 플레이어 공격력
                int baseDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;
                // 추가 데미지: 현재 돈의 설정된 비율
                int moneyBonusDamage = Mathf.FloorToInt(MoneyManager.Instance.GetMoney() * moneyBonusRatio);
                // 최종 데미지
                int damageToDeal = baseDamage + moneyBonusDamage;

                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지(기본: {baseDamage}, 돈 보너스: {moneyBonusDamage}, 비율: {moneyBonusRatio:P0})를 줌.");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지(기본: {baseDamage}, 돈 보너스: {moneyBonusDamage}, 비율: {moneyBonusRatio:P0})를 줌.");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)에 현재 돈의 {2:P0}를 추가로 주고, {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, moneyBonusRatio, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, moneyBonusRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). Then deals additional damage equal to {2} of the player’s current gold.";
        UpdateDescription();
    }
}