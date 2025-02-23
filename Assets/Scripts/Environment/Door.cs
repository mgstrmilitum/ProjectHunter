using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject[] models;
    [SerializeField] string buttonInfo;
    public bool isLocked;
    public bool doorCloses;
    bool inTrigger;
    bool hasOpened;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inTrigger)
        {
            if (Input.GetButtonDown("Interact") && !isLocked)
            {
                if (doorCloses && hasOpened) { }
                else
                {
                    foreach (GameObject model in models) model.SetActive(false);
                    GameManager.Instance.buttonInteract.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    { 
           if (other.isTrigger)
            return;

        if (isLocked)
        {
        }

        IOpen open = other.GetComponent<IOpen>();

        if (open != null)
        {
            inTrigger = true;

            if (!isLocked)
            {
                GameManager.Instance.buttonInteract.SetActive(true);
                GameManager.Instance.buttonInfo.text = buttonInfo;
                //model.SetActive(false);
            }
            else
            {
                if(open.hasKey())
                {
                    GameManager.Instance.buttonInteract.SetActive(true);
                    GameManager.Instance.buttonInfo.text = buttonInfo;

                    if (Input.GetButtonDown("Interact"))
                    {
                        foreach (GameObject model in models) model.SetActive(false);
                        GameManager.Instance.buttonInteract.SetActive(false);
                    }
                }
                else
                {
                    GameManager.Instance.buttonLocked.SetActive(true);
                    GameManager.Instance.buttonInfo.text = "Door locked";
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;

        IOpen open = other.GetComponent<IOpen>();

        if (open != null)
        {
            inTrigger = false;
            GameManager.Instance.buttonInteract.SetActive(false);
            GameManager.Instance.buttonLocked.SetActive(false);
            GameManager.Instance.buttonInfo.text = null;
            if(doorCloses)
                foreach (GameObject model in models) model.SetActive(true);
        }
    }
}
