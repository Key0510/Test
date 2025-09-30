// 크리10
using UnityEngine;

public class GoldLossCritGainBlockAbility : BlockAbility
{
    [Header("능력 설정")]
    public int goldLossAmount = 10;          // 잃을 골드(고정값)
    public float critChanceIncrease = 0.05f; // 치명타 확률 증가 (0.05 = 5%)

    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();

        if (playerAbility == null)
            Debug.LogWarning("[GoldLossCritGainBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        if (moneyManager == null)
            Debug.LogWarning("[GoldLossCritGainBlockAbility] MoneyManager를 찾을 수 없습니다.");
        descriptionTemplate = "The player loses {0} gold and increases their Critical Hit Chance by {1}.";
    }

    public override void Execute()
    {
        if (moneyManager == null || playerAbility == null) return;

        int cost = Mathf.Max(0, goldLossAmount);
        int beforeGold = moneyManager.GetMoney();

        // 보유 금액이 부족하면 아무 것도 하지 않음
        if (beforeGold < cost)
        {
            Debug.Log($"[GoldLossCritGainBlockAbility] 골드 부족: 필요 {cost}, 보유 {beforeGold}. 치명타 증가 취소");
            return;
        }

        // 지불 시도
        bool paid = moneyManager.SpendMoney(cost);

        // 실패하거나 금액이 의도대로 줄지 않았으면 원상복구
        if (!paid || moneyManager.GetMoney() != beforeGold - cost)
        {
            int now = moneyManager.GetMoney();
            int diff = beforeGold - now;
            if (diff != 0)
            {
                if (diff > 0) moneyManager.AddMoney(diff);      // 덜어낸 만큼 되돌림
                else moneyManager.SpendMoney(-diff);    // 이상 증가했으면 다시 차감
            }
            Debug.Log($"[GoldLossCritGainBlockAbility] 결제 실패/오차 발생. 골드를 {beforeGold}로 복구. 치명타 증가 취소");
            return;
        }

        // 정상 지불 시 치명타 확률 증가
        float beforeCrit = playerAbility.critChance;
        playerAbility.AddCritChance(critChanceIncrease);
        Debug.Log($"[GoldLossCritGainBlockAbility] 골드 {cost} 지불. 치명타 확률 {beforeCrit:P0} → {playerAbility.critChance:P0}");
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description = $"골드 {goldLossAmount}를 지불하면 치명타 확률을 {critChanceIncrease * 100}% 얻습니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, goldLossAmount, critChanceIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player loses {0} gold and increases their Critical Hit Chance by {1}.";
        UpdateDescription();
    }
}
