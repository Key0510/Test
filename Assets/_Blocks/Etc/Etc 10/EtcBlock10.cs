using UnityEngine;

public class EtcBlock10 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to both the frontmost and rearmost enemies.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
            return;
        }

        // 가장 앞과 가장 뒤 적 찾기
        GameObject closestEnemy = null;
        GameObject farthestEnemy = null;
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;

        foreach (GameObject enemy in enemies)
        {
            float xPos = enemy.transform.position.x;
            if (xPos < minX)
            {
                minX = xPos;
                closestEnemy = enemy;
            }
            if (xPos > maxX)
            {
                maxX = xPos;
                farthestEnemy = enemy;
            }
        }

        // 치명타 확률에 따라 치명타 여부 결정
        bool isCriticalHit = Random.value < playerAbility.critChance;
        // 데미지 계산: 블록 데미지 + 플레이어 공격력
        int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

        int affectedEnemies = 0;
        string logMessage = isCriticalHit ? "치명타! " : "";

        // 가장 앞의 적 공격
        if (closestEnemy != null)
        {
            Enemy closest = closestEnemy.GetComponent<Enemy>();
            if (closest != null)
            {
                closest.TakeDamage(damageToDeal);
                affectedEnemies++;
                logMessage += $"가장 앞의 {closestEnemy.name}에게 {damageToDeal} 데미지를 입힘. (잔여 체력: {closest.currentHealth})";
            }
        }

        // 가장 뒤의 적 공격 (중복 확인)
        if (farthestEnemy != null && farthestEnemy != closestEnemy)
        {
            Enemy farthest = farthestEnemy.GetComponent<Enemy>();
            if (farthest != null)
            {
                farthest.TakeDamage(damageToDeal);
                affectedEnemies++;
                logMessage += farthestEnemy != closestEnemy ? $" | 가장 뒤의 {farthestEnemy.name}에게 {damageToDeal} 데미지를 입힘. (잔여 체력: {farthest.currentHealth})" : "";
            }
        }

        if (affectedEnemies > 0)
        {
            Debug.Log(logMessage);
        }
        else
        {
            Debug.Log("유효한 적을 찾을 수 없어 데미지를 입히지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("가장 앞과 가장 뒤의 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 입히고, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to both the frontmost and rearmost enemies.";
        UpdateDescription();
    }
}