using UnityEngine;

public class DisappearingObject : MonoBehaviour
{
    [SerializeField] GameObject toBeDisappeared;
    [SerializeField] float disappearDelay;
    [SerializeField] float reappearDelay;

    bool toBeReappeared;

    float localDisappearDelay;
    float localReappearDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // if the object is disappeared and waiting to be reappeared, starts a timer. When the timer ends, the object reappears.
        if (toBeReappeared)
        {
            if (localReappearDelay <= 0)
            {
                toBeDisappeared.SetActive(true);
                toBeReappeared = false;
                localDisappearDelay = reappearDelay;
            }
            else
            {
                reappearDelay -= Time.deltaTime;
            }
        }
    }

    // when the player enters the trigger, starts a timer. When the timer is up, disappears the object.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            if (!toBeReappeared)
            {
                if (localDisappearDelay <= 0)
                {
                    toBeDisappeared.SetActive(false);
                    toBeReappeared = true;
                    localDisappearDelay = disappearDelay;
                }
                else
                {
                    disappearDelay -= Time.deltaTime;
                }
            }
        }
    }
}
