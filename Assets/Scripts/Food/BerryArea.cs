using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryArea : MonoBehaviour
{
    // The diameter of the area where the agent and berrys can be used for observing the
    // relative distance from the agent to the berry
    public const float AreaDiameter = 20f;

    // The list of all the berry plants in this berry area(berry plants have multiple berrys)
    private List<GameObject> berryPlants;
    
    // A lookup dictionary for looking up a berry from a food collider
    private Dictionary<Collider, Berry> FoodBerryDictionary;

    /// <summary>
    /// The list of all berrys in the berry area
    /// </summary>
    public List<Berry> Berries { get; private set; }

    /// <summary>
    /// Reset the berrys and berry plants
    /// </summary>
    public void ResetBerries()
    {
        // Rotate each berry arround the Y axis and subtly around the X and Z
        foreach (GameObject BerryPlant in berryPlants)
        {
            float xRotation = Random.Range(-5f, 5f);
            float yRotation = Random.Range(-180f, 180f);
            float zRotation = Random.Range(-5f, 5f);
            BerryPlant.transform.localRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
        }

        // Rest each berry
        foreach (Berry berry in Berries)
        {
            berry.ResetBerry();
        }
    }

    /// <summary>
    /// Gets the <see cref="Berry"/> that a food collider belongs to
    /// </summary>
    /// <param name="collider">The food collider</param>
    /// <returns>The matching berry</returns>
    public Berry GetBerryFromFood(Collider collider)
        => FoodBerryDictionary[collider];

    /// <summary>
    /// Called when the area wakes up
    /// </summary>
    private void Awake()
    {
        // Initialize variables
        berryPlants = new List<GameObject>();
        FoodBerryDictionary = new Dictionary<Collider, Berry>();
        Berries = new List<Berry>();
    }

    /// <summary>
    /// Called when the game starts
    /// </summary>
    private void Start()
    {
        // Find all berrys that are children of this GameObject/Transform
        FindChildBerries(transform);
    }

    private void Update()
    {
        if (!Berries[0].HasFood)
        {
            ResetBerries();
        }
    }

    /// <summary>
    /// Recursively finds all berrys and berry plants that are children of a parent transform
    /// </summary>
    /// <param name="transform">The parent of the children to check</param>
    private void FindChildBerries(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);

            if (child.CompareTag("berry_plant"))
            {
                // Found a berry plant, add it to the berry plants list
                berryPlants.Add(child.gameObject);

                // Look for berrys within the berry plant
                FindChildBerries(child);
            }
            else
            {
                // Not a berry plant, look for a berry component
                Berry berry = child.GetComponent<Berry>();
                if (berry != null)
                {
                    // Found a berry! Add it to the berrys list
                    Berries.Add(berry);

                    // Add the food collider to the lookup dictionary
                    FoodBerryDictionary.Add(berry.FoodCollider, berry);

                    // Note: There are no berry that are children of other berrys
                }
                else
                {
                    // berry component not found, so check children
                    FindChildBerries(child);
                }
            }
        }
    }
}
