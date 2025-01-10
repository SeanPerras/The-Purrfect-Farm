using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    public static CatManager instance;

    // List of all possible cats
    public List<Cat> allAvailableCats = new();

    private void Awake()
    {
        // Singleton pattern for global access
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Method to find a cat by name
    public Cat GetCatByName(string name)
    {
        return allAvailableCats.Find(cat => cat.name == name);
    }
}
