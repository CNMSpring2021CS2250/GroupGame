using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMouth : MonoBehaviour
{

    public bool TouchingFood { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("berry"))
        {
            TouchingFood = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("berry"))
        {
            TouchingFood = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("berry"))
        {
            TouchingFood = false;
        }
    }
}
