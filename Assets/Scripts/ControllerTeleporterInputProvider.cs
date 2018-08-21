using UnityEngine;
using System.Collections;
using VRStandardAssets.Utils;

// here we fake the Vive controller output when its button is clicked. That way, we can
// tap into the SteamVR teleport code without too much modification. This should help with
// keeping the code backward compatible when, in the future, the SteamVR libraries are updated

public class ControllerTeleporterInputProvider : MonoBehaviour {

    public VRInput VR_Input;
    public SteamVR_Teleporter theTeleporterComponent;
    public event ClickedEventHandler TriggerClicked;
    public VREyeRaycaster eyecaster;
    private ClickedEventArgs theArgs;

    void Start () {
        // grab a reference to the SteamVR teleporter Component
        if (theTeleporterComponent==null)
        {
            if(GetComponent<SteamVR_Teleporter>())
            {
                theTeleporterComponent = GetComponent<SteamVR_Teleporter>();
            }
        }

        // grab a reference to the eye raycaster to find out what we're looking at
        if (eyecaster == null)
        {
            if (GetComponent<VREyeRaycaster>())
            {
                eyecaster = GetComponent<VREyeRaycaster>();
            }
        }

    }

    void OnClick()
    {
        // we check with the eyecaster to make sure that the user's gaze is on an object
        // that's on the correct layer for teleportation
        if (eyecaster.isHit && eyecaster.hitLayer==LayerMask.NameToLayer("TeleportSafe")) // TODO: CHECK LAYER HERE!!
        {
            theArgs.controllerIndex = 0;
            theArgs.padX = eyecaster.HitPoint.x;
            theArgs.padY = eyecaster.HitPoint.z;

            OnTriggerClicked(theArgs);
        }
    }

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

    public virtual void OnTriggerClicked(ClickedEventArgs e)
    {
        if (TriggerClicked != null)
            TriggerClicked(this, e);
    }
}
