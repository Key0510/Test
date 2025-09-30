// 흡혈7
using UnityEngine;

public class LifeSteal7 : BlockAbility
{
    [SerializeField] private int playerDamage = 10; // 플레이어 피해 (인스펙터에서 설정)
    [SerializeField] private int selfAttackIncrease = 1; // 공격력 증가 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;
    private StackBlockManager stackBlockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        stackBlockManager = FindObjectOfType<StackBlockManager>();
        descriptionTemplate = "Deals {0} damage to the player and increases this block’s Attack Power by {1}.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        if (stackBlockManager == null)
        {
            Debug.LogWarning("StackBlockManager가 없습니다.");
            return;
        }

        // 플레이어에게 피해
        playerAbility.TakeDamage(playerDamage);

        // StackBlockManager의 Boost2Stack 증가
        stackBlockManager.LifeStea7Stack += selfAttackIncrease;

        GameObject closestEnemy = FindClosestEnemy();
        Enemy enemy = closestEnemy.GetComponent<Enemy>();

        enemy.TakeDamage(stackBlockManager.LifeStea7Stack + playerAbility.attackPower);



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
        Description = string.Format("플레이어에게 데미지를 입히고 블록 공격력을 영구히 증가시킴."); // 설명 업데이트
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, playerDamage, selfAttackIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to the player and increases this block’s Attack Power by {1}.";
        UpdateDescription();
    }
}