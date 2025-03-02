using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public int levelToLoad;
    public bool isFinalLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (!isFinalLevel)
        {
            OnLevelEnd();
            GameManager.Instance.DisplayLevelStats();
        }
        else
        {
            GameManager.Instance.OnWin();
        }
    }

    private void OnLevelEnd()
    {
        GameManager.Instance.statsSO.currentHealth = GameManager.Instance.playerScript.currentHealth;
        GameManager.Instance.statsSO.currentShield = GameManager.Instance.playerScript.currentShield;
        GameManager.Instance.statsSO.currentSpecial = GameManager.Instance.playerScript.currentAp;
        GameManager.Instance.statsSO.currentGarlic = GameManager.Instance.playerScript.numGarlic;
        GameManager.Instance.statsSO.currentStage++;
        
    }
}
