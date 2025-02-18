using UnityEngine;

public class PlayerDropShadow : MonoBehaviour
{
    public GameObject shadow;
    public RaycastHit hit;
    public float offset;
    Vector3 originalScale;

    private void Awake()
    {
       originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector3.down);
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Vector3 hitPosition = hit.point;
            shadow.transform.position = hitPosition;

            Vector3 footPosition = new Vector3(transform.position.x, transform.position.y - offset, transform.position.z);
            float distanceToGround = Vector3.Distance(footPosition, hitPosition);

            //if(distanceToGround > offset)
            //{
            if(distanceToGround > offset) shadow.transform.localScale = originalScale / distanceToGround;
            else shadow.transform.localScale = originalScale;
            //}
            //else
            //{
                //shadow.transform.localScale = Vector3.one;
            //}
            Debug.Log(distanceToGround);
            //shadow.transform.localScale = distanceToGround.normalized;
            //shadow.transform.position = new Vector3(transform.position.x, hit.point.y + offset, transform.position.z);
        }
    }
}
