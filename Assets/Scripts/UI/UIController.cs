/**
 * Author  : Stephen Murchison
 * Email   : smurchison1@cnm.edu
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls in-game UI elements, such as starting and stopping aspects of the UI.
/// </summary>
public class UIController : MonoBehaviour
{
    private PauseMenu pauseMenu;

    private void Start()
    {
        pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<PauseMenu>();
        SetupMenuStates();
    }

    void Update()
    {
        // Toggle game pauses with escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu != null)
                pauseMenu.TogglePause();
        }
    }

    /// <summary>
    /// Make sure all the menus are in correct states when the user hits play.
    /// </summary>
    private void SetupMenuStates()
    {
        if (pauseMenu != null)
            pauseMenu.Play();
    }
}
