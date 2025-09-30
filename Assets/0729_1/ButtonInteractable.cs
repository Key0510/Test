using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractable : MonoBehaviour
{
    private MoneyManager moneyManager;
    private Button button;
    [SerializeField] private int initialGold = 2; // 초기 필요한 금액, 인스펙터에서 설정
    private int gold; // 현재 필요한 금액
    private bool wasInteractable = false; // 이전 프레임의 interactable 상태
    [SerializeField] private TextMeshProUGUI goldText; // 금액을 표시할 TMP 필드

    void Awake()
    {
        // 컴포넌트 초기화
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError($"Button component not found on {gameObject.name}");
        }
    }

    void OnEnable()
    {
        // MoneyManager 찾기
        moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager == null)
        {
            Debug.LogError("MoneyManager component not found in scene!");
        }

        // 상태 완전 초기화
        gold = initialGold;
        wasInteractable = false; // interactable 상태 초기화
        UpdateGoldText();

        // 버튼 클릭 이벤트 연결
        if (button != null)
        {
            button.onClick.RemoveAllListeners(); // 기존 리스너 제거
            button.onClick.AddListener(SpendGold);
            button.interactable = moneyManager != null && moneyManager.money >= gold; // 초기 interactable 설정
        }

        Debug.Log($"OnEnable: Initialized gold to {gold}, interactable={button.interactable}");
    }

    void OnDisable()
    {
        // 버튼 클릭 이벤트 제거
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    void Update()
    {
        if (moneyManager == null || button == null) return;

        // 버튼 상호작용 가능 여부 갱신
        bool isInteractable = moneyManager.money >= gold;
        if (button.interactable != isInteractable)
        {
            button.interactable = isInteractable;
            Debug.Log($"Button interactable changed: {isInteractable}, gold={gold}, money={moneyManager.money}");
        }
        wasInteractable = isInteractable;

        // 금액 UI 갱신
        UpdateGoldText();
    }

    // 금액 지출 및 두 배 증가
    private void SpendGold()
    {
        moneyManager.TestSpendMoney(gold);
        
            gold *= 2; // 필요한 금액 두 배 증가
            UpdateGoldText();
            Debug.Log($"SpendGold: New gold requirement = {gold}");
    }

    // 금액 UI 업데이트
    private void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = $"X{gold}G";
        }
        else
        {
            Debug.LogWarning($"goldText is not assigned in ButtonInteractable on {gameObject.name}");
        }
    }

    // 인스펙터에서 initialGold 유효성 검사
    void OnValidate()
    {
        if (initialGold <= 0)
        {
            Debug.LogWarning($"initialGold should be positive in {gameObject.name}. Setting to 1.");
            initialGold = 1;
        }
    }
}