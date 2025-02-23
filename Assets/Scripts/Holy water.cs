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
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionradius);
        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionforce, transform.position, explosionradius);
                TakeDamage dmg= colliders[0].GetComponent<TakeDamage>();
                if(dmg != null)
                {
                    dmg.takeDamage(Damage);
                }
            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        TakeDamage dmg = other.GetComponent<TakeDamage>();
        if (dmg != null)
        {

            
            dmg.takeDamage(Damage);
            //Destroy(other.gameObject);
        }


    }
    //public GameObject explsoiveEffect;
    //public float delay = 3f;

    //public float explosiveforce = 10f;
    //public float radius = 20f;

    //void Start()
    //{
    //    Invoke("Explosion", delay);
    //}

    //private void Explode()
    //{
    //    //check nearby colliders
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

    //    //Apply them a force
    //    foreach(Collider near in colliders)
    //    {
    //        Rigidbody rig = near.GetComponent<Rigidbody>();
    //        if (rig != null )
    //        {
    //            rig.AddExplosionForce(explosiveforce, transform.position, radius, 1, ForceMode.Impulse);
    //        }
    //    }

    //    Instantiate(explosiveforce,transform.position, transform.rotation);
    //    Destroy(gameObject);
    //}
}
