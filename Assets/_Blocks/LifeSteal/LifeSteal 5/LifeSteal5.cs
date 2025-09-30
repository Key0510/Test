using UnityEngine;

public class LifeSteal5 : BlockAbility
{
    [SerializeField] private int playerDamage = 5; // 플레이어 피해 (인스펙터에서 설정)
    [SerializeField] private int shieldAmount = 5; // 방어도 추가 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} damage to the player and grants {1} Defense.";
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
        // 플레이어에게 방어도 추가
        playerAbility.AddShield(shieldAmount);

        Debug.Log($"플레이어에게 {playerDamage} 데미지를 입히고 방어도 {shieldAmount}를 추가함. (현재 방어도: {playerAbility.shield})");
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어에게 {0} 데미지를 입히고 방어도 {1}를 추가하며, {2} 방어력을 얻음.", playerDamage, shieldAmount, Defense); // 설명 업데이트
    }
                                public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, playerDamage, shieldAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} damage to the player and grants {1} Defense.";
        UpdateDescription();
    }
}