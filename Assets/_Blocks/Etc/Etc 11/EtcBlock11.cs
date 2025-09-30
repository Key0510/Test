using UnityEngine;
using System.Linq;

public class EtcBlock11 : BlockAbility
{
    private PlayerAbility playerAbility;
    private BlockManager blockManager;
    public int reduceDamage = 1;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to all enemies with penetration, but damage decreases by 1 for each enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility가 없습니다.");
            return;
        }

        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjs.Length == 0)
        {
            Debug.Log("적을 찾을 수 없어 데미지를 입히지 못함.");
            return;
        }

        // x-position으로 적 정렬 (가장 가까운 적부터)
        var sortedEnemies = enemyObjs.OrderBy(e => e.transform.position.x).ToList();
        int affectedEnemies = 0;

        // 치명타 확률에 따라 치명타 여부 결정
        bool isCriticalHit = Random.value < playerAbility.critChance;
        // 초기 데미지 계산: 블록 데미지 + 플레이어 공격력
        int currentDamage = (isCriticalHit ? CriticalHitDamage : AttackDamage) + playerAbility.attackPower;

        string logMessage = isCriticalHit ? "치명타! " : "";

        for (int i = 0; i < sortedEnemies.Count; i++)
        {
            Enemy enemy = sortedEnemies[i].GetComponent<Enemy>();
            if (enemy == null) continue;

            // 데미지 적용 (최소 0)
            if (currentDamage > 0)
            {
                enemy.TakeDamage(currentDamage);
                affectedEnemies++;
                logMessage += $"{sortedEnemies[i].name}에게 {currentDamage} 데미지를 입힘{(i < sortedEnemies.Count - reduceDamage ? ", " : "")}";
                currentDamage = Mathf.Max(0, currentDamage - reduceDamage); // 감소
            }
            else
            {
                break; // 데미지가 0이 되면 종료
            }
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
        Description = string.Format("모든 적에게 {0} + 플레이어 공격력 데미지(치명타 시 {1} + 플레이어 공격력)를 관통하며 주되, 각 적마다 데미지가 1씩 줄어들고, {2} 방어력을 얻음.", AttackDamage, CriticalHitDamage, Defense); // 설명 업데이트
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) to all enemies with penetration, but damage decreases by 1 for each enemy.";
        UpdateDescription();
    }
}