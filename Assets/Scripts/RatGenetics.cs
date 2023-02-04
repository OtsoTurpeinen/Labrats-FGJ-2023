using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.JSONSerializeModule;
public enum GeneticType
{
    FORWARD_SPEED = 0, //speed to next tile
    TURN_RATE = 1, //turning speed
    GLUTONY = 2, //range to 'cheese' before 'forced pathing'
    METABOLISIM = 3, //speed to recover from eating 'cheese'
    PATHING = 4, //increased memory length
    DIRECT = 5, //higher chance of going forward if choise to turn is given.
    LAST
}

public enum GeneticPerk
{
    NONE = 0,
    JUMPING = 1,
    LAST
}

public struct RatGene
{
    public GeneticType type;
    public float value;
    public float likelyhood;
    public GeneticPerk perk;
}

public struct RatStats
{
    public float forward_speed;
    public float turn_rate;
    public float glutony;
    public float metabolisim;
    public float pathing;
    public float direct;
}

public class RatGenetics : MonoBehaviour
{
    List<RatGene> current_genes;
    public void Start() {
        current_genes = new List<RatGene>();
    }

    public void AddGene(GeneticType type,float value, float likelyhood, GeneticPerk perk) {
        RatGene gene = new RatGene();
        gene.type = type;
        gene.value = value;
        gene.likelyhood = likelyhood;
        gene.perk = GeneticPerk.NONE;
        current_genes.Add(gene);
    }

    public float GetGeneValue(GeneticType type) {
        float n = 1.0f;
        foreach (RatGene gene in current_genes) {
            if (gene.type == type) {
                n += gene.value;
            }
        }
        return n;
    }

    public RatGene GetGene(int index) {
        return current_genes[index];
    }

    public RatStats GetStats() {
        RatStats r = new RatStats();
        r.forward_speed = 1.0f;
        r.turn_rate = 1.0f;
        r.glutony = 1.0f;
        r.metabolisim = 1.0f;
        r.pathing = 1.0f;
        r.direct = 1.0f;
        foreach (RatGene gene in current_genes) {
            switch (gene.type)
            {
                case GeneticType.FORWARD_SPEED: // = 0, //speed to next tile
                    r.forward_speed += gene.value;
                    break;
                case GeneticType.TURN_RATE: // = 1, //turning speed
                    r.turn_rate += gene.value;
                    break;
                case GeneticType.GLUTONY: // = 2, //range to 'cheese' before 'forced pathing'
                    r.glutony += gene.value;
                    break;
                case GeneticType.METABOLISIM: // = 3, //speed to recover from eating 'cheese'
                    r.metabolisim += gene.value;
                    break;
                case GeneticType.PATHING: // = 4, //increased memory length
                    r.pathing += gene.value;
                    break;
                case GeneticType.DIRECT: // = 5, //higher chance of going forward if choise to turn is given.
                    r.direct += gene.value;
                    break;
                default:
                    break;
            }
        }
        return r;
    }

    public bool HasPerk(GeneticPerk perk) {
        foreach (RatGene gene in current_genes) {
            if (gene.perk == perk) {
                return true;
            }
        }
        return false;
    }

    public List<RatGene> Mix(List<RatGene> other) {
        List<RatGene> new_genes = new List<RatGene>();
        int i;
        if (other.Count > current_genes.Count) {
            for (i = 0; i < current_genes.Count; i++)
            {
                float f = Random.value * (current_genes[i].likelyhood + other[i].likelyhood) - current_genes[i].likelyhood;
                if (f < 0.0f) {
                    new_genes.Add(current_genes[i]);
                } else {
                    new_genes.Add(other[i]);
                }
            }
            while( i < other.Count) {
                new_genes.Add(other[i]);
                i++;
            }
        } else {
            for (i = 0; i < other.Count; i++)
            {
                float f = Random.value * (current_genes[i].likelyhood + other[i].likelyhood) - current_genes[i].likelyhood;
                if (f < 0.0f) {
                    new_genes.Add(current_genes[i]);
                } else {
                    new_genes.Add(other[i]);
                }
            }
            while( i < current_genes.Count) {
                new_genes.Add(current_genes[i]);
                i++;
            }
        }
        return new_genes;
    }

    public void GenerateRandom(int count) {
        for (int i = 0; i < count; i++)
        {
            GeneticType t = (GeneticType)Random.Range(0,(int)GeneticType.LAST);
            float v = Random.value * 2.0f - 1.0f;
            float l = Random.value;
            AddGene(t,v,l,GeneticPerk.NONE);
        }
    }

    public void BuildFromList(List<RatGene> genes) {
        current_genes = genes;
    }
}
