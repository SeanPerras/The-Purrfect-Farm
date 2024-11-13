using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewExpeditionData", menuName = "Expeditions/Expedition Data")]
public class ExpeditionData : ScriptableObject
{
    public string expeditionName;
    public float expeditionTime;
    //public List<GameObject> expeditionRewards = new List<GameObject>();
}
