using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private Vector3 originalScale;
    [SerializeField] private float scaleFactor = 1.1f; // 확대 비율
    [SerializeField] private float transitionSpeed = 5f; // 확대/축소 속도

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 호버 시 패널 확대
        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale * scaleFactor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 벗어날 시 원래 크기로 복귀
        StopAllCoroutines();
        StartCoroutine(SmoothScale(originalScale));
    }

    private System.Collections.IEnumerator SmoothScale(Vector3 targetScale)
    {
        while (Vector3.Distance(rectTransform.localScale, targetScale) > 0.01f)
        {
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * transitionSpeed);
            yield return null;
        }
        rectTransform.localScale = targetScale;
    }
}