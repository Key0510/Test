using UnityEngine;

public class RandomGoldBlockAbility : BlockAbility
{
    [Header("골드 설정")]
    public int goldGainAmount = 25;   // 획득할 골드
    public int goldLoseAmount = 10;   // 잃을 골드 (양수로 작성)
    public float chance = 0.5f;       // 골드 획득 확률 (0.0 ~ 1.0)

    private MoneyManager moneyManager;

    void Start()
    {
        moneyManager = FindObjectOfType<MoneyManager>();
        descriptionTemplate = "The player loses {0} gold or gains {1} gold.";
    }

    public override void Execute()
    {
        if (moneyManager == null)
        {
            Debug.LogWarning("MoneyManager가 존재하지 않습니다.");
            return;
        }

        bool gainGold = Random.value < chance;

        if (gainGold)
        {
            moneyManager.AddMoney(goldGainAmount);
            Debug.Log($"🎉 골드 {goldGainAmount} 획득!");
        }
        else
        {
            // SpendMoney는 성공 여부 반환하므로 확인 가능
            bool success = moneyManager.SpendMoney(goldLoseAmount);
            if (success)
                Debug.Log($"💸 골드 {goldLoseAmount} 잃음!");
            else
                Debug.Log("💸 잃을 만큼의 골드가 없어서 변화 없음!");
        }
    }

    public override void Upgrade()
    {
        Description = $"50% 확률로 골드를 {goldGainAmount} 획득하거나, {goldLoseAmount}를 잃습니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, goldLoseAmount, goldGainAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player loses {0} gold or gains {1} gold.";
        UpdateDescription();
    }
}
