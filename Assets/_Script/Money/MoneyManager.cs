using UnityEngine;
using UnityEngine.UI; // UI 텍스트를 위해 필요
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance; // 싱글톤 패턴
    public int money = 0;

     [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] public bool enableMoneyIncrease = false; // 돈 증가 능력 활성화
    [SerializeField, Range(0f, 100f)] public float moneyIncreasePercentage = 0f; // 돈 증가 비율 (%)

    public event Action<int> OnMoneyChanged;

    private Animator playerAnimator;
    public GameObject MoneyFX;


    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        UpdateMoneyUI(); // 초기 UI 업데이트
    }

    // 돈 추가
    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("추가하려는 돈은 0 이상이어야 합니다.");
            return;
        }

        // 돈 증가 비율 적용
        int finalAmount = amount;
        if (enableMoneyIncrease)
        {
            finalAmount = Mathf.FloorToInt(amount * (1f + moneyIncreasePercentage / 100f));
            Debug.Log($"Money increased by {moneyIncreasePercentage}%: {amount} -> {finalAmount}");
        }

        MoneyFX.SetActive(false);
        MoneyFX.SetActive(true);

        money += finalAmount;
        UpdateMoneyUI();
        OnMoneyChanged?.Invoke(money); // 이벤트 호출
        Debug.Log($"돈 추가: {finalAmount}, 현재 돈: {money}");
    }

    // 돈 감소
    public bool SpendMoney(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        if (money >= amount)
        {
            money -= amount;
            UpdateMoneyUI();
            OnMoneyChanged?.Invoke(money); // 이벤트 호출
            return true;
        }
        else
        {
            return false;
        }
    }

    // 현재 돈 반환
    public int GetMoney()
    {
        return money;
    }

    // UI 업데이트
    private void UpdateMoneyUI()
    {
        moneyText.text = $": {money}";
    }

    public void TestAddMoney(int amount)
    {
        AddMoney(amount);
    }

    public void TestSpendMoney(int amount)
    {
        SpendMoney(amount);
    }

}