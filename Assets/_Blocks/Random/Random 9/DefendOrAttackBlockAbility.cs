using UnityEngine;

public class DefendOrAttackBlockAbility : BlockAbility
{
    [Header("확률 및 방어도 설정")]
    public int defenseGainAmount = 5;   // 얻는 방어도 수치
    public float chance = 0.5f;         // 방어도를 얻을 확률 (0.0 ~ 1.0)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit), or the player gains {2} Defense.";
    }

    public override void Execute()
    {
        // 확률 계산
        bool gainDefense = Random.value < chance;

        if (gainDefense)
        {
            if (playerAbility != null)
            {
                playerAbility.AddShield(defenseGainAmount);
                Debug.Log($" 플레이어가 {defenseGainAmount} 방어도를 획득했습니다.");
            }
        }
        else
        {
            GameObject closestEnemy = FindClosestEnemy();
            if (closestEnemy != null)
            {
                Enemy enemy = closestEnemy.GetComponent<Enemy>();
                if (enemy != null)
                {
                    bool isCritical = Random.value < playerAbility.critChance;
                    int damage = isCritical ? CriticalHitDamage : AttackDamage + playerAbility.attackPower;

                    enemy.TakeDamage(damage);
                    Debug.Log($" 적에게 {damage} 피해를 입혔습니다. (치명타: {isCritical})");
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
        Description = $"50% 확률로 방어도 {defenseGainAmount}를 얻거나, 적에게 {AttackDamage} 피해 (치명타 시 {CriticalHitDamage})를 입힙니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, defenseGainAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit), or the player gains {2} Defense.";
        UpdateDescription();
    }
}
