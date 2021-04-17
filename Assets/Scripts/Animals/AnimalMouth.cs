/**
 * Author  : Stephen Murchison
 * Email   : smurchison1@cnm.edu
 */
using UnityEngine;

/// <summary>
/// Provides the animal a way to eat and interact with food.
/// Tells the animal if they are touching a berry with their mouth.
/// </summary>
public class AnimalMouth : MonoBehaviour
{

    /// <summary>
    /// Is the mouth collider touching a berry
    /// </summary>
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
