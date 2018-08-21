using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	public void LoadGame()
    {
        SceneManager.LoadScene("game", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
