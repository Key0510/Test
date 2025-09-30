using UnityEngine;

public class LifeSteal2 : BlockAbility
{
    private PlayerAbility playerAbility;
    public float mul = 20f;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit), and deals additional damage equal to {2} of the player’s missing HP ratio.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null && playerAbility != null)
            {
                // 치명타 확률에 따라 치명타 여부 결정
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 기본 데미지: 블록 데미지 + 플레이어 공격력
                int baseDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

                // 플레이어 체력 비율 계산 (체력이 낮을수록 보너스 증가)
                float healthRatio = 1f - ((float)playerAbility.currentHealth / playerAbility.maxHealth);
                // 보너스 데미지: 체력 비율 * 20 (예시, 조정 가능)
                int bonusDamage = Mathf.FloorToInt(healthRatio * mul);

                // 최종 데미지
                int damageToDeal = baseDamage + bonusDamage;

                enemy.TakeDamage(damageToDeal);

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지(기본: {baseDamage}, 보너스: {bonusDamage})를 줌. (체력 비율: {healthRatio:P0})");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지(기본: {baseDamage}, 보너스: {bonusDamage})를 줌. (체력 비율: {healthRatio:P0})");
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
        Description = string.Format("가장 가까운 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 플레이어 체력이 낮을수록 추가 데미지(최대 20)를 주며, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, mul);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit), and deals additional damage equal to {2} of the player’s missing HP ratio.";
        UpdateDescription();
    }
}