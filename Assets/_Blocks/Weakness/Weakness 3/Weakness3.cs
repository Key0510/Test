using UnityEngine;

public class Weakness3 : BlockAbility
{
    [SerializeField] private int weaknessAmount = 3; // 약화 수치 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Applies {0} Weakness stacks to all enemies.";
    }

    public override void Execute()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int affectedEnemies = 0;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.weakness += weaknessAmount; // 약화 적용
                enemy.UpdateWeaknessUI(); // 약화 UI 업데이트
                affectedEnemies++;
            }
        }

        if (affectedEnemies > 0)
        {
            Debug.Log($"{affectedEnemies}명의 적에게 각각 {weaknessAmount} 약화 스택을 적용함.");
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 약화를 적용하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적에게 {0} 약화 스택을 적용하고, {1} 방어력을 얻음.", weaknessAmount, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Applies {0} Weakness stacks to all enemies.";
        UpdateDescription();
    }
}