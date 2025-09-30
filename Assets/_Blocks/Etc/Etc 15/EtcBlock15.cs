// 기타15
using UnityEngine;

public class EtcBlock15 : BlockAbility
{
    [SerializeField] private int damageAmount = 1; // 적 데미지 (인스펙터에서 설정)
    [SerializeField] private int weaknessAmount = 1; // 약화 스택 (인스펙터에서 설정)
    [SerializeField] private int poisonAmount = 1; // 독 스택 (인스펙터에서 설정)
    [SerializeField] private int moneyAmount = 1; // 돈 증가 (인스펙터에서 설정)
    [SerializeField] private int shieldAmount = 1; // 방어도 증가 (인스펙터에서 설정)
    [SerializeField] private int healAmount = 1; // 체력 회복 (인스펙터에서 설정)
    [SerializeField] private int attackPowerIncrease = 1; // 공격력 증가 (인스펙터에서 설정)
    [SerializeField] private float critChanceIncrease = 0.01f; // 치명타 확률 증가 (인스펙터에서 설정, 1% = 0.01)

    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;
    private BlockManager blockManager;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();
        blockManager = FindObjectOfType<BlockManager>();
        descriptionTemplate = "Grants the player {0} armor, {1} money, {2} health, {3} attack power, and {4} critical hit chance. Deals {5} damage, {6} poison stacks, and {7} weakness stacks to the nearest enemy.";
    }

    public override void Execute()
    {
        if (playerAbility == null || moneyManager == null)
        {
            Debug.LogWarning("PlayerAbility 또는 MoneyManager가 없습니다.");
            return;
        }

        // 플레이어 효과 적용
        moneyManager.AddMoney(moneyAmount);
        playerAbility.AddShield(shieldAmount);
        playerAbility.Heal(healAmount);
        playerAbility.AddAttackPower(attackPowerIncrease);
        playerAbility.AddCritChance(critChanceIncrease);

        GameObject closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Enemy enemy = closestEnemy.GetComponent<Enemy>();
            if (enemy != null)
            {
                // 가장 가까운 적에게 효과 적용
                enemy.TakeDamage(damageAmount);
                enemy.weakness += weaknessAmount;
                enemy.poison += poisonAmount;
                enemy.UpdateWeaknessUI();
                enemy.UpdatePoisonUI();

                Debug.Log($"{closestEnemy.name}에게 {damageAmount} 데미지, {weaknessAmount} 약화, {poisonAmount} 독을 적용함. 플레이어에게 {moneyAmount} 돈, {shieldAmount} 방어도, {healAmount} 회복, {attackPowerIncrease} 공격력, {critChanceIncrease:P0} 치명타 확률 증가.");
            }
        }
        else
        {
            Debug.Log("플레이어 효과 적용: {moneyAmount} 돈, {shieldAmount} 방어도, {healAmount} 회복, {attackPowerIncrease} 공격력, {critChanceIncrease:P0} 치명타 확률 증가. (적 없음)");
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minX = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                Enemy enemyComp = enemy.GetComponent<Enemy>();
                if (enemyComp != null && !enemyComp.IsDead())
                {
                    float xPos = enemy.transform.position.x;
                    if (xPos < minX)
                    {
                        minX = xPos;
                        closestEnemy = enemy;
                    }
                }
            }
        }
        return closestEnemy;
    }

    public override void Upgrade()
    {
        Description = string.Format("가장 가까운 적에게 {0} 데미지, {1} 약화, {2} 독을 적용하고, 플레이어에게 {3} 돈, {4} 방어도, {5} 회복, {6} 공격력, {7:P0} 치명타 확률 증가하며, {8} 방어력을 얻음.", damageAmount, weaknessAmount, poisonAmount, moneyAmount, shieldAmount, healAmount, attackPowerIncrease, critChanceIncrease, Defense); // 설명 업데이트
    }
    public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, shieldAmount, moneyAmount, healAmount, attackPowerIncrease, critChanceIncrease, damageAmount, poisonAmount, weaknessAmount);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Grants the player {0} armor, {1} money, {2} health, {3} attack power, and {4} critical hit chance. Deals {5} damage, {6} poison stacks, and {7} weakness stacks to the nearest enemy.";
        UpdateDescription();
    }
}