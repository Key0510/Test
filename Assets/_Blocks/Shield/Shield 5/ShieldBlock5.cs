using UnityEngine;

public class ShieldBlock5 : BlockAbility
{
    [Header("방어도 설정값")]
    [Tooltip("첫 번째 방어도 값 (예: 0)")]
    public int zeroDefense = 0;

    [Tooltip("두 번째 방어도 값 (예: 10)")]
    public int tenDefense = 10;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[RandomZeroOrTenDefenseBlockAbility] PlayerAbility를 찾을 수 없습니다.");

        descriptionTemplate = "The player gains either {0} Defense or {1} Defense.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 50% 확률로 방어도 선택
        int defenseToGain = (Random.value < 0.5f) ? zeroDefense : tenDefense;

        // 플레이어 방어도 추가
        playerAbility.AddShield(defenseToGain);

        Debug.Log($"[RandomZeroOrTenDefenseBlockAbility] 방어도 +{defenseToGain} 획득");
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description = $"방어도를 {zeroDefense} 또는 {tenDefense} 중 하나를 무작위로 획득합니다.";
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, zeroDefense, tenDefense);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains either {0} Defense or {1} Defense.";
        UpdateDescription();
    }
}
