using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public int levelToLoad;
    public bool isFinalLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (!isFinalLevel) GameManager.Instance.LoadLevel(levelToLoad);
        else
        {
            GameManager.Instance.OnWin();
        }
    }
}
