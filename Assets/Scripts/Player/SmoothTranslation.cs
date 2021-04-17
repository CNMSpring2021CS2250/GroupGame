using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Smoothly transitions the gameobjects transform to another position.
/// </summary>
public class SmoothTranslation : MonoBehaviour
{
    [Tooltip("The position where the object will end up after the transition")]
    public Transform endPos;

    [Tooltip("The time in seconds to translate from the current pos to the end pos")]
    public float timeToTransition = 0.5f;

    /// <summary>
    /// The starting position of the transitioning object
    /// </summary>
    private Transform startPos;

    /// <summary>
    /// Whether the object is currently transitioning
    /// </summary>
    private bool TransitioningPos = false;
    private bool TransitioningRot = false;

    private void Start()
        => startPos = GetComponent<Transform>();

    /// <summary>
    /// Start the transition from the start pos to the end pos.
    /// </summary>
    public void StartTransition()
    {
        startPos = GetComponent<Transform>();
        if (!TransitioningRot && !TransitioningPos)
        {
            StartCoroutine(TransitionPos());
            StartCoroutine(TransitionRotation());
        }
    }

    /// <summary>
    /// Whether a transition is under way
    /// </summary>
    /// <returns>True if the transition is happening now</returns>
    public bool Transitioning()
        => (TransitioningRot || TransitioningPos);

    /// <summary>
    /// Transitions the current rotation to the end points rotation
    /// </summary>
    private IEnumerator TransitionRotation()
    {
        endPos.rotation = new Quaternion(0, transform.rotation.x, 0, 0);

        TransitioningRot = true;
        float startTime = Time.time;
        float fractionComplete = 0;

        while (fractionComplete < 1)
        {
            fractionComplete = ((Time.time - startTime) / timeToTransition) * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, endPos.rotation, fractionComplete);

            if (Vector3.Distance(transform.position, endPos.position) < 0.1)
            {
                transform.rotation = endPos.rotation;
                break;
            }
            yield return null;
        }

        TransitioningRot = false;
    }

    /// <summary>
    /// Transitions the curre
    /// </summary>
    /// <returns></returns>
    private IEnumerator TransitionPos()
    {
        TransitioningPos = true;
        float startTime = Time.time;
        float fractionComplete = 0;

        while (fractionComplete < 1)
        {
            fractionComplete = ((Time.time - startTime) / timeToTransition) * Time.deltaTime;
            transform.position = Vector3.Slerp(startPos.position, endPos.position, fractionComplete);

            if (Vector3.Distance(transform.position, endPos.position) < 0.1)
            {
                transform.position = endPos.position;
                break;
            }
            yield return null;
        }

        TransitioningPos = false;
    }
}
