using UnityEngine;

public class Weakness4 : BlockAbility
{
    [SerializeField] private float weaknessMultiplier = 2f; // 약화 스택 배율 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Multiplies all enemies’ Weakness stacks by {0}.";
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
                int newWeakness = Mathf.FloorToInt(enemy.weakness * weaknessMultiplier); // 약화 스택 배율 적용
                enemy.weakness = newWeakness;
                enemy.UpdateWeaknessUI(); // 약화 UI 업데이트
                affectedEnemies++;
                Debug.Log($"{enemyObj.name}의 약화 스택을 {weaknessMultiplier}배로 증가. 총 약화 스택: {enemy.weakness}");
            }
        }

        if (affectedEnemies > 0)
        {
            Debug.Log($"{affectedEnemies}명의 적의 약화 스택을 {weaknessMultiplier}배로 증가함.");
        }
        else
        {
            Debug.Log("적을 찾을 수 없어 약화 스택을 증가시키지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적의 약화 스택을 {0}배로 증가시키고, {1} 방어력을 얻음.", weaknessMultiplier, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessMultiplier);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Multiplies all enemies’ Weakness stacks by {0}.";
        UpdateDescription();
    }
}