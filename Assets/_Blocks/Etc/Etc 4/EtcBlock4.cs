using UnityEngine;

public class EtcBlock4 : BlockAbility
{
    [SerializeField] private float lostHealthRecoveryRatio = 0.5f; // 잃은 체력 회복 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Restores health by {0} of the player's lost health.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        // 잃은 체력 계산
        int lostHealth = playerAbility.maxHealth - playerAbility.currentHealth;
        if (lostHealth > 0)
        {
            int healthToRecover = Mathf.FloorToInt(lostHealth * lostHealthRecoveryRatio); // 회복량 계산
            playerAbility.Heal(healthToRecover); // 체력 회복
            Debug.Log($"잃은 체력 {lostHealth}에 {lostHealthRecoveryRatio:P0} 비율을 적용해 {healthToRecover} 체력을 회복함. (현재 체력: {playerAbility.currentHealth})");
        }
        else
        {
            Debug.Log("잃은 체력이 없어 체력을 회복하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("플레이어의 잃은 체력의 {0}만큼 체력을 회복하고, {1} 방어력을 얻음.", lostHealthRecoveryRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, lostHealthRecoveryRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Restores health by {0} of the player's lost health.";
        UpdateDescription();
    }
}