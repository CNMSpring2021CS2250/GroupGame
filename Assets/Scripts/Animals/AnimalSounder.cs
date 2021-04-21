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
        AudioSource = GetComponent<AudioSource>();
        AudioSource.spatialBlend = 0.91f;

        StartCoroutine(MakeAnimalSounds());
    }

    private IEnumerator MakeAnimalSounds()
    {
        if (AudioClip != null)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(Random.Range(30, 70));
                AudioSource.PlayOneShot(AudioClip);
            }
        }
    }
}
