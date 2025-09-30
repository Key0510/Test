using UnityEngine;

public class ShieldToCritBlockAbility : BlockAbility
{
    [Header("방어막 → 치명타 확률 변환 설정")]
    [Range(0f, 1f)]
    public float shieldToCritRatio = 0.1f; // 잃은 방어막의 비율만큼 치명타 확률 증가 (0.1 = 10%)

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();

        if (playerAbility == null)
        {
            Debug.LogWarning("[ShieldToCritBlockAbility] PlayerAbility를 찾을 수 없습니다.");
        }
        descriptionTemplate = "The player loses all Defense, then increases their Critical Hit Chance by {0}% of the Defense lost.";
    }

    public override void Execute()
    {
        if (playerAbility == null) return;

        // 현재 방어막 값 저장
        int lostShield = playerAbility.shield;

        if (lostShield > 0)
        {
            // 방어막 제거
            playerAbility.shield = 0;
            playerAbility.UpdateShieldUI();

            // 증가할 치명타 확률 계산
            float critIncrease = lostShield * shieldToCritRatio / 100f; // % 변환

            float before = playerAbility.critChance;
            playerAbility.AddCritChance(critIncrease);

            Debug.Log($"[ShieldToCritBlock] 방어막 {lostShield} 소모, 치명타 확률 {before * 100f:0.#}% → {playerAbility.critChance * 100f:0.#}% (+{critIncrease * 100f:0.#}%)");
        }
        else
        {
            Debug.Log("[ShieldToCritBlock] 현재 방어막이 없어 효과 없음.");
        }
    }

    public override void Upgrade()
    {
        Description = $"현재 방어막을 모두 잃고, 잃은 방어막의 {shieldToCritRatio * 100f:0.#}% 만큼 치명타 확률을 얻습니다.";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, shieldToCritRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player loses all Defense, then increases their Critical Hit Chance by {0}% of the Defense lost.";
        UpdateDescription();
    }
}
