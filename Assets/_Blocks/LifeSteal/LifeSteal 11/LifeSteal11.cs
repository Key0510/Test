using UnityEngine;

public class LifeSteal11 : BlockAbility
{
    [SerializeField] private float healthThresholdRatio = 0.3f; // 체력 임계 비율 (30%, 인스펙터에서 설정)
    [SerializeField] private int weaknessAmount = 5; // 약화 스택 양 (인스펙터에서 설정)
    [SerializeField] private int poisonAmount = 5; // 독 스택 양 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the player’s current HP is {2} or less of their Max HP, apply {3} Poison and {4} Weakness.";
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

                // 플레이어 체력이 설정된 비율 이하라면 약화와 독 스택 증가
                if (playerAbility.currentHealth <= playerAbility.maxHealth * healthThresholdRatio)
                {
                    enemy.weakness += weaknessAmount;
                    enemy.poison += poisonAmount;
                    enemy.UpdateWeaknessUI(); // 약화 UI 업데이트

                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 주고, 플레이어 체력 {healthThresholdRatio:P0} 이하로 {weaknessAmount} 약화 스택과 {poisonAmount} 독 스택을 추가함. (총 약화: {enemy.weakness}, 총 독: {enemy.poison})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (플레이어 체력 {playerAbility.currentHealth}/{playerAbility.maxHealth})");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 플레이어 체력이 {2:P0} 이하라면 {3} 약화 스택과 {4} 독 스택을 추가하며, {5} 방어력을 얻음.", AttackDamage, CriticalHitDamage, healthThresholdRatio, weaknessAmount, poisonAmount, Defense); // 설명 업데이트
    }
    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, healthThresholdRatio, poisonAmount, weaknessAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). If the player’s current HP is {2} or less of their Max HP, apply {3} Poison and {4} Weakness.";
        UpdateDescription();
    }
}