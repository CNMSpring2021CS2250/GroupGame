﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berry : MonoBehaviour
{

    [Tooltip("The color when the flower is full")]
    public Color fullFlowerColor = new Color(1f, 0f, .3f);
    [Tooltip("The color when the flower is empty")]
    public Color emptyFlowerColor = new Color(0.5f, 0f, 1f);

    /// <summary>
    /// The trigger collider representing the nectar
    /// </summary>
    [HideInInspector]
    public Collider nectarCollider;

    // The flower's material
    private Material flowerMaterial;

    /// <summary>
    /// A vector pointing staight out of the flower
    /// </summary>
    public Vector3 FlowerUpVector
    {
        get => nectarCollider.transform.up;
    }

    /// <summary>
    /// The center position of the nectar collider
    /// </summary>
    public Vector3 FlowerCenterPosition
    {
        get => nectarCollider.transform.position;
    }

    /// <summary>
    /// The amount of nectar remaining in the flower
    /// </summary>
    public float NectarAmount { get; private set; }

    /// <summary>
    /// Whether the flower has any nectar remaining
    /// </summary>
    public bool HasNectar
    {
        get => NectarAmount > 0f;
    }

    /// <summary>
    /// Attempts to remove nectar from the flower
    /// </summary>
    /// <param name="amount">The amount of the nectar to try to remove</param>
    /// <returns>The actual amount removed</returns>
    public float Feed(float amount)
    {
        // Track how much nectar was successfully taken (cannot take more than is available)
        float nectarTaken = Mathf.Clamp(amount, 0f, NectarAmount);

        // Subtract the nectar
        NectarAmount -= amount;

        if (NectarAmount <= 0)
        {
            // No nectar remaining
            NectarAmount = 0;

            // Disable the flower and nectar colliders
            nectarCollider.gameObject.SetActive(false);

            // Change the flower color to indicate that it is empty
            flowerMaterial.SetColor("_BaseColor", emptyFlowerColor);
        }

        // Return the amount of nectar that was taken
        return nectarTaken;
    }

    /// <summary>
    /// Resets the flower
    /// </summary>
    public void ResetFlower()
    {
        // Refil the nectar
        NectarAmount = 1f;

        // Enable the flower and nectar colliders
        nectarCollider.gameObject.SetActive(true);

        // Change the flower clor to indicate that it is full
        flowerMaterial.SetColor("_BaseColor", fullFlowerColor);
    }

    private void Awake()
    {
        // Find the flower's mesh renderer and get the main material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        flowerMaterial = meshRenderer.material;

        // Find flower and nectar colliders
        nectarCollider = GetComponent<Collider>();
    }
}