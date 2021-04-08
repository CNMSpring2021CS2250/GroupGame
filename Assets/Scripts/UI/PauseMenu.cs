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
    /// Causes the pause menu to be visible
    /// </summary>
    public void Pause()
        => gameObject.SetActive(true);

    /// <summary>
    /// Deactivates the pause menu
    /// </summary>
    public void Play()
        => gameObject.SetActive(false);
}
