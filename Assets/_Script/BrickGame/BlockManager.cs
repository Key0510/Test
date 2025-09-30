using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    public List<GameObject> brokenBlocks = new List<GameObject>();
    public Transform brokenBlocksPanel; // 패널의 Transform (Grid Layout Group이 있는 패널)
    public GameObject blockImagePrefab; // UI에 표시할 블록 이미지 프리팹 (Image 컴포넌트 포함)
    private Dictionary<GameObject, GameObject> blockToUIImageMap = new Dictionary<GameObject, GameObject>(); // 블록과 UI 이미지 매핑

    public void AddBrokenBlock(GameObject block)
    {
        brokenBlocks.Add(block);
        DisplayBrokenBlock(block);
    }

    public void DisplayBrokenBlock(GameObject block)
    {
        SpriteRenderer spriteRenderer = block.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && blockImagePrefab != null)
        {
            GameObject blockImage = Instantiate(blockImagePrefab, brokenBlocksPanel);
            Image imageComponent = blockImage.GetComponent<Image>();
            if (imageComponent != null)
            {
                imageComponent.sprite = spriteRenderer.sprite;
                blockToUIImageMap.Add(block, blockImage); // 블록과 UI 이미지 매핑 저장
            }
        }
    }

    // 특정 블록에 해당하는 UI 이미지 제거
    public void RemoveBrokenBlockUI(GameObject block)
    {
        if (blockToUIImageMap.ContainsKey(block))
        {
            Destroy(blockToUIImageMap[block]);
            blockToUIImageMap.Remove(block);
        }
    }

    // 모든 UI 이미지 제거
    public void ClearBrokenBlocksUI()
    {
        foreach (Transform child in brokenBlocksPanel)
        {
            Destroy(child.gameObject);
        }
        blockToUIImageMap.Clear();
    }
}