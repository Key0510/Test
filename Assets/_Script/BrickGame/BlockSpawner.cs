using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("블록 리스트 MonoBehaviour")]
    public BlockList blockList;

    [Header("스폰 포지션 리스트 (10개)")]
    public List<Transform> spawnPositions;

    [Header("추가볼 프리팹")]
    public GameObject AddBallPrefab;

public void SpawnBlocks()
{
    if (blockList == null || blockList.blockPrefabs.Count == 0)
    {
        Debug.LogWarning("BlockList 가 비어있습니다.");
        return;
    }

    // 블록 리스트 개수만큼 블록을 스폰
    int blockCount = blockList.blockPrefabs.Count;

    // 스폰 포지션 인덱스 리스트 생성
    List<int> availableIndices = new List<int>();
    for (int i = 0; i < spawnPositions.Count; i++)
    {
        availableIndices.Add(i);
    }

    // 스폰 포지션이 블록 개수보다 적으면 경고
    if (blockCount > availableIndices.Count)
    {
        Debug.LogWarning("스폰 포지션이 블록 개수보다 적습니다.");
        blockCount = availableIndices.Count; // 스폰 포지션 수로 제한
    }

    // 스폰 포지션 인덱스 섞기
    ShuffleList(availableIndices);

    // 블록 리스트의 모든 블록을 순회하며 스폰
    for (int i = 0; i < blockCount; i++)
    {
        GameObject blockPrefab = blockList.blockPrefabs[i]; // 순서대로 블록 선택
        int spawnIndex = availableIndices[i];
        Transform spawnPoint = spawnPositions[spawnIndex];
        Instantiate(blockPrefab, spawnPoint.position, Quaternion.identity);
    }

    // 빈 스폰 포지션 찾기
    List<int> usedBlockIndices = availableIndices.GetRange(0, blockCount);
    List<int> emptyIndices = new List<int>();
    for (int i = 0; i < spawnPositions.Count; i++)
    {
        if (!usedBlockIndices.Contains(i))
        {
            emptyIndices.Add(i);
        }
    }

    // AddBall 스폰 로직 (기존과 동일)
    if (AddBallPrefab != null && emptyIndices.Count > 0)
    {
        int addBallCount = Random.Range(1, 3); // 1 또는 2개
        addBallCount = Mathf.Min(addBallCount, emptyIndices.Count);

        ShuffleList(emptyIndices);

        for (int i = 0; i < addBallCount; i++)
        {
            int randomEmptyIndex = emptyIndices[i];
            Transform AddBallSpawnPoint = spawnPositions[randomEmptyIndex];
            Instantiate(AddBallPrefab, AddBallSpawnPoint.position, Quaternion.identity);
        }
    }
}

    void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void MoveAllBlocksDown()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        GameObject[] addBallBlocks = GameObject.FindGameObjectsWithTag("AddBall");

        foreach (GameObject block in blocks)
        {
            Vector3 pos = block.transform.position;
            pos.y -= 0.43f;
            block.transform.position = pos;
        }

        foreach (GameObject addball in addBallBlocks)
        {
            Vector3 pos1 = addball.transform.position;
            pos1.y -= 0.43f;
            addball.transform.position = pos1;
        }
    }

    public void RemoveAll()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            Destroy(block);
        }

        GameObject[] addBalls = GameObject.FindGameObjectsWithTag("AddBall");
        foreach (GameObject addBall in addBalls)
        {
            Destroy(addBall);
        }

        GameObject[] Balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject Ball in Balls)
        {
            Destroy(Ball);
        }
    }
}