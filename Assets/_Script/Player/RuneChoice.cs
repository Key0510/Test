using System.Collections.Generic;
using UnityEngine;

public class BlockChoices : MonoBehaviour
{
    public BlockList blockList;            // BlockList 컴포넌트
    public Transform leftPanelParent;      // UI 부모
    public GameObject blockIconPrefab;     // 아이콘용 프리팹

    private void OnEnable()
    {
        // Subscribe to block list changes
        if (blockList != null)
        {
            blockList.OnBlockListChanged += RefreshIcons;
        }
        RefreshIcons(); // Initial refresh
    }

    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (blockList != null)
        {
            blockList.OnBlockListChanged -= RefreshIcons;
        }
    }

    public void AddBlock(GameObject block)
    {
        // BlockList에 추가 시도
        bool added = blockList.AddBlock(block);

            // CreateBlockIconUI is called via RefreshIcons triggered by AddBlock
            Debug.Log($"{block.name} 블록이 추가되었습니다.");

    }

    public void RefreshIcons()
    {
        // Clear existing icons
        for (int i = leftPanelParent.childCount - 1; i >= 0; i--)
        {
            Destroy(leftPanelParent.GetChild(i).gameObject);
        }

        // Create new icons for all blocks in the list
        if (blockList == null || blockList.blockPrefabs == null)
        {
            Debug.LogError("BlockList 또는 blockPrefabs가 null입니다.");
            return;
        }

        foreach (var block in blockList.blockPrefabs)
        {
            CreateBlockIconUI(block);
        }
    }

    private void CreateBlockIconUI(GameObject block)
    {
        if (blockIconPrefab == null || leftPanelParent == null)
        {
            Debug.LogError("blockIconPrefab 또는 leftPanelParent가 null입니다.");
            return;
        }

        GameObject icon = Instantiate(blockIconPrefab, leftPanelParent);

        var img = icon.GetComponentInChildren<UnityEngine.UI.Image>();
        if (img != null)
        {
            Sprite sp = block.GetComponent<SpriteRenderer>()?.sprite;
            if (sp != null) img.sprite = sp;
        }

        var nameTxt = icon.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (nameTxt != null)
        {
            nameTxt.text = block.name;
        }

        // Find LevelText under Canvas, which is a child of the Image
        var canvas = img?.GetComponentInChildren<Canvas>();
        var levelTxt = canvas?.transform.Find("LevelText")?.GetComponent<TMPro.TextMeshProUGUI>();
        if (levelTxt != null)
        {
            // Use BlockAbility to get the level
            var blockComponent = block.GetComponent<BlockAbility>();
            if (blockComponent != null && blockComponent.Level > 0)
            {
                levelTxt.text = $"Lv.{blockComponent.Level}";
            }
            else
            {
                levelTxt.text = "Lv.1"; // Default level if none found
                Debug.LogWarning($"{block.name}에 BlockAbility 컴포넌트 또는 유효한 레벨이 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("LevelText TMPro.TextMeshProUGUI 컴포넌트를 Canvas의 자식으로 찾을 수 없습니다.");
        }
    }
}