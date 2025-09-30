using UnityEngine;

public class Poison7 : BlockAbility
{
    [SerializeField] private float poisonStackMultiplier = 2f; // 독 스택 배율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "All enemies’ Poison stacks are multiplied by {0}.";
    }

    public override void Execute()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                int newPoison = Mathf.FloorToInt(enemy.poison * poisonStackMultiplier); // 독 스택 배율 적용
                enemy.poison = newPoison;
                Debug.Log($"{enemyObj.name}의 독 스택을 {poisonStackMultiplier}배로 증가. 총 독 스택: {enemy.poison}");
            }
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적의 독 스택을 {0}배로 증가시키고, {1} 방어력을 얻음.", poisonStackMultiplier, Defense); // 설명 업데이트
    }
                    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, poisonStackMultiplier);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "All enemies’ Poison stacks are multiplied by {0}.";
        UpdateDescription();
    }
}