using UnityEngine;

public class TutorialWeapons : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.tutorialUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player")
        {
            GameManager.Instance.tutorialUI.SetActive(false);
        }
    }
}
