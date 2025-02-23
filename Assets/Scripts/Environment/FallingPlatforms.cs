using System.Collections;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    public bool isFalling;
    float fallSpeed = 0f;
    public Transform[] platforms;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isFalling)
        {
            fallSpeed += Time.deltaTime / 10;

            transform.position = new Vector3(transform.position.x, transform.position.y - fallSpeed, transform.position.z);
            StartCoroutine(StartFalling());
        }
    }

    IEnumerator StartFalling()
    {
        yield return new WaitForSeconds(3f);
        foreach (Transform t in platforms)
        {
            Destroy(t.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {

        }
    }
}
