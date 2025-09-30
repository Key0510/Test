using UnityEngine;

public class Poison4 : BlockAbility
{
    [SerializeField] private float poisonStackMultiplier = 2f; // 독 스택 배율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Multiplies the nearest enemy’s Poison stacks by {0}.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                int newPoison = Mathf.FloorToInt(enemy.poison * poisonStackMultiplier); // 독 스택 배율 적용
                enemy.poison = newPoison;
                Debug.Log($"가장 가까운 적 {closestEnemy.name}의 독 스택을 {poisonStackMultiplier}배로 증가. 총 독 스택: {enemy.poison}");
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
        Description = string.Format("가장 가까운 적의 독 스택을 {0}배로 증가시키고, {1} 방어력을 얻음.", poisonStackMultiplier, Defense); // 설명 업데이트
    }
                    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, poisonStackMultiplier);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Multiplies the nearest enemy’s Poison stacks by {0}.";
        UpdateDescription();
    }
}