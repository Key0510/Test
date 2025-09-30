using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BlockHoverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel; // 정보 표시용 패널
    [SerializeField] private TextMeshProUGUI nameText; // 블록 이름 표시용 텍스트
    [SerializeField] private TextMeshProUGUI descriptionText; // 블록 설명 표시용 텍스트

    private Camera mainCamera;
    private List<GameObject> processedBlocks = new List<GameObject>(); // 이미 처리된 블록 추적

    private void Start()
    {
        // 시작 시 패널 비활성화 및 초기 설정
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera가 씬에 없습니다. 태그가 'MainCamera'로 설정된 카메라를 추가하세요.");
            return;
        }

        if (mainCamera.GetComponent<Physics2DRaycaster>() == null)
        {
            Debug.LogError("Main Camera에 Physics2DRaycaster가 없습니다. 2D 호버 이벤트를 위해 추가하세요.");
            mainCamera.gameObject.AddComponent<Physics2DRaycaster>();
        }

        if (infoPanel == null)
        {
            Debug.LogError("InfoPanel이 할당되지 않았습니다. Inspector에서 할당하세요.");
            return;
        }

        if (infoPanel.GetComponent<RectTransform>() == null)
        {
            Debug.LogError("InfoPanel에 RectTransform 컴포넌트가 없습니다.");
            return;
        }

        if (nameText == null || descriptionText == null)
        {
            Debug.LogWarning("NameText 또는 DescriptionText가 할당되지 않았습니다. 텍스트가 표시되지 않을 수 있습니다.");
        }

        infoPanel.SetActive(false);
    }

    private void Awake()
    {
        // EventSystem 확인
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogError("씬에 EventSystem이 없습니다. GameObject > UI > EventSystem을 추가하세요.");
        }
    }

    private void Update()
    {
        // 현재 씬의 Block 태그 오브젝트를 감지
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            // 이미 처리된 블록은 제외
            if (!processedBlocks.Contains(block))
            {
                AddHoverEvents(block);
                processedBlocks.Add(block);
            }
        }

        // 파괴된 블록 제거
        processedBlocks.RemoveAll(block => block == null);
    }

    private void AddHoverEvents(GameObject block)
    {
        // Collider2D 확인
        if (block.GetComponent<Collider2D>() == null)
        {
            Debug.LogWarning($"블록 '{block.name}'에 Collider2D가 없습니다. 호버 이벤트가 작동하지 않을 수 있습니다.");
            block.AddComponent<BoxCollider2D>(); // 기본적으로 BoxCollider2D 추가
        }

        // EventTrigger 컴포넌트 추가
        EventTrigger trigger = block.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = block.AddComponent<EventTrigger>();
        }

        // 기존 트리거 제거 후 새로 추가 (중복 방지)
        trigger.triggers.Clear();

        // PointerEnter 이벤트 추가
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => { OnPointerEnter(block); });
        trigger.triggers.Add(pointerEnter);

        // PointerExit 이벤트 추가
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => { OnPointerExit(); });
        trigger.triggers.Add(pointerExit);
    }

    private void OnPointerEnter(GameObject block)
    {
        Debug.Log($"OnPointerEnter 호출됨: 블록 '{block.name}'");

        // BlockAbility 컴포넌트 가져오기
        BlockAbility blockAbility = block.GetComponent<BlockAbility>();
        if (blockAbility == null)
        {
            Debug.LogWarning($"블록 '{block.name}'에 BlockAbility 컴포넌트가 없습니다.");
            return;
        }

        if (infoPanel == null)
        {
            Debug.LogError("InfoPanel이 null입니다. Inspector에서 할당 확인하세요.");
            return;
        }

        // 패널 활성화 및 텍스트 업데이트
        infoPanel.SetActive(true);
        Debug.Log($"패널 활성화: Name={blockAbility.Name}, Description={blockAbility.Description}");

        if (nameText != null)
        {
            nameText.text = blockAbility.Name ?? "No Name";
        }
        else
        {
            Debug.LogWarning("NameText가 null입니다. Inspector에서 할당 확인하세요.");
        }

        if (descriptionText != null)
        {
            descriptionText.text = blockAbility.Description ?? "No Description";
        }
        else
        {
            Debug.LogWarning("DescriptionText가 null입니다. Inspector에서 할당 확인하세요.");
        }
    }

    private void OnPointerExit()
    {
        Debug.Log("OnPointerExit 호출됨");
        // 패널 비활성화
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}