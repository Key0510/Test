using UnityEngine;

public class EtcBlock3 : BlockAbility
{
    [SerializeField] private float shieldToHealthRatio = 0.5f; // 방어도에서 체력으로 전환하는 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Consumes all of the player's armor and restores health by {0:}.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        // 플레이어 방어도 소진 및 체력 회복
        int shieldAmount = playerAbility.shield;
        if (shieldAmount > 0)
        {
            playerAbility.shield = 0; // 방어도 모두 소진
            playerAbility.UpdateShieldUI(); // 방어도 UI 업데이트
            int healthToRecover = Mathf.FloorToInt(shieldAmount * shieldToHealthRatio); // 체력 회복량 계산
            playerAbility.Heal(healthToRecover); // 체력 회복
            Debug.Log($"방어도 {shieldAmount}을 소진하고 {shieldToHealthRatio:P0} 비율로 {healthToRecover} 체력을 회복함.");
        }
        else
        {
            Debug.Log("방어도가 0이어서 체력을 회복하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어의 모든 방어도를 소진하고 그 수치의 {0:}만큼 체력을 회복합니다, {1} 방어력을 얻음.", shieldToHealthRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, shieldToHealthRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Consumes all of the player's armor and restores health by {0:}.";
        UpdateDescription();
    }
}