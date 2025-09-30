using UnityEngine;

// 크리1
public class CritChanceUpBlockAbility : BlockAbility
{
    [Header("치명타 확률 증가 설정")]
    [Range(0f, 1f)]
    public float critChanceIncrease = 0.02f; // 기본 2%p 증가 (0.02)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
        {
            Debug.LogWarning("[CritChanceUpBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        }
        descriptionTemplate = "The player’s Critical Hit Chance increases by {0}.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("[CritChanceUpBlockAbility] 실행 불가: PlayerAbility 미할당");
            return;
        }

        // 치명타 확률 증가 및 0~1 범위 클램프
        float before = playerAbility.critChance;
        playerAbility.AddCritChance(critChanceIncrease);

        Debug.Log($"[치명타 확률 상승] {before * 100f:0.#}% → {playerAbility.critChance * 100f:0.#}% (+{critChanceIncrease * 100f:0.#}%)");
    }

    public override void Upgrade()
    {
        // Description만 변경 (스탯 변경 X)
        Description = $"블록 파괴 시 플레이어의 치명타 확률이 {critChanceIncrease * 100f:0.#}% 증가합니다.";
    }
    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, critChanceIncrease);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player’s Critical Hit Chance increases by {0}.";
        UpdateDescription();
    }
}
