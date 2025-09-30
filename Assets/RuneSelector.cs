using UnityEngine;

public class RuneSelector : MonoBehaviour
{
    private Floor floor;
    private PlayerAbility playerAbility;
    private MoneyManager moneyManager;

    private void Awake()
    {
        // 필요한 컴포넌트 참조
        floor = FindObjectOfType<Floor>();
        playerAbility = FindObjectOfType<PlayerAbility>();
        moneyManager = FindObjectOfType<MoneyManager>();

        // null 체크
        if (floor == null) Debug.LogError("Floor component not found!");
        if (playerAbility == null) Debug.LogError("PlayerAbility component not found!");
        if (moneyManager == null) Debug.LogError("MoneyManager component not found!");
    }

    // 벽돌 깨질 때마다 돈 1원 추가 능력 활성화
    public void ActivateMoneyOnBreak()
    {
        if (floor == null)
        {
            Debug.LogWarning("Floor is null, cannot activate MoneyOnBreak");
            return;
        }
        floor.enableMoneyOnBreak = true;
        floor.BreakMoneyAmount += 2;
        Debug.Log($"MoneyOnBreak activated. BreakMoneyAmount: {floor.BreakMoneyAmount}");
    }

    // 벽돌 깨질 때마다 최대 체력 1 증가 능력 활성화
    public void ActivateMaxHpOnBreak()
    {
        if (floor == null)
        {
            Debug.LogWarning("Floor is null, cannot activate MaxHpOnBreak");
            return;
        }
        floor.enableMaxHpOnBreak = true;
        floor.BreakMaxHealthAmount += 1;
        Debug.Log($"MaxHpOnBreak activated. BreakMaxHealthAmount: {floor.BreakMaxHealthAmount}");
    }

    // 벽돌 깨질 때마다 공격력 1 증가 능력 활성화
    public void ActivateAttackOnBreak()
    {
        if (floor == null)
        {
            Debug.LogWarning("Floor is null, cannot activate AttackOnBreak");
            return;
        }
        floor.enableAttackOnBreak = true;
        floor.BreakAttackAmount += 1;
        Debug.Log($"AttackOnBreak activated. BreakAttackAmount: {floor.BreakAttackAmount}");
    }

    // 벽돌 깨질 때마다 방어도 1 증가 능력 활성화
    public void ActivateDefenseOnBreak()
    {
        if (floor == null)
        {
            Debug.LogWarning("Floor is null, cannot activate DefenseOnBreak");
            return;
        }
        floor.enableDefenseOnBreak = true;
        floor.BreakShieldAmount += 2;
        Debug.Log($"DefenseOnBreak activated. BreakShieldAmount: {floor.BreakShieldAmount}");
    }

    // 벽돌 깨질 때마다 체력 3 회복 능력 활성화
    public void ActivateHpRecoveryOnBreak()
    {
        if (floor == null)
        {
            Debug.LogWarning("Floor is null, cannot activate HpRecoveryOnBreak");
            return;
        }
        floor.enableHpRecoveryOnBreak = true;
        floor.BreakHealAmount += 3;
        Debug.Log($"HpRecoveryOnBreak activated. BreakHealAmount: {floor.BreakHealAmount}");
    }

    // 턴 종료 시 방어도 10% 유지 능력 활성화
    public void ActivateShieldRetentionOnTurnEnd()
    {
        if (floor == null)
        {
            Debug.LogWarning("Floor is null, cannot activate ShieldRetentionOnTurnEnd");
            return;
        }
        floor.enableShieldRetentionOnTurnEnd = true;
        floor.shieldRetentionPercentage += 20f;
        Debug.Log($"ShieldRetentionOnTurnEnd activated. ShieldRetentionPercentage: {floor.shieldRetentionPercentage}%");
    }

    // 받는 모든 피해 10% 감소 능력 활성화
    public void ActivateDamageReduction()
    {
        if (playerAbility == null)
        {
            Debug.LogWarning("PlayerAbility is null, cannot activate DamageReduction");
            return;
        }
        playerAbility.enableDamageReduction = true;
        playerAbility.damageReductionPercentage += 10f;
        Debug.Log($"DamageReduction activated. DamageReductionPercentage: {playerAbility.damageReductionPercentage}%");
    }

    // 얻는 돈 10% 증가 능력 활성화
    public void ActivateMoneyIncrease()
    {
        if (moneyManager == null)
        {
            Debug.LogWarning("MoneyManager is null, cannot activate MoneyIncrease");
            return;
        }
        moneyManager.enableMoneyIncrease = true;
        moneyManager.moneyIncreasePercentage += 20f;
        Debug.Log($"MoneyIncrease activated. MoneyIncreasePercentage: {moneyManager.moneyIncreasePercentage}%");
    }

    public void ActivateCriticalIncrease()
    {
        if (moneyManager == null)
        {
            Debug.LogWarning("MoneyManager is null, cannot activate MoneyIncrease");
            return;
        }
        floor.enableCritOnBreak = true;
        floor.BreakCritAmount += 0.01f;
        Debug.Log($"MoneyIncrease activated. MoneyIncreasePercentage: {moneyManager.moneyIncreasePercentage}%");
    }

    public void ActivaeAddBall()
    {
        BallManager ballManager = FindObjectOfType<BallManager>();
        ballManager.playerBallCount += 50;
    }
    public void ActivaeTurnReduce()
    {
        floor.Turn -= 40;
    }
}