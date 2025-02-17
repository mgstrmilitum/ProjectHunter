using UnityEngine;

public class DisappearingObject : MonoBehaviour
{
    [SerializeField] GameObject toBeDisappeared;
    [SerializeField] float disappearDelay;
    [SerializeField] float reappearDelay;

    bool toBeReappeared;
    bool disappearing;

    Color ogColor;
    float localDisappearDelay;
    float localReappearDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toBeReappeared = false;
        disappearing = false;
        localDisappearDelay = disappearDelay;
        localReappearDelay = reappearDelay;
        ogColor = toBeDisappeared.gameObject.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (disappearing)
        {
            toBeDisappeared.gameObject.GetComponent<Renderer>().material.color = Color.black;
            if (!toBeReappeared)
            {
                if (localDisappearDelay <= 0)
                {
                    toBeDisappeared.SetActive(false);
                    toBeReappeared = true;
                    localDisappearDelay = disappearDelay;
                    disappearing = false;
                }
                else
                {
                    localDisappearDelay -= Time.deltaTime;
                }
            }
        }

        // if the object is disappeared and waiting to be reappeared, starts a timer. When the timer ends, the object reappears.
        if (toBeReappeared)
        {
            toBeDisappeared.gameObject.GetComponent<Renderer>().material.color = ogColor;
            if (localReappearDelay <= 0)
            {
                toBeDisappeared.SetActive(true);
                toBeReappeared = false;
                localReappearDelay = reappearDelay;
            }
            else
            {
                localReappearDelay -= Time.deltaTime;
            }
        }
    }

    // when the player enters the trigger, starts a timer. When the timer is up, disappears the object.
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag.Equals("Player"))
        //{
        disappearing = true;
        //}
    }
}
