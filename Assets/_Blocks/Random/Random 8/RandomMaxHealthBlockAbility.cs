using UnityEngine;

public class RandomMaxHealthBlockAbility : BlockAbility
{
    [Header("�ִ� ü�� ���� ����")]
    public int maxHealthIncreaseAmount = 20;   // ������ (+)
    public int maxHealthDecreaseAmount = -10;  // ���ҷ� (���� ����)
    public float chance = 0.5f;                // ���� Ȯ�� (0.0 ~ 1.0)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The player’s Max HP increases by {0} or decreases by {1}.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility�� ã�� �� �����ϴ�.");
            return;
        }

        bool increase = Random.value < chance;

        if (increase)
        {
            playerAbility.maxHealth += maxHealthIncreaseAmount;
            playerAbility.currentHealth += maxHealthIncreaseAmount; // ��� ü�µ� �ݿ�
            Debug.Log($" �ִ� ü���� {maxHealthIncreaseAmount}��ŭ �����߽��ϴ�.");
        }
        else
        {
            playerAbility.maxHealth = Mathf.Max(1, playerAbility.maxHealth + maxHealthDecreaseAmount);
            playerAbility.currentHealth = Mathf.Min(playerAbility.currentHealth, playerAbility.maxHealth);
            Debug.Log($" �ִ� ü���� {Mathf.Abs(maxHealthDecreaseAmount)}��ŭ �����߽��ϴ�.");
        }

        // UI ����
        playerAbility.UpdateHealthUI();
    }

    public override void Upgrade()
    {
        Description = $"50% Ȯ���� �ִ� ü���� {maxHealthIncreaseAmount} �����ϰų� {Mathf.Abs(maxHealthDecreaseAmount)} �����մϴ�.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, maxHealthIncreaseAmount, maxHealthDecreaseAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Max HP increases by {0} or decreases by {1}.";
        UpdateDescription();
    }
}
