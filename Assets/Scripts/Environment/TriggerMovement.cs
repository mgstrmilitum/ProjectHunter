using UnityEngine;

public class TriggerMovement : MonoBehaviour
{
    [SerializeField] bool triggerMovement;
    [SerializeField] bool triggerRotation;
    [SerializeField] bool triggerGrowth;
    [SerializeField] GameObject objectThatWillMove;

    private MovingObject toBeActivated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toBeActivated = objectThatWillMove.GetComponent<MovingObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.tag.Equals("Player"))
        //{
            if (triggerMovement)
            {
                toBeActivated.isMoving = true;
            }

            if (triggerRotation)
            {
                toBeActivated.isRotating = true;
            }

            if (triggerGrowth)
            {
                toBeActivated.isGrowing = true;
            }
        //}
    }
}
