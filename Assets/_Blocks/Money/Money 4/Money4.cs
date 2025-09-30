using UnityEngine;

public class Money4 : BlockAbility
{
    [SerializeField] private int minMoneyAmount = 1; // 최소 돈 획득량 (인스펙터에서 설정)
    [SerializeField] private int maxMoneyAmount = 20; // 최대 돈 획득량 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "The player gains a random amount of gold between {0} and {1}.";
    }

    public override void Execute()
    {
        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning("MoneyManager가 없습니다.");
            return;
        }

        // minMoneyAmount ~ maxMoneyAmount 사이의 무작위 돈 획득
        int moneyAmount = Random.Range(minMoneyAmount, maxMoneyAmount + 1);
        MoneyManager.Instance.AddMoney(moneyAmount);
        Debug.Log($"무작위로 {moneyAmount} 돈을 획득함. (범위: {minMoneyAmount}~{maxMoneyAmount})");
    }

    public override void Upgrade()
    {
        Description = string.Format("{0}~{1} 사이의 무작위 돈을 획득하고, {2} 방어력을 얻음.", minMoneyAmount, maxMoneyAmount, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, minMoneyAmount, maxMoneyAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains a random amount of gold between {0} and {1}.";
        UpdateDescription();
    }
}