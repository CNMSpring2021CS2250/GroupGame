/**
 * Author   : Stephen Murchison
 * Email    : smurchison1@cnm.edu
 */
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates a scene managment script to advance or change game scenes.
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// Advances the scene by one, using the build index number.
    /// </summary>
    public void AdvanceScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }

    /// <summary>
    /// Goes back a scene, using the build index number.
    /// </summary>
    public void GoBackScene()
    {
        int BackSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        SceneManager.LoadScene(BackSceneIndex);
    }

    /// <summary>
    /// Gets a specific scene by name.
    /// </summary>
    /// <param name="name">The name of the scene to go to</param>
    public void GetSceneByName(string name)
        => SceneManager.LoadScene(name);

    /// <summary>
    /// Gets a specific scene by the build index number
    /// </summary>
    /// <param name="SceneIndex">The scene build index number to go to</param>
    public void GetSceneByIndex(int SceneIndex)
        => SceneManager.LoadScene(SceneIndex);

    /// <summary>
    /// Quits the unity game
    /// </summary>
    public void QuitGame()
        => Application.Quit();
}
