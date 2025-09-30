using UnityEngine;

public class RandomCritChanceBlockAbility : BlockAbility
{
    [Header("치명타 확률 증감 설정")]
    public float critChanceIncrease = 0.1f;   // 증가할 치명타 확률 (예: +10%)
    public float critChanceDecrease = -0.05f; // 감소할 치명타 확률 (예: -5%)
    public float chance = 0.5f;               // 치명타 확률 증가 확률

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The player’s Critical Hit Chance increases by {0} or decreases by {1}.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility를 찾을 수 없습니다.");
            return;
        }

        bool shouldIncrease = Random.value < chance;

        if (shouldIncrease)
        {
            playerAbility.AddCritChance(critChanceIncrease);

            Debug.Log($" 치명타 확률이 {critChanceIncrease * 100}% 증가하여 현재 {playerAbility.critChance * 100}%가 되었습니다.");
        }
        else
        {
            playerAbility.AddCritChance(critChanceDecrease);

            Debug.Log($" 치명타 확률이 {Mathf.Abs(critChanceDecrease) * 100}% 감소하여 현재 {playerAbility.critChance * 100}%가 되었습니다.");
        }
    }

    public override void Upgrade()
    {
        Description = $"50% 확률로 치명타 확률이 {critChanceIncrease * 100}% 증가하거나 {Mathf.Abs(critChanceDecrease * 100)}% 감소합니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, critChanceIncrease, critChanceDecrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Critical Hit Chance increases by {0} or decreases by {1}.";
        UpdateDescription();
    }
}
