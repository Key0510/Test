using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameQuit : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("[GameQuitButton] Game quit requested.");
        
        #if UNITY_EDITOR
        // Unity 에디터에서 플레이 모드 중지
        EditorApplication.isPlaying = false;
        #else
        // 빌드된 게임 종료
        Application.Quit();
        #endif
    }
}