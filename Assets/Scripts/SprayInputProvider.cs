using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

// here we fake the Vive controller output when its button is clicked. That way, we can
// tap into the SteamVR teleport code without too much modification. This should help with
// keeping the code backward compatible when, in the future, the SteamVR libraries are updated

public class SprayInputProvider : MonoBehaviour
{

    public VRInput VR_Input;
    public SprayShooter sprayCan;
    public event ClickedEventHandler TriggerClicked;
    public VREyeRaycaster eyecaster;

    private ClickedEventArgs theArgs;

    private void OnEnable()
    {
        // when this script is first enabled in the scene,
        // subscribe to events from vrInput
        VR_Input.OnClick += OnClick;
    }

    private void OnDisable()
    {
        // unsubscribe from events from vrInput
        VR_Input.OnClick -= OnClick;
    }

    void OnClick()
    {
        sprayCan.Fire();
    }

}
