// 크리8
using UnityEngine;

/// <summary>
/// 사용 시 플레이어의 치명타 확률을 감소시키고,
/// 이 블록의 공격력(AttackDamage)을 증가시키는 능력.
/// </summary>
public class CritAttackIncreaseBlockAbility : BlockAbility
{
    [Header("수치 설정")]
    [Tooltip("플레이어 치명타 확률 감소량 (0.05 = 5%)")]
    [Range(0f, 1f)]
    public float critChanceDecrease = 0.05f;

    [Tooltip("이 블록의 공격력 증가량")]
    public int blockAttackIncrease = 5;

    private PlayerAbility playerAbility;
    private StackBlockManager stackBlockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        stackBlockManager = FindObjectOfType<StackBlockManager>();
        if (playerAbility == null)
            Debug.LogWarning("[CritDownBlockPowerUp] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "";
    }

    public override void Execute()
    {
        // 1) 플레이어 치명타 확률 감소 (0~1 범위로 자동 클램프)
        if (playerAbility != null)
        {
            float beforeCrit = playerAbility.critChance;

            if (playerAbility.critChance < critChanceDecrease)
            {

            }
            else
            {
                playerAbility.AddCritChance(-Mathf.Abs(critChanceDecrease));
                Debug.Log($"[CritDownBlockPowerUp] 치명타 확률 {beforeCrit:P0} → {playerAbility.critChance:P0} (↓{critChanceDecrease * 100f:F0}%)");
                stackBlockManager.Critical8Stack += blockAttackIncrease;
            }

        }

        GameObject closestEnemy = FindClosestEnemy();
        Enemy enemy = closestEnemy.GetComponent<Enemy>();

        enemy.TakeDamage(stackBlockManager.Critical8Stack + playerAbility.attackPower);

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
        // Description만 변경 (스탯은 유지)
        Description = $"사용 시 치명타 확률 {critChanceDecrease * 100f:F0}% 감소, 이 블록의 공격력 {blockAttackIncrease} 증가.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, critChanceDecrease, blockAttackIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage. The player loses {1} Critical Hit Chance, but this block’s Attack Power permanently increases by {2}.";
        UpdateDescription();
    }
}
