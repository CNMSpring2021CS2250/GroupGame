using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketBooth : MonoBehaviour
{

    public AudioClip[] ticketSounds;

    private AudioSource ticketBoothAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        ticketBoothAudioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ticketBoothAudioSource.PlayOneShot(ticketSounds[Random.Range(0, ticketSounds.Length)]);
        }
    }
}
