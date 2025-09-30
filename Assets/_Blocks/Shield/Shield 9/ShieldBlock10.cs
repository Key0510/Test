using UnityEngine;

public class ShieldBlock10 : BlockAbility
{
    private PlayerAbility playerAbility;

    [Header("설정")]
    [Tooltip("플레이어 공격력 대비 획득할 방어도 비율 (0~1 사이, 예: 0.5f = 공격력의 50% 만큼 방어도 획득)")]
    [SerializeField] private float shieldRatio = 0.5f;

    private void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        descriptionTemplate = "The player gains Defense equal to {0} of their Attack Power.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility를 찾을 수 없습니다.");
            return;
        }

        int attackPower = playerAbility.attackPower;

        // 공격력에 비례한 방어도 계산 (소수점 이하 내림)
        int shieldToAdd = Mathf.FloorToInt(attackPower * shieldRatio);

        if (shieldToAdd > 0)
        {
            playerAbility.AddShield(shieldToAdd);
            Debug.Log($"ShieldBlock10: 공격력 {attackPower}의 {shieldRatio * 100}%에 해당하는 방어도 {shieldToAdd} 획득! 현재 보호막: {playerAbility.shield}");
        }
        else
        {
            Debug.Log("ShieldBlock10: 공격력 대비 방어도 획득량이 0이어서 방어도 획득이 이루어지지 않았습니다.");
        }
    }

    public override void Upgrade()
    {
        upgradeDescription = "ShieldBlock10이 업그레이드되었습니다! 방어도 획득 비율이 조정될 수 있습니다.";
        Debug.Log(upgradeDescription);
    }
            public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, shieldRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "The player gains Defense equal to {0} of their Attack Power.";
        UpdateDescription();
    }
}
