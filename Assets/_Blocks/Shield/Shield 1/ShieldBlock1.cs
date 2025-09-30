using UnityEngine;

public class ShieldBlock1 : BlockAbility
{
    private PlayerAbility playerAbility;

    private void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The player gains {0} Defense.";
    }

    public override void Execute()
    {
        if (playerAbility != null)
        {
            playerAbility.AddShield(defense);
            Debug.Log($"ShieldBlock1: 방어도 {defense} 획득! 현재 보호막: {playerAbility.shield}");
        }
        else
        {
            Debug.LogWarning("PlayerAbility를 찾을 수 없습니다.");
        }
    }

    public override void Upgrade()
    {
        // level 증가는 하지 않습니다.
        upgradeDescription = $"ShieldBlock1이 업그레이드되었습니다! 다음에는 더 강력한 방어도를 획득할 수 있습니다.";
        Debug.Log(upgradeDescription);
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, defense);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains {0} Defense.";
        UpdateDescription();
    }
}
