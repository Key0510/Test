using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TrashCanvas : MonoBehaviour
{
    public BlockList blockList;         
    public Transform targetPanel;       
    public GameObject buttonPrefab;     

    // 설명 표시 패널
    public GameObject detailPanel;      
    public TextMeshProUGUI descriptionText;        
    public TextMeshProUGUI upgradeDescriptionText; 
    public Button deleteButton; // 삭제 버튼
    public BlockChoice blockChoice;

    [SerializeField] private int deleteCost = 50; // 삭제 비용 (인스펙터에서 설정)

    private int selectedIndex = -1; 
    private BlockAbility selectedBlock; 

    void OnEnable()
    {
        Debug.Log("TrashCanvas OnEnable 호출");
        RefreshButtons();
        if (detailPanel != null)
            detailPanel.SetActive(true);
        else
            Debug.LogWarning("detailPanel이 null입니다.");
    }

    public void RefreshButtons()
    {
        Debug.Log("RefreshButtons 호출");
        SpawnButtons();
    }

    void SpawnButtons()
    {
        Debug.Log($"SpawnButtons 시작, targetPanel: {targetPanel}, blockList: {blockList}, blockPrefabs Count: {(blockList?.blockPrefabs?.Count ?? 0)}");

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
            Debug.Log($"Processing block: {blockPrefab?.name}");

            GameObject btnObj = Instantiate(buttonPrefab, targetPanel);
            Debug.Log($"Button instantiated: {btnObj.name}, Parent: {targetPanel.name}");

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
                Debug.Log($"Sprite set: {sr.sprite.name}");
            }
            else
            {
                Debug.LogWarning("프리팹에 SpriteRenderer 또는 Sprite가 없습니다: " + blockPrefab.name);
            }


            Button btn = btnObj.GetComponent<Button>();
            BlockAbility blockAbility = blockPrefab.GetComponent<BlockAbility>();
            Debug.Log($"Block: {blockPrefab.name}, BlockAbility: {blockAbility != null}");
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

        // 블록 정보 표시
        descriptionText.text = block.Name;          
        upgradeDescriptionText.text = block.Description;
        Debug.Log($"Block selected: {block.Name}, Index: {index}, Description: {block.Description}");

        // 삭제 버튼 활성화 여부 결정
        int currentMoney = MoneyManager.Instance.GetMoney();
        deleteButton.interactable = currentMoney >= deleteCost;
        Debug.Log($"Delete button interactable: {deleteButton.interactable}, Current Money: {currentMoney}, Delete Cost: {deleteCost}");

        detailPanel.SetActive(true);

        // 삭제 버튼 이벤트 연결
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(DeleteSelectedBlock);
    }

    private void DeleteSelectedBlock()
    {
        if (selectedBlock != null && selectedIndex >= 0 && selectedIndex < blockList.blockPrefabs.Count)
        {
            // 돈이 충분한지 확인
            int currentMoney = MoneyManager.Instance.GetMoney();
            if (!MoneyManager.Instance.SpendMoney(deleteCost))
            {
                Debug.LogWarning($"돈이 부족하여 블록을 삭제할 수 없습니다. (Current Money: {currentMoney}, Delete Cost: {deleteCost})");
                return;
            }

            // 선택된 블록을 blockList에서 제거
            string blockName = blockList.blockPrefabs[selectedIndex].name;
            blockList.blockPrefabs.RemoveAt(selectedIndex);
            Debug.Log($"블록 삭제됨: {blockName}, 비용: {deleteCost}, 남은 돈: {MoneyManager.Instance.GetMoney()}");

            // UI 갱신 및 패널 비활성화
            RefreshButtons();
            detailPanel.SetActive(false);

            // 선택 상태 초기화
            selectedBlock = null;
            selectedIndex = -1;
            blockChoice.RefreshIcons();

        }
        else
        {
            Debug.LogWarning("선택된 블록이 없거나 유효하지 않습니다.");
        }
    }
}