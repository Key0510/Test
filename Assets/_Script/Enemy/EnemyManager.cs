using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyManager : MonoBehaviour
{
    public List<Enemy> enemyList = new List<Enemy>();
    private GateSpawner gateSpawner; // GateSpawner 참조 추가

    [System.Obsolete]
    void Start()
    {
        gateSpawner = FindObjectOfType<GateSpawner>();
        if (gateSpawner == null)
        {
            Debug.LogError("[EnemyManager] GateSpawner not found!");
        }
    }

    [System.Obsolete]
    public void StartEnemyTurn()
    {
        Debug.Log("[EnemyManager] StartEnemyTurn 호출됨");
        RefreshAndSortEnemyList();
    }

    [System.Obsolete]
    private void RefreshAndSortEnemyList()
    {
        enemyList.Clear();
        Enemy[] allEnemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in allEnemies)
        {
            string tag = enemy.gameObject.tag;
            if (tag == "Enemy" && !enemy.IsDead()) // 죽은 적 제외
            {
                enemyList.Add(enemy);
                Debug.Log($"[EnemyManager] 적 추가: {enemy.gameObject.name}, Tag: {tag}, IsDead: {enemy.IsDead()}");
            }
        }

        Debug.Log($"[EnemyManager] 적 리스트에 들어간 개수: {enemyList.Count}, Names: {string.Join(", ", enemyList.Select(e => e.gameObject.name))}");

        // 리스트가 비어 있으면 라운드 종료 및 게이트 스폰
        if (enemyList.Count == 0 && gateSpawner != null)
        {
            Debug.Log("[EnemyManager] 적 리스트가 비어 있음, 라운드 종료 및 게이트 스폰");
            gateSpawner.isRoundEnd = true;
            gateSpawner.SpawnTwoRandomGates();
            return;
        }

        // X좌표 기준 정렬 (왼쪽부터 오른쪽)
        enemyList.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        Debug.Log($"[EnemyManager] 정렬된 적 리스트: {string.Join(", ", enemyList.Select(e => e.gameObject.name))}");
    }

    public IEnumerator EnemyTurnRoutine()
    {
        Debug.Log($"[EnemyManager] EnemyTurnRoutine 시작, 적 수: {enemyList.Count}");
        for (int i = 0; i < enemyList.Count; i++)
        {
            Enemy enemy = enemyList[i];
            if (enemy != null && !enemy.IsDead())
            {
                Debug.Log($"[EnemyManager] 적 공격 처리: {enemy.gameObject.name}");
                yield return enemy.Attack();
            }
            else
            {
                Debug.Log($"[EnemyManager] 적 스킵: {enemy?.gameObject.name ?? "null"}, IsDead: {enemy?.IsDead() ?? true}");
            }
        }

        // 턴 종료 후 적 리스트 재확인
        RefreshAndSortEnemyList(); // 리스트 갱신
        if (enemyList.Count == 0 && gateSpawner != null)
        {
            Debug.Log("[EnemyManager] 모든 적이 죽음, 라운드 종료 및 게이트 스폰");
            gateSpawner.isRoundEnd = true;
            gateSpawner.SpawnTwoRandomGates();
        }

        Debug.Log("[EnemyManager] EnemyTurnRoutine 완료");
    }
}