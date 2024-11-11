using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Plants/Plant Data")]

public class PlantData : ScriptableObject
{
    public string plantName;
    public GameObject plantPrefab;
    public Sprite[] growthStages;
    public float[] growthTime;
}
