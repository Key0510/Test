// 버프9
using UnityEngine;

public class Boost9 : BlockAbility
{
    [Header("공격력 증가 범위 설정")]
    public int minIncrease = 1; // 최소 증가량
    public int maxIncrease = 5; // 최대 증가량

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
            Debug.LogWarning("[RandomAttackPowerIncreaseBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        descriptionTemplate = "The player gains a random amount of Attack Power between {0} and {1}.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 랜덤 증가량 계산
        int randomIncrease = Random.Range(minIncrease, maxIncrease + 1); // max 포함

        // 플레이어 공격력 증가
        int before = playerAbility.attackPower;
        playerAbility.AddAttackPower(randomIncrease);

        Debug.Log($"[RandomAttackPowerIncreaseBlockAbility] 공격력 +{randomIncrease} ({before} ▶ {playerAbility.attackPower})");
    }

    public override void Upgrade()
    {
        Description = $"플레이어의 공격력을 {minIncrease}~{maxIncrease} 중 랜덤한 값만큼 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, minIncrease, maxIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains a random amount of Attack Power between {0} and {1}.";
        UpdateDescription();
    }
}
