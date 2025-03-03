using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCatStats", menuName = "Cats/Cat Stats")]
public class CatStats : ScriptableObject
{

    public Sprite sprite;
    public Sprite[] walkSprites;
    public int value, strength, speed, defense;
    
}
