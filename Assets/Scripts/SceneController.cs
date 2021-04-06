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
    /// Advances the scene by one, using the build index number. If index goes beyond
    /// the last index, wrap back around to the first scene.
    /// </summary>
    public static void AdvanceScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1 % SceneManager.sceneCount;
        SceneManager.GetSceneAt(nextSceneIndex);
    }

    /// <summary>
    /// Goes back a scene, using the build index number. If index goes before
    /// the first index, wrap back around to the last scene.
    /// </summary>
    public static void GoBackScene()
    {
        int BackSceneIndex = SceneManager.GetActiveScene().buildIndex -1 % SceneManager.sceneCount;
        SceneManager.GetSceneAt(BackSceneIndex);
    }

    /// <summary>
    /// Gets a specific scene by name.
    /// </summary>
    /// <param name="name">The name of the scene to go to</param>
    public static void GetSceneByName(string name)
        => SceneManager.GetSceneByName(name);

    /// <summary>
    /// Gets a specific scene by the build index number
    /// </summary>
    /// <param name="SceneIndex">The scene build index number to go to</param>
    public static void GetSceneByIndex(int SceneIndex)
        => SceneManager.GetSceneAt(SceneIndex);

    /// <summary>
    /// Quits the unity game
    /// </summary>
    public static void QuitGame()
        => Application.Quit();
}
