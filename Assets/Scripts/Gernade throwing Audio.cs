using UnityEngine;

public class GernadethrowingAudio : MonoBehaviour
{
    private AudioSource audioSource;

    public static GernadethrowingAudio instance;//singleton to have acess to it from other places


    void Awake()
    {
        //Singleton setup!
        if(instance==null)
        {
            instance=this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        audioSource= GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float clipVolume=0.3f)
    {
        audioSource.PlayOneShot(clip,clipVolume);
    }
}
