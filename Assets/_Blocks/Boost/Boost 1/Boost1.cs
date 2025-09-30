// 버프1
using UnityEngine;

public class Boost1 : BlockAbility
{
    [Header("능력 설정")]
    public int attackDamageIncrease = 1; // 증가시킬 공격력 수치

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
            Debug.LogWarning("[AttackDamageIncreaseBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player’s Attack Power increases by {0}.";
    }

    public override void Execute()
    {
        if (playerAbility != null)
        {
            int beforeAttack = playerAbility.attackPower; // 증가 전 공격력
            playerAbility.AddAttackPower(attackDamageIncrease);

        }
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description = $"플레이어의 공격력을 {attackDamageIncrease} 증가시킵니다.";
    }
    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamageIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Attack Power increases by {0}.";
        UpdateDescription();
    }
}
