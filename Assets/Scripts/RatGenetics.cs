using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.JSONSerializeModule;

public enum GeneticType
{
    FORWARD_SPEED, //speed to next tile
    TURN_RATE, //turning speed
    GLUTONY, //range to 'cheese' before 'forced pathing'
    METABOLISIM, //speed to recover from eating 'cheese'
    PATHING, //increased chances of staying on the 'right' path
}

struct RatGene
{
    public GeneticType gene_type;
    public float gene_value;
}
/* 
[Serializable]
public struct RatSerialized
{
    public float forward_speed;
    public float turn_rate;
    public float glutony;
    public float metabolisim;
    public float pathing;
} */

public class RatGenetics : MonoBehaviour
{
    List<RatGene> obtained_genes;
    public void Start() {
        obtained_genes = new List<RatGene>();
    }

    public void AddGene(GeneticType gene_type,float gene_value) {
        RatGene gene = new RatGene();
        gene.gene_type = gene_type;
        gene.gene_value = gene_value;
        obtained_genes.Add(gene);
    }

    public float GetGeneValue(GeneticType gene_type) {
        float n = 0.0f;
        foreach (RatGene gene in obtained_genes) {
            if (gene.gene_type == gene_type) {
                n += gene.gene_value;
            }
        }
        return n;
    }
/* 
    public string GetSerialized() {
        RatSerialized serialized_rat = new RatSerialized();
        serialized_rat.forward_speed = 0.0f;
        serialized_rat.turn_rate = 0.0f;
        serialized_rat.glutony = 0.0f;
        serialized_rat.metabolisim = 0.0f;
        serialized_rat.pathing = 0.0f;
        foreach (RatGene gene in obtained_genes) {
            switch (gene.gene_type)
            {
                case GeneticType.FORWARD_SPEED:
                serialized_rat.forward_speed += gene.gene_value;
                break;
                case GeneticType.TURN_RATE:
                serialized_rat.turn_rate += gene.gene_value;
                break;
                case GeneticType.GLUTONY:
                serialized_rat.glutony += gene.gene_value;
                break;
                case GeneticType.METABOLISIM:
                serialized_rat.metabolisim += gene.gene_value;
                break;
                case GeneticType.PATHING:
                serialized_rat.pathing += gene.gene_value;
                break;
                default:
                break;
            }
        }
        return JsonUtility.ToJson(serialized_rat);
    } */
}
