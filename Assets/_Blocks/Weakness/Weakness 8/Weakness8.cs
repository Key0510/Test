using UnityEngine;

public class Weakness8 : BlockAbility
{
    [SerializeField] private int weaknessAmount = 3; // 약화 수치 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Applies {0} Weakness stack(s) to the enemy with the highest attack power.";
    }
    public override void Execute()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Debug.Log("적을 찾을 수 없어 약화를 적용하지 못함.");
            return;
        }

        // 가장 공격력이 높은 적 찾기
        Enemy highestAttackEnemy = null;
        int highestAttackPower = int.MinValue;
        GameObject targetEnemyObj = null;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null && enemy.attackPower > highestAttackPower)
            {
                highestAttackPower = (int)enemy.attackPower;
                highestAttackEnemy = enemy;
                targetEnemyObj = enemyObj;
            }
        }

        if (highestAttackEnemy != null)
        {
            highestAttackEnemy.weakness += weaknessAmount; // 약화 적용
            highestAttackEnemy.UpdateWeaknessUI(); // 약화 UI 업데이트
            Debug.Log($"{targetEnemyObj.name} (공격력: {highestAttackPower})에게 {weaknessAmount} 약화 스택을 적용함. 총 약화 스택: {highestAttackEnemy.weakness}");
        }
        else
        {
            Debug.Log("유효한 적을 찾을 수 없어 약화를 적용하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적 중 가장 공격력이 높은 적에게 {0} 약화 스택을 적용하고, {1} 방어력을 얻음.", weaknessAmount, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Applies {0} Weakness stack(s) to the enemy with the highest attack power.";
        UpdateDescription();
    }
}