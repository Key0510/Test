using UnityEngine;

public class LifeSteal10 : BlockAbility
{
    [SerializeField] private int playerDamage = 10; // 플레이어 피해 (인스펙터에서 설정)
    [SerializeField] private int moneyAmount = 20; // 골드 획득량 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} damage to the player and grants {1} gold.";
    }

    public override void Execute()
    {
        if (playerAbility == null || MoneyManager.Instance == null)
        {
            Debug.LogWarning("PlayerAbility 또는 MoneyManager가 없습니다.");
            return;
        }

        // 플레이어에게 피해
        playerAbility.TakeDamage(playerDamage);
        // 골드 획득
        MoneyManager.Instance.AddMoney(moneyAmount);

        Debug.Log($"플레이어에게 {playerDamage} 데미지를 입히고 {moneyAmount} 골드를 획득함.");
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어에게 {0} 데미지를 입히고 {1} 골드를 획득하며, {2} 방어력을 얻음.", playerDamage, moneyAmount, Defense); // 설명 업데이트
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, playerDamage, moneyAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to the player and grants {1} gold.";
        UpdateDescription();
    }
}