using UnityEngine;

public class TutorialUIScript : MonoBehaviour
{
    public string TextDisplay;

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.tutorialText.text = TextDisplay;
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
