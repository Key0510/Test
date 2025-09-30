using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class BlockButtonSpawner : MonoBehaviour
{
    public BlockList blockList;         
    public Transform targetPanel;       
    public GameObject buttonPrefab;     

    // 현재 블록 정보 표시 패널
    public GameObject detailPanel;      
    public TextMeshProUGUI descriptionText;        
    public TextMeshProUGUI upgradeDescriptionText; 
    public TextMeshProUGUI levelText; // 블록 레벨 표시

    // 다음 레벨 블록 정보 표시 패널
    public GameObject nextLevelDetailPanel;      
    public TextMeshProUGUI nextLevelNameText;        
    public TextMeshProUGUI nextLevelDescriptionText; 
    public TextMeshProUGUI nextLevelLevelText; // 다음 레벨 블록 레벨 표시
    public TextMeshProUGUI nextLevelCostText;  // 다음 레벨 업그레이드 비용 표시

    public Button upgradeButton;        

    [SerializeField] private int level1Cost = 50; // 레벨 1 → 2 비용
    [SerializeField] private int level2Cost = 100; // 레벨 2 → 3 비용

    private int selectedIndex = -1; 
    private BlockAbility selectedBlock; 
    private int currentUpgradeCost = 0; // 현재 선택 블록의 업그레이드 비용

    void OnEnable()
    {
        RefreshButtons();
        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(false); // 초기 상태에서 업그레이드 버튼 비활성화
        else
            Debug.LogError("upgradeButton이 인스펙터에서 할당되지 않았습니다.");
    }

    public void RefreshButtons()
    {
        SpawnButtons();
    }

    void SpawnButtons()
    {
        // 기존 버튼 제거
        for (int i = targetPanel.childCount - 1; i >= 0; i--)
        {
            Destroy(targetPanel.GetChild(i).gameObject);
        }

        if (blockList == null || blockList.blockPrefabs == null || blockList.blockPrefabs.Count == 0)
        {
            Debug.LogWarning("BlockList 또는 blockPrefabs가 null이거나 비어 있습니다.");
            return;
        }

        for (int index = 0; index < blockList.blockPrefabs.Count; index++)
        {
            GameObject blockPrefab = blockList.blockPrefabs[index];
            if (blockPrefab == null)
            {
                Debug.LogWarning($"blockPrefabs[{index}]가 null입니다.");
                continue;
            }

            GameObject btnObj = Instantiate(buttonPrefab, targetPanel);
            Image btnImage = btnObj.GetComponent<Image>();
            if (btnImage == null)
            {
                Debug.LogError("buttonPrefab에 Image 컴포넌트가 없습니다.");
                continue;
            }

            SpriteRenderer sr = blockPrefab.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                btnImage.sprite = sr.sprite;
            }
            else
            {
                Debug.LogWarning("프리팹에 SpriteRenderer 또는 Sprite가 없습니다: " + blockPrefab.name);
            }

            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = blockPrefab.name;
            }
            else
            {
                Debug.LogWarning("buttonPrefab에 TextMeshProUGUI 컴포넌트가 없습니다: " + blockPrefab.name);
            }

            Button btn = btnObj.GetComponent<Button>();
            BlockAbility blockAbility = blockPrefab.GetComponent<BlockAbility>();
            if (blockAbility != null)
            {
                int currentIndex = index;
                btn.onClick.AddListener(() => SelectBlock(blockAbility, currentIndex));
            }
            else
            {
                Debug.LogWarning("BlockPrefab에 BlockAbility 컴포넌트가 없습니다: " + blockPrefab.name);
            }
        }
    }

    private void SelectBlock(BlockAbility block, int index)
    {
        selectedBlock = block;
        selectedIndex = index;

        // 현재 블록 정보 표시
        descriptionText.text = block.Name;          
        upgradeDescriptionText.text = block.Description;
        levelText.text = $"Lv.{block.Level}";
        detailPanel.SetActive(true);

        // 업그레이드 비용 계산
        if (block.Level == 1) currentUpgradeCost = level1Cost;
        else if (block.Level == 2) currentUpgradeCost = level2Cost;
        else currentUpgradeCost = 0; // 더 이상 업그레이드 불가

        // 다음 레벨 블록 정보 표시
        if (block.NextLevelBlock != null)
        {
            BlockAbility nextBlock = block.NextLevelBlock.GetComponent<BlockAbility>();
            if (nextBlock != null)
            {
                nextLevelNameText.text = nextBlock.Name;
                nextLevelDescriptionText.text = nextBlock.Description;
                nextLevelLevelText.text = $"Lv.{nextBlock.Level}";
                nextLevelCostText.text = $"{currentUpgradeCost}G";
                nextLevelDetailPanel.SetActive(true);
            }
            else
            {
                Debug.LogWarning("NextLevelBlock에 BlockAbility 컴포넌트가 없습니다: " + block.NextLevelBlock.name);
                nextLevelNameText.text = "MAX";
                nextLevelDescriptionText.text = "최대 레벨에 도달했습니다.";
                nextLevelLevelText.text = "";
                nextLevelCostText.text = "업그레이드 불가";
                nextLevelDetailPanel.SetActive(true);
            }
        }
        else
        {
            // 최대 레벨 도달 시 "MAX" 표시
            nextLevelNameText.text = "MAX";
            nextLevelDescriptionText.text = "-";
            nextLevelLevelText.text = "MAX";
            nextLevelCostText.text = "MAX";
            nextLevelDetailPanel.SetActive(true);
        }

        // 업그레이드 버튼 상태 업데이트
        if (upgradeButton == null)
        {
            Debug.LogError("upgradeButton이 null입니다. 인스펙터에서 할당 확인하세요.");
            return;
        }

        upgradeButton.gameObject.SetActive(true); // 항상 버튼 활성화
        bool canUpgrade = currentUpgradeCost > 0 && MoneyManager.Instance != null && MoneyManager.Instance.GetMoney() >= currentUpgradeCost;
        upgradeButton.interactable = canUpgrade; // 돈이 충분하고 업그레이드 가능할 때만 클릭 가능
        Debug.Log($"SelectBlock: Block={block.Name}, Level={block.Level}, Cost={currentUpgradeCost}, Money={MoneyManager.Instance?.GetMoney()}, CanUpgrade={canUpgrade}");

        // 업그레이드 버튼 이벤트 연결
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(UpgradeSelectedBlock);
    }

    private void UpgradeSelectedBlock()
    {
        if (selectedBlock != null && selectedIndex >= 0 && selectedIndex < blockList.blockPrefabs.Count)
        {
            if (currentUpgradeCost <= 0)
            {
                Debug.LogWarning("업그레이드 불가 (최대 레벨이거나 비용 설정 안됨)");
                return;
            }

            // 돈이 충분한지 확인
            if (MoneyManager.Instance == null || !MoneyManager.Instance.SpendMoney(currentUpgradeCost))
            {
                Debug.LogWarning("돈이 부족하거나 MoneyManager가 null입니다.");
                return;
            }

            GameObject nextPrefab = selectedBlock.NextLevelBlock;
            if (nextPrefab != null)
            {
                // Update block list using UpdateBlock to trigger event
                blockList.UpdateBlock(selectedIndex, nextPrefab);
                Debug.Log($"Upgraded: {selectedBlock.name} to {nextPrefab.name}");
                RefreshButtons();
                detailPanel.SetActive(false);
                nextLevelDetailPanel.SetActive(false);
                upgradeButton.gameObject.SetActive(false); // 업그레이드 후 버튼 비활성화
            }
            else
            {
                Debug.LogWarning("다음 레벨 프리팹이 없습니다. 최대 레벨에 도달했습니다.");
            }
        }
        else
        {
            Debug.LogWarning("선택된 블록이 없습니다.");
        }
    }
}