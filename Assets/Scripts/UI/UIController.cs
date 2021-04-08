﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private PauseMenu pauseMenu;

    private void Start()
        => pauseMenu = GameObject.FindGameObjectWithTag("UI").GetComponentInChildren<PauseMenu>();

    void Update()
    {
        // Toggle game pauses with escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu != null)
            {
                pauseMenu.TogglePause();
            }
        }
    }
}
