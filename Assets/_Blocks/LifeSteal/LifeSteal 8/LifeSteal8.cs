using UnityEngine;

public class LifeSteal8 : BlockAbility
{
    [SerializeField] private int playerDamage = 15; // 플레이어 피해 (인스펙터에서 설정)
    [SerializeField] private int enemyDamage = 10;  // 모든 적 피해 (인스펙터에서 설정)
    [SerializeField] private int criticalEnemyDamage = 20; // 치명타 시 적 피해 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} damage to the player and deals {1} + Player’s Attack Power damage to all enemies ({2} + Player’s Attack Power on a critical hit).";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        // 플레이어에게 피해
        playerAbility.TakeDamage(playerDamage);

        // 모든 적에게 피해
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int attackedEnemies = 0;

        // 치명타 확률에 따라 치명타 여부 결정
        bool isCriticalHit = Random.value < playerAbility.critChance;
        int damageToDeal = isCriticalHit ? criticalEnemyDamage : enemyDamage;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damageToDeal + playerAbility.attackPower);
                attackedEnemies++;
            }
        }

        // 디버그 로그
        if (isCriticalHit)
        {
            Debug.Log($"치명타! 플레이어에게 {playerDamage} 데미지를 입히고, {attackedEnemies}명의 적에게 각각 {damageToDeal} 데미지를 줌.");
        }
        else
        {
            Debug.Log($"플레이어에게 {playerDamage} 데미지를 입히고, {attackedEnemies}명의 적에게 각각 {damageToDeal} 데미지를 줌.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어에게 {0} 데미지를 입히고 씬의 모든 적에게 {1}(치명타 시 {2}) 데미지를 주며, {3} 방어력을 얻음.",
            playerDamage, enemyDamage, criticalEnemyDamage, Defense);
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, playerDamage, enemyDamage, criticalEnemyDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to the player and deals {1} + Player’s Attack Power damage to all enemies ({2} + Player’s Attack Power on a critical hit).";
        UpdateDescription();
    }
}