using UnityEngine;

public class Credits : MonoBehaviour
{
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation()
    {
        anim.SetTrigger("CreditsRoll");
    }
}
