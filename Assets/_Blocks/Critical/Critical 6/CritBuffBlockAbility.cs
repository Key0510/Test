using UnityEngine;

public class CritBuffBlockAbility : BlockAbility
{
    [Header("ġ  Ÿ    ġ  Ÿ Ȯ            ")]
    [Range(0f, 1f)]
    public float critChanceIncrease = 0.08f; // ġ  Ÿ           ġ  Ÿ Ȯ   (0.08 = 8%)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
        {
            Debug.LogWarning("[CritBuffBlockAbility] PlayerAbility   ã           ϴ .");
        }
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player’s Critical Hit Chance increases by {2}.";
    }

    public override void Execute()
    {
        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy == null || playerAbility == null) return;

        Enemy enemy = closestEnemy.GetComponent<Enemy>();
        if (enemy == null) return;

        // ġ  Ÿ           ( ÷  ̾         ġ  Ÿ Ȯ      )
        bool isCriticalHit = Random.value < playerAbility.critChance;
        int damageToDeal = isCriticalHit ? CriticalHitDamage : AttackDamage;

        //            
        enemy.TakeDamage(damageToDeal + playerAbility.attackPower);

        // ġ  Ÿ  ߻     ġ  Ÿ Ȯ       
        if (isCriticalHit)
        {
            float before = playerAbility.critChance;
            playerAbility.AddCritChance(critChanceIncrease);
            Debug.Log($"[CritBuffBlock] ġ  Ÿ  ߻ ! ġ  Ÿ Ȯ   {before * 100f:0.#}%    {playerAbility.critChance * 100f:0.#}% (+{critChanceIncrease * 100f:0.#}%)");
        }
    }

    //      x  ǥ           ã  
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
        // Description        (                 )
        Description = $"        ġ  Ÿ    ߻  ϸ  ġ  Ÿ Ȯ     {critChanceIncrease * 100f:0.#}%      մϴ . " +
                      $"(    : {AttackDamage}, ġ  Ÿ     : {CriticalHitDamage})";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage, critChanceIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The nearest enemy takes {0} + Player’s Attack Power damage ({1} + Player’s Attack Power on a critical hit). If a critical hit occurs, the player’s Critical Hit Chance increases by {2}.";
        UpdateDescription();
    }
}
