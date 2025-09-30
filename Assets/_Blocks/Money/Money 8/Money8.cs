using UnityEngine;

public class Money8 : BlockAbility
{
    [SerializeField] private float shieldToMoneyRatio = 1.0f; // 방어도 전환 비율 (인스펙터에서 설정, 기본 100%)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Consume all Defense and gain gold equal to {0} of the Defense consumed.";
    }

    public override void Execute()
    {
        if (MoneyManager.Instance == null || playerAbility == null)
        {
            Debug.LogWarning("MoneyManager 또는 PlayerAbility가 없습니다.");
            return;
        }

        // 플레이어 방어도에서 일정 비율 소진 및 돈으로 전환
        int shieldAmount = playerAbility.shield;
        if (shieldAmount > 0)
        {
            int shieldToConvert = Mathf.FloorToInt(shieldAmount * shieldToMoneyRatio); // 전환할 방어도 계산
            playerAbility.shield = Mathf.Max(0, shieldAmount - shieldToConvert); // 방어도 감소
            playerAbility.UpdateShieldUI(); // UI 업데이트
            MoneyManager.Instance.AddMoney(shieldToConvert); // 전환된 방어도만큼 돈 추가
            Debug.Log($"방어도 {shieldToConvert}({shieldToMoneyRatio:P0})을 소진하고 {shieldToConvert} 돈을 획득함. (남은 방어도: {playerAbility.shield})");
        }
        else
        {
            Debug.Log("방어도가 0이어서 돈을 획득하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어의 방어도를 {0:P0} 소진하고 그 수치만큼 돈으로 전환하며, {1} 방어력을 얻음.", shieldToMoneyRatio, Defense); // 설명 업데이트
    }
                            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, shieldToMoneyRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Consume all Defense and gain gold equal to {0} of the Defense consumed.";
        UpdateDescription();
    }
}