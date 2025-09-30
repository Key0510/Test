using UnityEngine;

public class LifeSteal6 : BlockAbility
{
    [SerializeField] private int playerDamage = 10; // 플레이어 피해 (인스펙터에서 설정)
    [SerializeField] private int maxHealthIncrease = 1; // 최대 체력 증가 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} damage to the player and increases the player’s Max HP by {1}";
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
        // 최대 체력 증가
        playerAbility.maxHealth += maxHealthIncrease;
        playerAbility.UpdateHealthUI(); // UI 업데이트

        Debug.Log($"플레이어에게 {playerDamage} 데미지를 입히고 최대 체력을 {maxHealthIncrease} 증가함. (현재 최대 체력: {playerAbility.maxHealth}, 현재 체력: {playerAbility.currentHealth})");
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어에게 {0} 데미지를 입히고 최대 체력을 {1} 증가시키며, {2} 방어력을 얻음.", playerDamage, maxHealthIncrease, Defense); // 설명 업데이트
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, playerDamage, maxHealthIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to the player and increases the player’s Max HP by {1}";
        UpdateDescription();
    }
}