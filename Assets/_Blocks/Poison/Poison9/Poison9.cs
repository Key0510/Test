using UnityEngine;

public class Poison9 : BlockAbility
{
    [SerializeField] private int poisonStackAmount = 3; // 뒤에 있는 적들에게 추가할 독 스택량 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). Then, applies {2} Poison stacks to all other enemies.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy frontEnemy = closestEnemy.GetComponent<Enemy>();
            if (frontEnemy != null)
            {
                // 치명타 확률에 따라 가장 앞의 적 공격
                bool isCriticalHit = Random.value < playerAbility.critChance;
                // 데미지 계산: 블록 데미지 + 플레이어 공격력
                int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;
                frontEnemy.TakeDamage(damageToDeal);

                // 앞의 적보다 뒤에 있는 적들에게 독 스택 추가
                float frontX = closestEnemy.transform.position.x;
                GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (GameObject enemyObj in enemies)
                {
                    if (enemyObj.transform.position.x > frontX)
                    {
                        Enemy enemy = enemyObj.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.poison += poisonStackAmount;
                            Debug.Log($"앞의 적 뒤에 있는 {enemyObj.name}에게 {poisonStackAmount} 독 스택을 추가함. 총 독 스택: {enemy.poison}");
                        }
                    }
                }

                // 디버그 로그
                if (isCriticalHit)
                {
                    Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌.");
                }
                else
                {
                    Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌.");
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
        Description = string.Format("가장 앞의 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 뒤에 있는 모든 적에게 독 스택 {2}개를 적용하며, {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, poisonStackAmount, Defense); // 설명 업데이트
    }
                    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, poisonStackAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to the nearest enemy ({1} + Player’s Attack Power on a critical hit). Then, applies {2} Poison stacks to all other enemies.";
        UpdateDescription();
    }
}