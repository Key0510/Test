using UnityEngine;

public class ShieldBlock7 : BlockAbility
{
    [Header("골드 → 실드 변환 설정")]
    [Tooltip("현재 골드의 몇 %를 실드로 전환할지 (예: 5 = 5%)")]
    [Min(0f)]
    public float percentOfGoldToShield = 5f;

    [Tooltip("한 번에 얻을 수 있는 실드의 최대값")]
    [Min(0)]
    public int maxShieldGain = 30;

    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();

        if (playerAbility == null)
            Debug.LogWarning("[GoldProportionalShieldBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        if (moneyManager == null)
            Debug.LogWarning("[GoldProportionalShieldBlockAbility] MoneyManager를 찾을 수 없습니다.");

        descriptionTemplate = "The player gains Defense equal to {0}% of their current gold (up to a maximum of {1}).";
    }

    public override void Execute()
    {
        if (playerAbility == null || moneyManager == null) return;

        // 1) 현재 골드 확인
        int currentGold = moneyManager.GetMoney();

        // 2) 비례 실드 계산: (골드 × 퍼센트) → 정수 반올림
        int proportionalShield = Mathf.RoundToInt(currentGold * (percentOfGoldToShield / 100f));

        // 3) 최대치 적용(Clamp)
        int gain = Mathf.Clamp(proportionalShield, 0, maxShieldGain);

        // 4) 실드 지급
        playerAbility.AddShield(gain);

        Debug.Log($"[GoldProportionalShield] 골드:{currentGold}, 비례실드:{proportionalShield} → 지급:{gain} (최대 {maxShieldGain})");
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description = $"현재 골드의 {percentOfGoldToShield}%만큼 방어도를 얻지만, 한 번에 최대 {maxShieldGain}까지만 획득합니다.";
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, percentOfGoldToShield, maxShieldGain);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains Defense equal to {0}% of their current gold (up to a maximum of {1}).";
        UpdateDescription();
    }
}
