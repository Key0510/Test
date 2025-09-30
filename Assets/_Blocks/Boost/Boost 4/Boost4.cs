// 버프4
using UnityEngine;

public class Boost4 : BlockAbility
{
    [Header("증가 설정 (몇퍼센트 올릴지)")]
    [Range(0f, 100f)]
    public float percent = 5f;   // 현재 골드의 %만큼 플레이어 공격력 증가 (기본 5%)

    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();

        if (playerAbility == null)
            Debug.LogWarning("[MoneyToAttackBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        if (moneyManager == null)
            Debug.LogWarning("[MoneyToAttackBlockAbility] MoneyManager를 찾을 수 없습니다.");
        descriptionTemplate = "The player’s Attack Power increases by {0} for each unit of gold currently held.";
    }

    public override void Execute()
    {
        if (playerAbility == null || moneyManager == null) return;

        int currentGold = moneyManager.GetMoney();
        int attackGain = Mathf.RoundToInt(currentGold * (percent / 100f)); // 골드의 % → 공격력 가산치

        if (attackGain <= 0)
        {
            Debug.Log("[MoneyToAttackBlockAbility] 현재 골드가 적어 증가 없음.");
            return;
        }

        int before = playerAbility.attackPower;
        playerAbility.AddAttackPower(attackGain);

        Debug.Log($"[MoneyToAttackBlockAbility] 골드 {currentGold} 기준 공격력 +{attackGain} → {before} ▶ {playerAbility.attackPower}");
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description = $"현재 보유 골드의 {percent}%만큼 플레이어 공격력을 즉시 증가시킵니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, percent);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Attack Power increases by {0} for each unit of gold currently held.";
        UpdateDescription();
    }
}
