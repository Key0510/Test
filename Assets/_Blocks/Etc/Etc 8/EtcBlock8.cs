using UnityEngine;
using System;

/// <summary>
/// 앞(가장 x가 작은) 적부터 공격하고, 남은 데미지를 뒤의 적에게 순차적으로 넘기는 능력.
/// - 치명타는 1회 판정하여 전체 체인에 동일하게 적용
/// - 기본 피해는 BlockAbility의 AttackDamage / CriticalHitDamage 사용
/// - (옵션) 플레이어의 attackPower를 더해 총 피해를 시작값으로 사용할 수 있음
/// - (옵션) 최대 타격 대상 수를 제한할 수 있음(0이면 무제한)
/// </summary>
public class EtcBlock8 : BlockAbility
{
    [Header("옵션 (인스펙터에서 수정 가능)")]
    [Tooltip("총 피해에 플레이어의 attackPower를 더할지 여부")]
    public bool includePlayerAttackPower = true;

    [Tooltip("맞는 적의 최대 수(0이면 제한 없음)")]
    public int maxTargets = 0;

    private PlayerAbility playerAbility;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        if (playerAbility == null)
        {
            Debug.LogWarning("[PiercingOverflow] PlayerAbility를 찾을 수 없습니다.");
        }
        descriptionTemplate = "가장 가까운 적에게 {0} 독 스택을 부여합니다.";
    }

    public override void Execute()
    {
        // 적 수집
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies == null || enemies.Length == 0) return;

        // x가 작은 순(앞쪽)으로 정렬
        Array.Sort(enemies, (a, b) =>
            a.transform.position.x.CompareTo(b.transform.position.x));

        // 시작 피해량 계산 (치명타 1회 판정)
        bool isCriticalHit = (playerAbility != null) && (UnityEngine.Random.value < playerAbility.critChance);
        int damagePool = isCriticalHit ? CriticalHitDamage : AttackDamage;
        if (includePlayerAttackPower && playerAbility != null)
            damagePool += playerAbility.attackPower;

        if (damagePool <= 0) return;

        int hitCount = 0;
        int initialDamage = damagePool;

        // 순차적으로 적에게 피해 적용, 남은 피해는 다음 적에게 넘김
        for (int i = 0; i < enemies.Length; i++)
        {
            if (maxTargets > 0 && hitCount >= maxTargets) break;

            Enemy enemy = enemies[i].GetComponent<Enemy>();
            if (enemy == null || enemy.IsDead()) continue;

            // 이 적에게 실제로 들어갈 수 있는 피해량 계산
            // - 먼저 쉴드 소모 -> 남은 피해로 체력 감소
            int remain = damagePool;

            int damageToShield = (int)Mathf.Min(enemy.currentShield, remain);
            remain -= damageToShield;

            int damageToHealth = (int)Mathf.Min(enemy.currentHealth, remain);
            remain -= damageToHealth;

            int deal = damagePool - remain; // 이 적에게 실제로 들어간 총 피해

            if (deal > 0)
            {
                enemy.TakeDamage(deal);
                damagePool -= deal;
                hitCount++;
            }

            // 남은 피해가 없으면 종료
            if (damagePool <= 0) break;
        }

        // 로그
        if (hitCount > 0)
        {
            if (isCriticalHit)
                Debug.Log($"[PiercingOverflow] CRIT! 시작 피해 {initialDamage} → {hitCount}명에게 관통/잔여 피해 적용, 남은 피해 {damagePool}");
            else
                Debug.Log($"[PiercingOverflow] 시작 피해 {initialDamage} → {hitCount}명에게 관통/잔여 피해 적용, 남은 피해 {damagePool}");
        }
    }

    public override void Upgrade()
    {
        // Description만 변경
        Description =
            $"앞의 적부터 {AttackDamage}(치명타 시 {CriticalHitDamage})로 공격하고 남은 피해를 뒤의 적에게 순차적으로 넘깁니다. " +
            $"{(includePlayerAttackPower ? "플레이어 공격력이 합산됩니다. " : "")}" +
            $"{(maxTargets > 0 ? $"최대 {maxTargets}명까지 타격." : "타격 대상 제한 없음.")}";
    }
        public override void UpdateDescription()
    {
        description = string.Format(descriptionTemplate, attackDamage, criticalHitDamage);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        descriptionTemplate = "Deals {0} + player attack damage (or {1} + player attack damage on critical hit) and sequentially transfers remaining damage to the next enemies.";
        UpdateDescription();
    }
}
