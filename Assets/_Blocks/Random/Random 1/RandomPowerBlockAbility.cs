// 랜덤 1
using UnityEngine;

public class RandomPowerBlockAbility : BlockAbility
{
    [Header("변경 수치")]
    [SerializeField] private int attackDamageChangeAmount = 5; // 블록 공격력 증감량
    [SerializeField] private int playerAttackChangeAmount = 3; // 플레이어 공격력 증감량

    [Header("확률")]
    [Range(0f, 1f)]
    [SerializeField] private float increaseChance = 0.5f; // 증가할 확률 (나머지는 감소)

    private PlayerAbility playerAbility;
    private StackBlockManager stackBlockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        stackBlockManager = FindObjectOfType<StackBlockManager>();

        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility not found.");
        }

        if (stackBlockManager == null)
        {
            Debug.LogWarning("StackBlockManager not found.");
            return;
        }
        descriptionTemplate = "This block’s Attack Power increases or decreases by {0}, or the player’s Attack Power increases or decreases by {1}.";


    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility not found.");
            return;
        }

        if (stackBlockManager == null)
        {
            Debug.LogWarning("StackBlockManager not found.");
            return;
        }

        // 0: 블록 공격력, 1: 플레이어 공격력
        int statToChange = Random.Range(0, 2);
        bool shouldIncrease = Random.value < increaseChance;

        if (statToChange == 0)
        {
            // 블록 공격력 변경
            int change = shouldIncrease ? attackDamageChangeAmount : -attackDamageChangeAmount;
            stackBlockManager.SelfAttackStack += change;
            if (stackBlockManager.SelfAttackStack < 0)
                stackBlockManager.SelfAttackStack = 0;

            Debug.Log($"[RandomPowerBlockAbility] 블록 공격력 {(shouldIncrease ? "증가" : "감소")} → AttackDamage: {AttackDamage}, SelfAttackStack: {stackBlockManager.SelfAttackStack}");
        }
        else
        {
            // 플레이어 공격력 변경
            int change = shouldIncrease ? playerAttackChangeAmount : -playerAttackChangeAmount;
            playerAbility.AddAttackPower(change);
            Debug.Log($"[RandomPowerBlockAbility] 플레이어 공격력 {(shouldIncrease ? "증가" : "감소")} → attackPower: {playerAbility.attackPower}");
        }

        GameObject closestEnemy = FindClosestEnemy();
        Enemy enemy = closestEnemy.GetComponent<Enemy>();

        enemy.TakeDamage(stackBlockManager.SelfAttackStack + playerAbility.attackPower);
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
        Description = string.Format("무작위로 블록 공격력(±{0}) 또는 플레이어 공격력(±{1})을 변경합니다. 증가 확률: {2}%. (현재 SelfAttackStack: {3})",
            attackDamageChangeAmount, playerAttackChangeAmount, increaseChance * 100, stackBlockManager != null ? stackBlockManager.SelfAttackStack : 0);
    }
    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamageChangeAmount, playerAttackChangeAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "This block’s Attack Power increases or decreases by {0}, or the player’s Attack Power increases or decreases by {1}.";
        UpdateDescription();
    }
}