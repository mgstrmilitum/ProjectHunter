using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holywater : MonoBehaviour
{

    [Header("Explosion prefab")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private Vector3 explosionPratciles = new Vector3(0, 1, 0);
    [SerializeField] private GameObject audioSourcePrefab;

    [Header("Explsoion Settings")]
    [SerializeField] private float explosionDelay = 2f;
    [SerializeField] private float explosionforce = 700f;
    [SerializeField] private float explosionradius = 5f;

    [Header("Audio Effects")]
    [SerializeField] private AudioClip explosionAudioSource;
    [SerializeField] private AudioClip impactSound;

    private float countdown;
    private bool hasExploded;
    private AudioSource audio_;

    [Header("Gernade Damage")]
    [SerializeField] private int Damage;
    [SerializeField] private int BonkDamage;

    [Header("Gernade Explosion Layers")]
    public LayerMask explosionLayers;

    private Collider[] colliders;
    bool cankill=false;
    private void Start()
    {


        countdown=explosionDelay;
        audio_ = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!hasExploded)
        {
            countdown-=Time.deltaTime;
            if (countdown<=0f)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    void Explode()
    {
        GameObject explosioneffect = Instantiate(explosionEffectPrefab, transform.position+explosionPratciles, Quaternion.identity);
        Destroy(explosioneffect, 4f);

        PlaySound(explosionAudioSource);

        NearbyForceApply();

        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip)
    {
        GameObject audioSourceObject_= Instantiate(audioSourcePrefab, transform.position, Quaternion.identity);
        AudioSource inststantedAudio= audioSourceObject_.GetComponent<AudioSource>();
        inststantedAudio.clip=clip;
        inststantedAudio.spatialBlend=1;//3d sound
        inststantedAudio.Play();

        Destroy(audioSourceObject_,inststantedAudio.clip.length);

    }

    void NearbyForceApply()
    {
         colliders = Physics.OverlapSphere(transform.position, explosionradius, explosionLayers);
        foreach (Collider nearbyObject in colliders)
        {
            //Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            //if (rb != null)
            //{
            //    cankill=true;
            //    rb.AddExplosionForce(explosionforce, transform.position, explosionradius);
            //    TakeDamage dmg= gameObject.GetComponent<TakeDamage>();
            //    if(dmg != null)
            //    {

            //        dmg.takeDamage(Damage);
            //    }
            //    OnTriggerEnter(nearbyObject);
            //}
            TakeDamage dmg = nearbyObject.gameObject.GetComponent<TakeDamage>();
            if (dmg != null)
            {

                dmg.takeDamage(Damage);
            }
            OnTriggerEnter(nearbyObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.isTrigger)
        //{
        //    return;
        //}
        //StartCoroutine(TimerDelay());
        TakeDamage dmg= other.gameObject.GetComponent<TakeDamage>();
        if(dmg != null && cankill)
        {
            dmg.takeDamage(Damage);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        audio_.clip=impactSound;
        audio_.spatialBlend=1;
        audio_.Play();

        TakeDamage dmg=collision.gameObject.GetComponent<TakeDamage>();
        if(dmg != null)
        {
            dmg.takeDamage(BonkDamage);
        }
        
    }

    IEnumerator TimerDelay()
    {
        yield return new WaitForSeconds(4f);
        TakeDamage dmg = gameObject.GetComponent<TakeDamage>();
        if (dmg != null)
        {
            dmg.takeDamage(Damage);
        }
           

    }

}
