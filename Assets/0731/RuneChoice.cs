using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // UI 컴포넌트 접근용
using TMPro;           // TextMeshPro 사용 시

public class RuneChoice : MonoBehaviour
{
    // 선택된 룬 목록
    public List<GameObject> selectedRunes = new List<GameObject>();

    // 오른쪽 하단에 선택된 룬 아이콘을 표시할 부모 오브젝트
    public Transform rightPanelParent;

    // 룬 아이콘 UI 프리팹 (이미지 + 텍스트 포함된 UI 프리팹으로 준비)
    public GameObject runeIconPrefab;

    /// <summary>
    /// 룬을 선택했을 때 호출하는 함수
    /// </summary>
    /// <param name="rune">선택된 룬 오브젝트</param>
    public void AddRune(GameObject rune)
    {
        if (!selectedRunes.Contains(rune))
        {
            selectedRunes.Add(rune);

            // => 여기서 UI 아이콘 생성 코드 추가
            CreateRuneIconUI(rune);

            Debug.Log($"{rune.name} 룬이 선택되어 리스트에 추가됨.");
        }
        else
        {
            Debug.Log($"{rune.name} 룬은 이미 선택된 상태입니다.");
        }
    }

    /// <summary>
    /// RightPanel에 룬 아이콘 UI를 생성 및 세팅하는 함수
    /// </summary>
    /// <param name="rune">아이콘화할 룬 오브젝트</param>
    private void CreateRuneIconUI(GameObject rune)
    {
        if (runeIconPrefab == null || rightPanelParent == null)
        {
            Debug.LogError("runeIconPrefab or rightPanelParent가 할당되지 않았습니다.");
            return;
        }

        // 프리팹 인스턴스화 및 부모 지정
        GameObject icon = Instantiate(runeIconPrefab, rightPanelParent);

        // 아이콘 내부 Image 컴포넌트에 룬의 스프라이트 및 색상 할당
        Image iconImage = icon.GetComponentInChildren<Image>();
        if (iconImage != null)
        {
            Sprite runeSprite = rune.GetComponent<SpriteRenderer>()?.sprite;
            if (runeSprite != null)
            {
                iconImage.sprite = runeSprite;
                iconImage.color = rune.GetComponent<SpriteRenderer>().color; // 색상 적용
                Debug.Log($"Icon for {rune.name} set with color {rune.GetComponent<SpriteRenderer>().color}");
            }
            else
            {
                iconImage.color = Color.white; // 스프라이트가 없으면 기본 색상
                Debug.LogWarning($"룬 {rune.name}에 SpriteRenderer 또는 스프라이트가 없습니다.");
            }
        }

        // 아이콘 텍스트(이름) 세팅 (TextMeshPro 사용 시)
        TextMeshProUGUI iconText = icon.GetComponentInChildren<TextMeshProUGUI>();
        if (iconText != null)
        {
            iconText.text = rune.name;
        }
    }
}