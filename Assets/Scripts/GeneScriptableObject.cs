using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Gene", order = 1)]
public class GeneScriptableObject : ScriptableObject
{
    public string fancy_name = "Name Me!";
    public GeneticType type = GeneticType.FORWARD_SPEED;
    public float value = 1.0f;
    public float likelyhood = 1.0f;
    public GeneticPerk perk = GeneticPerk.NONE;
}