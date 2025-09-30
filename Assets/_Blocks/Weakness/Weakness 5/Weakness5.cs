using UnityEngine;

public class Weakness5 : BlockAbility
{
    [SerializeField] private int poisonStackAmount = 3; // 약화 스택 1 이상인 적에게 적용할 독 스택 수치 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Applies {0} Poison stacks to all weakened enemies.";
    }

    public override void Execute()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int affectedEnemies = 0;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null && enemy.weakness >= 1)
            {
                enemy.poison += poisonStackAmount; // 독 스택 적용
                enemy.UpdateWeaknessUI(); // 약화 UI 갱신 (상태 확인용)
                affectedEnemies++;
                Debug.Log($"{enemyObj.name}에게 약화 스택 {enemy.weakness}이 있어 {poisonStackAmount} 독 스택을 적용함. 총 독 스택: {enemy.poison}");
            }
        }

        if (affectedEnemies > 0)
        {
            Debug.Log($"{affectedEnemies}명의 약화 스택 1 이상인 적에게 각각 {poisonStackAmount} 독 스택을 적용함.");
        }
        else
        {
            Debug.Log("약화 스택 1 이상인 적을 찾을 수 없어 독 스택을 적용하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("약화 스택이 1 이상인 모든 적에게 {0} 독 스택을 적용하고, {1} 방어력을 얻음.", poisonStackAmount, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, poisonStackAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Applies {0} Poison stacks to all weakened enemies.";
        UpdateDescription();
    }
}