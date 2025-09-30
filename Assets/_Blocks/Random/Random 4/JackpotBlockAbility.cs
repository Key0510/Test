using UnityEngine;

public class JackpotBlockAbility : BlockAbility
{
    [Header("잭팟 보상 설정")]
    public int goldReward = 100;      // 잭팟 보상 골드 (인스펙터에서 수정 가능)
    public bool resetOnReward = true; // 보상 후 카운트 초기화 여부

    private MoneyManager moneyManager;
    private Floor floor;

    void Start()
    {
        moneyManager = FindObjectOfType<MoneyManager>();
        floor = FindObjectOfType<Floor>();

        if (moneyManager == null) Debug.LogWarning("[JackpotBlockAbility] MoneyManager를 찾을 수 없습니다.");
        if (floor == null) Debug.LogWarning("[JackpotBlockAbility] Floor를 찾을 수 없습니다.");
        descriptionTemplate = "If this block is broken 3 times in a row, gain {0} gold.";
    }

    public override void Execute()
    {
        // Floor.ExecuteBrokenBlockAbilities()에서 이미
        // - JackpotBlockAbility인지 체크하고 countJackpot 증가/초기화 처리함
        // 따라서 여기서는 그 결과만 보고 보상 지급만 하면 됨.
        if (moneyManager == null || floor == null) return;

        if (floor.countJackpot >= 3)
        {
            moneyManager.AddMoney(goldReward);
            Debug.Log($"[Jackpot] 연속 3개 성공! +{goldReward}G 지급");

            if (resetOnReward)
                floor.countJackpot = 0; // 다음 콤보를 위해 초기화
        }
        else
        {
            Debug.Log($"[Jackpot] 진행 상황: {floor.countJackpot}/3");
        }
    }

    public override void Upgrade()
    {
        // Description만 변경 (스탯/값 변경 없음)
        Description = $"이 블록을 연속으로 3개 파괴하면 {goldReward} 골드를 획득합니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, goldReward);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "If this block is broken 3 times in a row, gain {0} gold.";
        UpdateDescription();
    }
}
