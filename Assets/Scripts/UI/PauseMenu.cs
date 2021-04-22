/**
 * Author  : Stephen Murchison
 * Email   : smurchison1@cnm.edu
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates the ability to hide and unhide a gameobject.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Whether the game is currently paused
    /// </summary>
    public bool IsPaused { get; private set; } = false;

    /// <summary>
    /// Causes the pause menu to be visible
    /// </summary>
    public void Pause()
    {
        // Update UI
        gameObject.SetActive(true);
        IsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Deactivates the pause menu
    /// </summary>
    public void Play()
    {
        // Update UI
        gameObject.SetActive(false);
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Toggles the pause state between true and false
    /// </summary>
    public void TogglePause()
    {
        if (this.IsPaused)
        {
            Play();
        }
        else
        {
            Pause();
        }
    }
}
