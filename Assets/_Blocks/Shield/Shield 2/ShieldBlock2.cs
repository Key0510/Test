using UnityEngine;

public class ShieldBlock2 : BlockAbility
{
    private PlayerAbility playerAbility;

    public float defenseMultiplier = 2;
    private void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The player’s Defense is multiplied by {0}.";
    }

    public override void Execute()
    {
        if (playerAbility != null)
        {
            float currentShield = playerAbility.shield;
            float doubledShield = currentShield * defenseMultiplier;

            // 현재 보호막 값을 직접 2배 값으로 세팅
            playerAbility.shield = (int)doubledShield;

            // UI 갱신 메서드 직접 호출
            playerAbility.UpdateShieldUI();

            Debug.Log($"ShieldBlock2: 보호막이 기존 {currentShield}에서 {doubledShield}로 두 배가 되었습니다.");
        }
        else
        {
            Debug.LogWarning("PlayerAbility를 찾을 수 없습니다.");
        }
    }

    public override void Upgrade()
    {
        // level 증가 없이 upgradeDescription만 변경
        upgradeDescription = "ShieldBlock2가 업그레이드되었습니다! 다음 단계에서는 보호막 증폭 효과가 더 커집니다.";
        Debug.Log(upgradeDescription);
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, defenseMultiplier);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Defense is multiplied by {0}.";
        UpdateDescription();
    }
}

