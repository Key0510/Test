using UnityEngine;

public class RandomAttackOrShieldBlock : BlockAbility
{
    [Header("증감 수치 (인스펙터에서 수정 가능)")]
    public int attackPowerIncrease = 5;  // 공격력 증가량
    public int shieldIncrease = 10;      // 쉴드 증가량

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
        {
            Debug.LogWarning("[PowerOrShieldAbility] PlayerAbility를 찾을 수 없습니다.");
        }
        descriptionTemplate = "Increases the player’s Attack Power by {0} or increases Defense by {1}.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 50% 확률 (Random.value < 0.5f)
        if (Random.value < 0.5f)
        {
            int before = playerAbility.attackPower;
            playerAbility.attackPower += attackPowerIncrease;
            Debug.Log($"[PowerOrShieldAbility] 공격력 +{attackPowerIncrease} ({before} → {playerAbility.attackPower})");
        }
        else
        {
            playerAbility.AddShield(shieldIncrease);
            Debug.Log($"[PowerOrShieldAbility] 쉴드 +{shieldIncrease} (현재 쉴드: {playerAbility.shield})");
        }
    }

    public override void Upgrade()
    {
        // Description만 업데이트
        Description = $"50% 확률로 공격력 +{attackPowerIncrease}, 그렇지 않으면 쉴드 +{shieldIncrease} 증가";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackPowerIncrease, shieldIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Increases the player’s Attack Power by {0} or increases Defense by {1}.";
        UpdateDescription();
    }
}
