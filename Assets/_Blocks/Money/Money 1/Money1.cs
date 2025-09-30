using UnityEngine;

public class Money1 : BlockAbility
{
    [SerializeField] protected int moneyAmount = 10;

    void Start()
    {
      descriptionTemplate = "The player gains {0} gold.";   
    }
    public override void Execute()
    {
        if (MoneyManager.Instance != null)
        {
            MoneyManager.Instance.AddMoney(moneyAmount);
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("Gains {0} money and gains {1} defense.", moneyAmount, Defense); // Description 업데이트
    }
                        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, moneyAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains {0} gold.";
        UpdateDescription();
    }
}