// 버프2
using UnityEngine;

public class Boost2 : BlockAbility
{
    [Header("공격력 증가 설정")]
    [SerializeField] private int selfAttackIncrease = 1; // 자기 객체 AttackDamage 증가량 (인스펙터에서 설정)

    private StackBlockManager stackBlockManager;
    private PlayerAbility playerAbility;


    void Start()
    {
        stackBlockManager = FindObjectOfType<StackBlockManager>();
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (stackBlockManager == null)
        {
            Debug.LogWarning("StackBlockManager를 찾을 수 없습니다.");
            return;
        }
        descriptionTemplate = "This block’s Attack Power permanently increases by {0}.";


    }

    public override void Execute()
    {
        if (stackBlockManager == null)
        {
            Debug.LogWarning("StackBlockManager가 없습니다.");
            return;
        }

        // StackBlockManager의 Boost2Stack 증가
        stackBlockManager.Boost2Stack += selfAttackIncrease;

        GameObject closestEnemy = FindClosestEnemy();
        Enemy enemy = closestEnemy.GetComponent<Enemy>();

        enemy.TakeDamage(stackBlockManager.Boost2Stack + playerAbility.attackPower);


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
        Description = string.Format("이 블록을 실행하여, 이 블록의 공격력을 영구히 {0} 증가시키며, {1} 방어력을 얻음. (현재 Boost2Stack: {2})",
            selfAttackIncrease, Defense, stackBlockManager != null ? stackBlockManager.Boost2Stack : 0);
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, selfAttackIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "This block’s Attack Power permanently increases by {0}.";
        UpdateDescription();
    }
}