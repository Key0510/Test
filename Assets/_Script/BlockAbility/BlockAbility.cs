using UnityEngine;

public abstract class BlockAbility : MonoBehaviour
{
    [SerializeField] protected string name;
    [SerializeField] protected int level = 1;
    [SerializeField] protected int attackDamage;
    [SerializeField] protected int defense;
    [SerializeField] protected int criticalHitDamage;
    [SerializeField] [TextArea] protected string descriptionTemplate; // 설명 템플릿
    [SerializeField] [TextArea] protected string upgradeDescriptionTemplate; // 업그레이드 설명 템플릿
    [SerializeField] [TextArea] protected string description; // 최종 설명
    [SerializeField] [TextArea] protected string upgradeDescription; // 최종 업그레이드 설명
    [SerializeField] protected GameObject nextLevelBlock; // 다음 레벨 프리팹

    public string Name { get => name; set => name = value; }
    public int Level { get => level; set => level = value; }
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }
    public int Defense { get => defense; set => defense = value; }
    public int CriticalHitDamage { get => criticalHitDamage; set => criticalHitDamage = value; }
    public string Description { get => description; set => description = value; }
    public string UpgradeDescription { get => upgradeDescription; set => upgradeDescription = value; }
    public GameObject NextLevelBlock { get => nextLevelBlock; set => nextLevelBlock = value; }

    public abstract void Execute();
    public abstract void Upgrade();

    // 선택적으로 오버라이드 가능한 설명 업데이트 메서드
    public virtual void UpdateDescription()
    {
        // 기본 구현: 템플릿이 있으면 그대로 사용, 없으면 description 유지
        if (!string.IsNullOrEmpty(descriptionTemplate))
        {
            description = descriptionTemplate;
        }
    }

    // Inspector에서 값이 변경될 때 호출
    protected virtual void OnValidate()
    {
        // 기본 템플릿 설정 (하위 클래스에서 오버라이드 가능)
        if (string.IsNullOrEmpty(descriptionTemplate))
        {
            descriptionTemplate = "기본 능력 설명";
        }
        if (string.IsNullOrEmpty(upgradeDescriptionTemplate))
        {
            upgradeDescriptionTemplate = "기본 업그레이드 설명";
        }
        UpdateDescription();
    }
}