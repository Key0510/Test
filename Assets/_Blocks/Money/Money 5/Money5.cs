using UnityEngine;

public class Money5 : BlockAbility
{
    [SerializeField] protected int baseMoneyAmount = 10; // 기본 돈 획득량
    [SerializeField] protected int critMoneyAmount = 20; // 치명타 시 돈 획득량

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The player gains {0} gold. If a critical hit occurs, the player instead gains {1} gold.";
    }

    public override void Execute()
    {
        if (MoneyManager.Instance != null && playerAbility != null)
        {
            // 치명타 확률에 따라 치명타 여부 결정
            bool isCriticalHit = Random.value < playerAbility.critChance;
            // 돈 획득량 계산
            int moneyToAdd = isCriticalHit ? critMoneyAmount : baseMoneyAmount;
            MoneyManager.Instance.AddMoney(moneyToAdd);

            // 디버그 로그
            if (isCriticalHit)
            {
                Debug.Log($"치명타! {moneyToAdd} 돈을 획득함.");
            }
            else
            {
                Debug.Log($"{moneyToAdd} 돈을 획득함.");
            }
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("기본 {0} 돈(치명타 시 {1} 돈)을 획득하고, {2} 방어력을 얻음.", baseMoneyAmount, critMoneyAmount, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, baseMoneyAmount, critMoneyAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains {0} gold. If a critical hit occurs, the player instead gains {1} gold.";
        UpdateDescription();
    }
}