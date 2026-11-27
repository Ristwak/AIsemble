using UnityEngine;

public class ScreenOrientationSetter : MonoBehaviour
{
    private void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;

        Screen.orientation = ScreenOrientation.AutoRotation;
    }
}