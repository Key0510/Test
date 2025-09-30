using UnityEngine;

public class LifeSteal9 : BlockAbility
{
    [SerializeField] private int enemyDamage = 10; // 모든 적에게 주는 피해 (인스펙터에서 설정)
    [SerializeField] private int criticalEnemyDamage = 20; // 치명타 시 적에게 주는 피해 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to all enemies ({1} + Player’s Attack Power on a critical hit), and the player heals for the amount of damage dealt.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        // 모든 적에게 피해
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int attackedEnemies = 0;
        int totalDamageDealt = 0;

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
                totalDamageDealt += damageToDeal;
            }
        }

        // 입힌 총 데미지만큼 플레이어 체력 회복
        if (totalDamageDealt > 0)
        {
            playerAbility.Heal(totalDamageDealt);
        }

        // 디버그 로그
        if (isCriticalHit)
        {
            Debug.Log($"치명타! {attackedEnemies}명의 적에게 각각 {damageToDeal} 데미지를 주고, 플레이어 체력을 {totalDamageDealt} 회복함.");
        }
        else
        {
            Debug.Log($"{attackedEnemies}명의 적에게 각각 {damageToDeal} 데미지를 주고, 플레이어 체력을 {totalDamageDealt} 회복함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("씬의 모든 적에게 {0}(치명타 시 {1}) 데미지를 주고 입힌 총 데미지만큼 플레이어 체력을 회복하며, {2} 방어력을 얻음.",
            enemyDamage, criticalEnemyDamage, Defense);
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, enemyDamage, criticalEnemyDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + Player’s Attack Power damage to all enemies ({1} + Player’s Attack Power on a critical hit), and the player heals for the amount of damage dealt.";
        UpdateDescription();
    }
}