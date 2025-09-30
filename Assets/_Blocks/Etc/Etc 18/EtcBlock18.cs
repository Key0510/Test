using UnityEngine;
using System.Linq;

public class EtcBlock18 : BlockAbility
{
    [Header("뒤 enemy딜 비율(맨앞의 일부)")]
    public float backDeal = 0.5f;

    private PlayerAbility playerAbility;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "가장 가까운 적에게 {0} 독 스택을 부여합니다.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
            return;
        }

        // x-position으로 적 정렬 (가장 가까운 적부터)
        var sortedEnemies = enemies.OrderBy(e => e.transform.position.x).ToList();
        int affectedEnemies = 0;

        // 치명타 확률에 따라 치명타 여부 결정
        bool isCriticalHit = Random.value < playerAbility.critChance;
        // 맨 앞 적 데미지 계산: 블록 데미지 + 플레이어 공격력
        int frontDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;
        // 뒤의 적 데미지: 맨 앞 데미지의 절반
        int backDamage = Mathf.FloorToInt(frontDamage * backDeal);

        string logMessage = isCriticalHit ? "치명타! " : "";

        // 맨 앞 적 공격
        Enemy frontEnemy = sortedEnemies[0].GetComponent<Enemy>();
        if (frontEnemy != null)
        {
            frontEnemy.TakeDamage(frontDamage);
            affectedEnemies++;
            logMessage += $"가장 앞의 {sortedEnemies[0].name}에게 {frontDamage} 데미지를 입힘";
        }

        // 뒤의 모든 적 공격 (절반 데미지)
        if (sortedEnemies.Count > 1)
        {
            for (int i = 1; i < sortedEnemies.Count; i++)
            {
                Enemy enemy = sortedEnemies[i].GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(backDamage);
                    affectedEnemies++;
                }
            }
            logMessage += $", 뒤의 {sortedEnemies.Count - 1}명에게 각각 {backDamage} 데미지를 입힘";
        }

        if (affectedEnemies > 0)
        {
            Debug.Log($"{logMessage} (총 {affectedEnemies}명의 적 공격)");
        }
        else
        {
            Debug.Log("유효한 적을 찾을 수 없어 데미지를 입히지 못함.");
        }
    }

    public override void Upgrade()
    {
        Description = string.Format("가장 앞의 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 주고, 뒤의 모든 적에게 그 절반의 데미지를 주며, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to the frontmost enemy, and half that damage to all enemies behind.";
        UpdateDescription();
    }
}