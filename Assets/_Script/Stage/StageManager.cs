using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StageManager : MonoBehaviour
{
    [Header("현재 스테이지 (0부터 시작)")]
    public int currentStage = 0;

    [Header("프리팹 리스트 (EnemyList에 일반/보스 분리 저장)")]
    public EnemyList enemyList;

    [Header("스폰 포지션 (인덱스가 작은 순서대로 스폰)")]
    public List<Transform> spawnPositions = new List<Transform>();

    [Header("보스 스폰 포인트")]
    public Transform bossSpawnPoint;

    public BackgroundManager backgroundManager;
    /// <summary>
    /// 규칙:
    /// - currentStage % 4 != 0 : 나머지 값만큼 일반 적을 "중복 허용" 랜덤 스폰 (보스 스폰 X)
    /// - currentStage % 4 == 0 : 보스 (몫-1 인덱스) 1마리 + 일반 적 2마리 "중복 비허용" 랜덤 스폰
    /// - 스폰 순서는 spawnPositions 인덱스가 작은 순
    /// </summary>
    void Start()
    {
        backgroundManager = FindObjectOfType<BackgroundManager>();
    }

public void SpawnForCurrentStage()
{
    Debug.Log($"[StageManager] SpawnForCurrentStage called. currentStage: {currentStage}");

    if (enemyList == null)
    {
        Debug.LogError("[StageManager] EnemyList가 비어 있습니다.");
        return;
    }

    if (spawnPositions == null || spawnPositions.Count == 0)
    {
        Debug.LogError("[StageManager] spawnPositions가 비어 있습니다.");
        return;
    }

    int remainder = currentStage % 4;
    int quotient = currentStage / 4;

    // 1) 보스 스테이지가 아닌 경우 (나머지 != 0)
    if (remainder != 0)
    {
        int need = remainder;

        for (int i = 0; i < need; i++)
        {
            if (i >= spawnPositions.Count) break;

            GameObject prefab = PickRandomAllowDuplicate(enemyList.enemyPrefabs);
            if (prefab == null)
            {
                Debug.LogWarning("[StageManager] enemyPrefabs가 비어 있어 스폰할 수 없습니다.");
                break;
            }

            Debug.Log($"[StageManager] Spawning normal enemy: {prefab.name} at {spawnPositions[i].position}");
            Instantiate(prefab, spawnPositions[i].position, Quaternion.identity);
        }
        return;
    }

    // 2) 보스 스테이지인 경우 (나머지 == 0)
    int bossIndex = Mathf.Max(0, quotient - 1);

    // 2-1) 보스 스폰
    if (bossIndex >= 0 && bossIndex < enemyList.bossPrefabs.Count)
    {
        if (bossSpawnPoint != null && enemyList.bossPrefabs[bossIndex] != null)
        {
            Debug.Log($"[StageManager] Spawning boss: {enemyList.bossPrefabs[bossIndex].name} at {bossSpawnPoint.position}");
            Instantiate(enemyList.bossPrefabs[bossIndex], bossSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("[StageManager] 보스 프리팹 또는 bossSpawnPoint가 비어 있습니다.");
        }
    }
    else
    {
        Debug.LogWarning($"[StageManager] bossIndex {bossIndex} 가 bossPrefabs 범위를 벗어났습니다.");
    }
}

    // ===== 유틸 =====

    // 중복 허용 랜덤
    private GameObject PickRandomAllowDuplicate(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        int idx = UnityEngine.Random.Range(0, list.Count);
        return list[idx];
    }

    // 중복 비허용 랜덤 n개
    private List<GameObject> PickRandomUnique(List<GameObject> list, int count)
    {
        List<GameObject> result = new List<GameObject>();
        if (list == null || list.Count == 0 || count <= 0) return result;

        // 리스트 복사 후 섞기(Fisher–Yates)
        List<GameObject> pool = list.Where(x => x != null).ToList();
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        int take = Mathf.Min(count, pool.Count);
        for (int i = 0; i < take; i++)
            result.Add(pool[i]);

        return result;
    }
}
