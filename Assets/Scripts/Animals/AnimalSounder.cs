using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AnimalSounder : MonoBehaviour
{

    public AudioClip AudioClip;

    private AudioSource AudioSource;

    void Start()
    {
        GetComponent<AudioSource>();
        StartCoroutine(MakeAnimalSounds());
    }

    private IEnumerator MakeAnimalSounds()
    {
        if (AudioClip != null)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(Random.Range(5, 15));
                AudioSource.PlayOneShot(AudioClip);
            }
        }
    }
}
