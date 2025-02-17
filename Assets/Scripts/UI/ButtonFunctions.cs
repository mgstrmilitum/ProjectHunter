using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
  
   
   public void Restart()
    {
       string sceneName = SceneManager.GetActiveScene().name;

        SceneManager.LoadSceneAsync(sceneName);
        GameManager.Instance.StateUnpause();

    }
 
  public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
