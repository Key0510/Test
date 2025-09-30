using UnityEngine;
using System.Collections.Generic;

public class GateSpawner : MonoBehaviour
{
    [Header("게이트 프리팹 3종 등")]
    public List<GameObject> gatePrefabs;

    [Header("스폰될 위치 2곳")]
    public List<Transform> spawnPoints;

    [Header("UI 패널")]
    public GameObject BrickSelectPanel;
    public GameObject RerollButton;
    public GameObject HealButton;
    public GameObject SpecialStagePanel; // 스테이지 4, 8, 12, 16, 20용 패널
    public GameObject FinalStagePanel;   // 스테이지 24용 패널
    public GameObject Maincanvas;

    private List<GameObject> spawnedGates = new List<GameObject>();
    public bool isRoundEnd = false;
    public bool flag = true;
    private PlayerAbility playerAbility;
    private StageManager stageManager;
    private BGMController bGMController;

    void Start()
    {
        playerAbility = FindObjectOfType<PlayerAbility>();
        stageManager = FindObjectOfType<StageManager>();
        bGMController = FindObjectOfType<BGMController>();
        if (playerAbility == null) Debug.LogError("[GateSpawner] PlayerAbility not found!");
        if (stageManager == null) Debug.LogError("[GateSpawner] StageManager not found!");
        if (BrickSelectPanel == null) Debug.LogWarning("[GateSpawner] BrickSelectPanel not assigned!");
        if (RerollButton == null) Debug.LogWarning("[GateSpawner] RerollButton not assigned!");
        if (HealButton == null) Debug.LogWarning("[GateSpawner] HealButton not assigned!");
        if (SpecialStagePanel == null) Debug.LogWarning("[GateSpawner] SpecialStagePanel not assigned!");
        if (FinalStagePanel == null) Debug.LogWarning("[GateSpawner] FinalStagePanel not assigned!");
        if (Maincanvas == null) Debug.LogWarning("[GateSpawner] Maincanvas not assigned!");
        else
        {
            Animator animator = Maincanvas.GetComponent<Animator>();
            if (animator == null) Debug.LogWarning("[GateSpawner] Animator not found on Maincanvas!");
        }
    }

    public void SpawnTwoRandomGates()
    {
        Debug.Log("[GateSpawner] SpawnTwoRandomGates 호출됨");
        // 라운드 시작 시 초기화
        if (playerAbility != null)
        {
            playerAbility.attackPower = 0;
            playerAbility.UpdateAttackPowerUI();
            playerAbility.critChance = 0.1f;
            playerAbility.UpdateCritChanceUI();
            playerAbility.shield = 0;
            playerAbility.UpdateShieldUI();
        }
        RemoveAllGates();


        // 스테이지 확인
        int stage = (stageManager != null) ? stageManager.currentStage : 1;
        bool isSpecialStage = (stage == 4 || stage == 8 || stage == 12 || stage == 16 || stage == 20);
        bool isFinalStage = (stage == 24);

        // UI 활성화 로직
        if (isFinalStage && FinalStagePanel != null && flag)
        {
            FinalStagePanel.SetActive(true);
            bGMController.PlayBGM(7);

        }
        else if (isSpecialStage && SpecialStagePanel != null && flag)
        {
            SpecialStagePanel.SetActive(true);
            playerAbility.Heal(30);
        }
        else if (flag)
        {
            if (BrickSelectPanel != null && !BrickSelectPanel.activeSelf)
            {
                BrickSelectPanel.SetActive(true);
                Debug.Log("[GateSpawner] BrickSelectPanel 활성화");
            }
            if (RerollButton != null && !RerollButton.activeSelf)
            {
                RerollButton.SetActive(true);
                Debug.Log("[GateSpawner] RerollButton 활성화");
            }
            if (HealButton != null && !HealButton.activeSelf)
            {
                HealButton.SetActive(true);
                Debug.Log("[GateSpawner] HealButton 활성화");
            }
            if (Maincanvas != null)
            {
                Animator animator = Maincanvas.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("isPanelEnable", true);
                    Debug.Log("[GateSpawner] Set Maincanvas Animator isPanelEnable to true");
                }
            }
        }
        else
        {
            Debug.Log($"[GateSpawner] 스테이지 {stage}에서 UI 비활성화 (flag: {flag})");
        }

        if (gatePrefabs.Count < 2 || spawnPoints.Count < 2)
        {
            Debug.LogWarning("[GateSpawner] gatePrefabs 또는 spawnPoints가 부족합니다");
            return;
        }

        // 스테이지에 따른 색상 계산
        Color gateColor = GetGateColor(stage);

        // 무작위 2개 선택 후 스폰
        List<GameObject> shuffledPrefabs = new List<GameObject>(gatePrefabs);
        Shuffle(shuffledPrefabs);
        List<GameObject> selectedPrefabs = shuffledPrefabs.GetRange(0, 2);

        for (int i = 0; i < 2; i++)
        {
            GameObject gate = Instantiate(selectedPrefabs[i], spawnPoints[i].position, Quaternion.identity);
            spawnedGates.Add(gate);
            ApplyColorToGate(gate, gateColor);
            Debug.Log($"[GateSpawner] 게이트 스폰: {gate.name} at {spawnPoints[i].position}");
        }
    }

    public void RemoveAllGates()
    {
        foreach (GameObject gate in spawnedGates)
        {
            if (gate != null) Destroy(gate);
        }
        spawnedGates.Clear();
        Debug.Log("[GateSpawner] 모든 게이트 제거됨");
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
    }

    private Color GetGateColor(int stage)
    {
        if (stage == 24)
        {
            Debug.Log("[GateSpawner] 스테이지 24 색상 적용: (56/255, 61/255, 140/255)");
            return new Color(56f / 255f, 61f / 255f, 140f / 255f, 1f);
        }
        Debug.Log("[GateSpawner] 기본 색상 적용: White");
        return Color.white;
    }

    private void ApplyColorToGate(GameObject gate, Color color)
    {
        if (gate == null) return;
        var spriteRenderers = gate.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sr in spriteRenderers)
        {
            sr.color = color;
        }
        Debug.Log($"[GateSpawner] 게이트 {gate.name}에 색상 적용: {color}");
    }

    public void ChangeFlag()
    {
        flag = false;
        if (Maincanvas != null)
        {
            Animator animator = Maincanvas.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isPanelEnable", false);
                Debug.Log("[GateSpawner] Set Maincanvas Animator isPanelEnable to false");
            }
            else
            {
                Debug.LogWarning("[GateSpawner] Animator not found on Maincanvas when setting isPanelEnable to false");
            }
        }
        Debug.Log("[GateSpawner] flag set to false");
    }

    public void Anima()
    {
        if (Maincanvas != null)
        {
            Animator animator = Maincanvas.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetBool("isPanelEnable", false);
                Debug.Log("[GateSpawner] Set Maincanvas Animator isPanelEnable to false via Anima");
            }
        }
    }
}