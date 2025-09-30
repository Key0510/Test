using UnityEngine;

public class Poison2 : BlockAbility
{
    [SerializeField] private int poisonStackAmount = 3; // 독 스택 증가량 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Increases Poison stacks on all enemies by {0}.";
    }

    public override void Execute()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.poison += poisonStackAmount; // 설정된 양만큼 독 스택 증가
                Debug.Log($"Applied {poisonStackAmount} poison stacks to {enemyObj.name}. Total poison: {enemy.poison}");
            }
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적에게 {0} 독 스택을 적용하여 시간 경과에 따라 스택당 {1} 데미지를 주고, {2} 방어력을 얻음.", poisonStackAmount, AttackDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, poisonStackAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Increases Poison stacks on all enemies by {0}.";
        UpdateDescription();
    }
}