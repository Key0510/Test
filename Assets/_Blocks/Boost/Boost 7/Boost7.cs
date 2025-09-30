// 버프7
using UnityEngine;

public class Boost7 : BlockAbility
{
    [Header("능력 설정")]
    [Tooltip("방어도의 몇 %를 공격력으로 전환할지 (예: 10 = 10%)")]
    public float defensePercent = 10f;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
            Debug.LogWarning("[DefensePercentToAttackBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player consumes all Defense and gains Attack Power equal to {0} times the amount of Defense consumed.";
    }

    public override void Execute()
    {
        if (playerAbility != null)
        {
            // 현재 플레이어의 방어도(Shield)를 가져와서 일정 비율만큼 공격력 증가
            int attackIncrease = Mathf.FloorToInt(playerAbility.shield * (defensePercent / 100f));
            playerAbility.AddAttackPower(attackIncrease);

            Debug.Log($"[DefensePercentToAttackBlockAbility] 플레이어 방어도 {playerAbility.shield} → 공격력 +{attackIncrease}, 현재 공격력: {playerAbility.attackPower}");
        }
    }

    public override void Upgrade()
    {
        Description = $"현재 플레이어 방어도의 {defensePercent}% 만큼 플레이어 공격력을 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, defensePercent);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player consumes all Defense and gains Attack Power equal to {0} times the amount of Defense consumed.";
        UpdateDescription();
    }
}
