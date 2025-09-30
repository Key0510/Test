using UnityEngine;
using System.Collections.Generic;

public class EnemyList : MonoBehaviour
{
    [Header("Enemy List")]
    public List<GameObject> enemyPrefabs; // 적 프리팹 목록

    [Header("Boss List")]
    public List<GameObject> bossPrefabs; // 보스 프리팹 목록
}
