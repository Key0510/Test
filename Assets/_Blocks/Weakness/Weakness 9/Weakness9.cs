using UnityEngine;

public class Weakness9 : BlockAbility
{
    [SerializeField] private float weaknessDamageRatio = 1.0f; // 약화 스택을 피해로 전환하는 비율 (인스펙터에서 설정)

    private BlockManager blockManager;

    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals damage to all enemies equal to {0} times their Weakness stacks.";

    }

    public override void Execute()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int affectedEnemies = 0;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null && enemy.weakness > 0)
            {
                int damageToDeal = Mathf.FloorToInt(enemy.weakness * weaknessDamageRatio); // 약화 스택에 비율 적용
                enemy.TakeDamage(damageToDeal);
                enemy.UpdateWeaknessUI(); // 약화 UI 갱신 (상태 확인용)
                affectedEnemies++;
                Debug.Log($"{enemyObj.name}에게 약화 스택 {enemy.weakness}에 {weaknessDamageRatio:P0} 비율을 적용해 {damageToDeal} 데미지를 입힘.");
            }
        }

        if (affectedEnemies > 0)
        {
            Debug.Log($"{affectedEnemies}명의 적에게 각각 약화 스택의 {weaknessDamageRatio:P0}만큼 데미지를 입힘.");
        }
        else
        {
            Debug.Log("약화 스택이 있는 적을 찾을 수 없어 피해를 입히지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적에게 각자의 약화 스택의 {0:P0}만큼 데미지를 입히고, {1} 방어력을 얻음.", weaknessDamageRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessDamageRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals damage to all enemies equal to {0} times their Weakness stacks.";
        UpdateDescription();
    }
}