using UnityEngine;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioClip[] bgmList; // BGM 리스트
    private AudioSource audioSource; // 오디오 소스 컴포넌트

    void Start()
    {
        // AudioSource 컴포넌트 가져오기 (없으면 추가)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AudioSource 기본 설정
        audioSource.loop = true; // BGM 반복 재생
        audioSource.playOnAwake = false; // 시작 시 자동 재생 방지

        // BGM 리스트가 비어있는지 확인
        if (bgmList.Length == 0)
        {
            Debug.LogWarning("BGM 리스트가 비어있습니다. 인스펙터에서 BGM 클립을 추가하세요.");
        }
    }

    // 특정 인덱스의 BGM 재생 (버튼의 OnClick에서 호출)
    public void PlayBGM(int index)
    {
        if (index < 0 || index >= bgmList.Length || bgmList[index] == null)
        {
            Debug.LogError($"유효하지 않은 BGM 인덱스: {index}");
            return;
        }

        // 현재 재생 중인 BGM 정지
        audioSource.Stop();

        // 새로운 BGM 설정 및 재생
        audioSource.clip = bgmList[index];
        audioSource.Play();

        Debug.Log($"BGM 재생: {bgmList[index].name}");
    }
}