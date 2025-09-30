using UnityEngine;

public class SpeedUpButton : MonoBehaviour
{
    public void OnSpeedUpButtonClicked()
    {
        Ball.SetMinSpeedTo20();
        Debug.Log("[SpeedUpButton] Called Ball.SetMinSpeedTo20 to set minSpeed of all balls to 20.");
    }
}