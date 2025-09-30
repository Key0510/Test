using UnityEngine;

public class Money10 : BlockAbility
{
    [SerializeField] private int moneyAmount = 100; // 돈 획득량 (인스펙터에서 설정)
    [SerializeField] private float durationMultiplier = 3; // Duration 배율 (인스펙터에서 설정)

    private Block block;
    private BlockManager blockManager;

    void Awake()
    {
        block = GetComponent<Block>();
        blockManager = FindObjectOfType<BlockManager>();


        descriptionTemplate = "This block has {0}× duration. Grants {1} gold.";
    }

    public override void Execute()
    {
        if (MoneyManager.Instance == null)
        {
            Debug.LogWarning("MoneyManager가 없습니다.");
            return;
        }

        // 돈 추가
        MoneyManager.Instance.AddMoney(moneyAmount);
        Debug.Log($"Money10: {moneyAmount} 돈을 획득함. 현재 Duration: {(block != null ? block.Duration : 0)}");
    }

    public override void Upgrade()
    {
        Description = string.Format("{0} 돈을 획득하고, Duration이 기본 내구도의 {1}배이며, {2} 방어력을 얻음.", moneyAmount, durationMultiplier, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, durationMultiplier, moneyAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "This block has {0}× duration. Grants {1} gold.";
        UpdateDescription();
    }
}