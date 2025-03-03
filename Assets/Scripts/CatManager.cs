using System.Collections.Generic;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    public static CatManager instance;

    // List of all possible cats
    private List<Cat> allAvailableCats;

    private void Awake()
    {
        // Singleton pattern for global access
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public List<Cat> GetCats() { return allAvailableCats; }
    public void SetCats(List<Cat> cats) { allAvailableCats = new(cats); }
    // Method to find a cat by name
    public Cat GetCatByName(string name) { return allAvailableCats.Find(cat => cat.name == name); }
}
