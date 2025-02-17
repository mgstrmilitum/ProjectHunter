using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.LoadLevel(1);
    }
}
