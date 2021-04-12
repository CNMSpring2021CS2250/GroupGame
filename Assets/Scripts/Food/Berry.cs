using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berry : MonoBehaviour
{

    [Tooltip("The color when the berry is full")]
    public Color fullBerryColor = new Color(1f, 0f, .3f);
    [Tooltip("The color when the berry is empty")]
    public Color emptyBerryColor = new Color(0.5f, 0f, 1f);

    /// <summary>
    /// The trigger collider representing the food
    /// </summary>
    [HideInInspector]
    public Collider FoodCollider;

    // The berry's material
    private Material BerryMaterial;

    /// <summary>
    /// A vector pointing staight out of the berry
    /// </summary>
    public Vector3 BerryUpVector
    {
        get => FoodCollider.transform.up;
    }

    /// <summary>
    /// The center position of the food collider
    /// </summary>
    public Vector3 BerryCenterPosition
    {
        get => FoodCollider.transform.position;
    }

    /// <summary>
    /// The amount of food remaining in the berry
    /// </summary>
    public float FoodAmount { get; private set; }

    /// <summary>
    /// Whether the berry has any food remaining
    /// </summary>
    public bool HasFood
    {
        get => FoodAmount > 0f;
    }

    /// <summary>
    /// Attempts to remove food from the berry
    /// </summary>
    /// <param name="amount">The amount of the food to try to remove</param>
    /// <returns>The actual amount removed</returns>
    public float Feed(float amount)
    {
        // Track how much food was successfully taken (cannot take more than is available)
        float FoodTaken = Mathf.Clamp(amount, 0f, FoodAmount);

        // Subtract the food
        FoodAmount -= amount;

        if (FoodAmount <= 0)
        {
            // No food remaining
            FoodAmount = 0;

            // Disable the berry and food colliders
            FoodCollider.gameObject.SetActive(false);

            // Change the berry color to indicate that it is empty
            BerryMaterial.SetColor("_BaseColor", emptyBerryColor);
        }

        // Return the amount of food that was taken
        return FoodTaken;
    }

    /// <summary>
    /// Resets the berry
    /// </summary>
    public void ResetBerry()
    {
        // Refil the food
        FoodAmount = 1f;

        // Enable the berry and food colliders
        FoodCollider.gameObject.SetActive(true);

        // Change the berry clor to indicate that it is full
        BerryMaterial.SetColor("_BaseColor", fullBerryColor);
    }

    private void Awake()
    {
        // Find the berry's mesh renderer and get the main material
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        BerryMaterial = meshRenderer.material;

        // Find berry and food colliders
        FoodCollider = GetComponent<Collider>();
    }
}
