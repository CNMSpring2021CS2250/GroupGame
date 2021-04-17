using System.Collections;
using UnityEngine;

/// <summary>
/// Chooses which camera settings the user operates.
/// </summary>
[RequireComponent(typeof(MouseLook))]
[RequireComponent(typeof(SimpleCameraController))]
[RequireComponent(typeof(SmoothTranslation))]
public class CameraController : MonoBehaviour
{

    public KeyCode flyKey = KeyCode.F;

    /// <summary>
    /// Whether the user is using a FPS camera or a flying one.
    /// </summary>
    private bool FPSCamera = true;

    private GameObject Player;
    private SmoothTranslation transitioner;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        transitioner = GetComponent<SmoothTranslation>();
    }

    void Update()
    {
        if (Input.GetKeyDown(flyKey) && !transitioner.Transitioning())
        {
            ToggleCameraView();
        }
    }

    private void ToggleCameraView()
    {
        FPSCamera = !FPSCamera;
        // Toggle camera scripts
        if (FPSCamera)
        {
            StartCoroutine(TransitionCamToFPS());
        }
        else
        {
            GetComponent<SimpleCameraController>().enabled = true;
            SetEnabledFPSCamera(false);
            transform.parent = null;
        }
    }

    /// <summary>
    /// Transitions from the flying position to the FPS perspective
    /// </summary>
    private IEnumerator TransitionCamToFPS()
    {
        GetComponent<SimpleCameraController>().enabled = false;
        transitioner.StartTransition();

        // Wait for camera to tranlate to FPS perspective
        while (transitioner.Transitioning())
            yield return null;

        // Enable controls
        transform.parent = Player.transform;
        SetEnabledFPSCamera(true);
    }

    /// <summary>
    /// Sets whether the FPS camera components are enabled or not
    /// </summary>
    /// <param name="enabled">The bool to enable or disable the value. TRUE is enabled</param>
    private void SetEnabledFPSCamera(bool enabled)
    {
        Player.GetComponent<FPSInputJump>().enabled = enabled;
        Player.GetComponent<MouseLook>().enabled = enabled;
        GetComponent<MouseLook>().enabled = enabled;
    }
}
