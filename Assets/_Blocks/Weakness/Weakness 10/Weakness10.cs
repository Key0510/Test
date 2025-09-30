using UnityEngine;

public class Weakness10 : BlockAbility
{
    [SerializeField] private float weaknessToShieldRatio = 1.0f; // 약화 스택을 방어도로 전환하는 비율 (인스펙터에서 설정)

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Gain Defense equal to {0} times the total Weakness stacks on all enemies.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int totalWeakness = 0;
        int affectedEnemies = 0;

        foreach (GameObject enemyObj in enemies)
        {
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            if (enemy != null)
            {
                totalWeakness += (int)enemy.weakness; // 약화 스택 합산
                affectedEnemies++;
            }
        }

        if (totalWeakness > 0)
        {
            int shieldToAdd = Mathf.FloorToInt(totalWeakness * weaknessToShieldRatio); // 비율 적용
            playerAbility.AddShield(shieldToAdd); // 방어도 추가
            Debug.Log($"{affectedEnemies}명의 적의 약화 스택 합계 {totalWeakness}에 {weaknessToShieldRatio:P0} 비율을 적용해 {shieldToAdd} 방어도를 획득함. (현재 방어도: {playerAbility.shield})");
        }
        else
        {
            Debug.Log("약화 스택이 있는 적을 찾을 수 없어 방어도를 획득하지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("모든 적의 약화 스택을 합산한 수치의 {0:P0}만큼 플레이어가 방어도를 얻고, {1} 방어력을 얻음.", weaknessToShieldRatio, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, weaknessToShieldRatio);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Gain Defense equal to {0} times the total Weakness stacks on all enemies.";
        UpdateDescription();
    }
}