using UnityEngine;

public class ShieldBlock4 : BlockAbility
{
    [Header("방어도 랜덤 범위 설정")]
    [Tooltip("획득할 방어도 최소값 (포함)")]
    public int minDefenseGain = 3;

    [Tooltip("획득할 방어도 최대값 (포함)")]
    public int maxDefenseGain = 7;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
            Debug.LogWarning("[RandomDefenseGainBlockAbility] PlayerAbility를 찾을 수 없습니다.");

        descriptionTemplate = "The player gains a random amount of Defense between {0} and {1}.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 잘못된 입력 방지: 최소/최대 보정
        if (minDefenseGain > maxDefenseGain)
        {
            int tmp = minDefenseGain;
            minDefenseGain = maxDefenseGain;
            maxDefenseGain = tmp;
        }

        // Unity int Random.Range는 상한 미포함 → +1
        int gain = Random.Range(minDefenseGain, maxDefenseGain + 1);

        // 플레이어 방어도(실드) 추가
        playerAbility.AddShield(gain);

        Debug.Log($"[RandomDefenseGainBlockAbility] 방어도 +{gain} (범위: {minDefenseGain}~{maxDefenseGain})");
    }

    public override void Upgrade()
    {
        // Description만 변경 (다른 값 변경 X)
        Description = $"방어도를 {minDefenseGain}~{maxDefenseGain} 중 랜덤한 값만큼 획득합니다.";
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, minDefenseGain, maxDefenseGain);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains a random amount of Defense between {0} and {1}.";
        UpdateDescription();
    }
}
