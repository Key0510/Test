using UnityEngine;

public class Poison5 : BlockAbility
{
    [SerializeField] private int poisonStackAmount = 5; // 독 스택 추가량 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "If the nearest enemy has 1 or more Poison stacks, deal {0} + Player’s Attack Power damage to that enemy ({1} + Player’s Attack Power on a critical hit). If the enemy has no Poison stacks, apply {2} Poison stacks instead.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (enemy.poison > 0)
                {
                    // 치명타 확률에 따라 치명타 여부 결정
                    bool isCriticalHit = Random.value < playerAbility.critChance;
                    // 데미지 계산: 블록 데미지 + 플레이어 공격력
                    int damageToDeal = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;
                    enemy.TakeDamage(damageToDeal);
                    // 디버그 로그
                    if (isCriticalHit)
                    {
                        Debug.Log($"치명타! {closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (독 스택: {enemy.poison})");
                    }
                    else
                    {
                        Debug.Log($"{closestEnemy.name}에게 {damageToDeal} 데미지를 줌. (독 스택: {enemy.poison})");
                    }
                }
                else
                {
                    // 독 스택이 없으면 설정된 양만큼 추가
                    enemy.poison += poisonStackAmount;
                    Debug.Log($"{closestEnemy.name}에게 {poisonStackAmount} 독 스택을 추가함. 총 독 스택: {enemy.poison}");
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
        Description = string.Format("독이 있는 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주거나, 가장 가까운 적에게 독 스택 {2}개를 적용하고 {3} 방어력을 얻음.", AttackDamage, CriticalHitDamage, poisonStackAmount, Defense); // 설명 업데이트
    }
                    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, poisonStackAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "If the nearest enemy has 1 or more Poison stacks, deal {0} + Player’s Attack Power damage to that enemy ({1} + Player’s Attack Power on a critical hit). If the enemy has no Poison stacks, apply {2} Poison stacks instead.";
        UpdateDescription();
    }
}