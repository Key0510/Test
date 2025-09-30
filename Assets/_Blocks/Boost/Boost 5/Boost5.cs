// 버프5
using UnityEngine;

public class Boost5 : BlockAbility
{
    [Header("능력 설정")]
    [Tooltip("잃은 체력 비율 (0.1 = 10%)")]
    public float lostHealthPercent = 0.1f; // 잃은 체력의 10%

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
            Debug.LogWarning("[LostHealthAttackBoostBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player’s Attack Power increases by {0} for each point of missing Health.";
    }

    public override void Execute()
    {
        if (playerAbility != null)
        {
            // 잃은 체력 계산
            int lostHealth = playerAbility.maxHealth - playerAbility.currentHealth;

            // 잃은 체력의 일정 비율만큼 공격력 증가
            int attackIncrease = Mathf.FloorToInt(lostHealth * lostHealthPercent);
            playerAbility.AddAttackPower(attackIncrease);

            Debug.Log($"[LostHealthAttackBoostBlockAbility] 잃은 체력 {lostHealth} → 공격력 {attackIncrease} 증가, 현재 공격력: {playerAbility.attackPower}");
        }
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description = $"잃은 체력의 {lostHealthPercent * 100}%만큼 공격력을 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, lostHealthPercent);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Attack Power increases by {0} for each point of missing Health.";
        UpdateDescription();
    }
}
