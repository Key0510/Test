using UnityEngine;

public class RandomActiveButtons: MonoBehaviour
{
    void OnEnable()
    {
        // 1. 모든 자식 비활성화
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        int activeCount = 3;
        int childCount = transform.childCount;

        // 2. 자식 인덱스 리스트 생성
        int[] indices = new int[childCount];
        for (int i = 0; i < childCount; i++)
            indices[i] = i;

        // 3. 리스트 섞기 (Fisher-Yates 방식)
        for (int i = 0; i < childCount; i++)
        {
            int randomIndex = Random.Range(i, childCount);
            int temp = indices[i];
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        // 4. 무작위 3개 자식 활성화
        for (int i = 0; i < Mathf.Min(activeCount, childCount); i++)
        {
            transform.GetChild(indices[i]).gameObject.SetActive(true);
        }
    }
}
