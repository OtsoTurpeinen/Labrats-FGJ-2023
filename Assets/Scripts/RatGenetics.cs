using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GeneticType
{
    FORWARD_SPEED = 0, //speed to next tile
    TURN_RATE = 1, //turning speed
    PATHING = 2, //increased memory length
    LAST
}

public enum GeneticPerk
{
    NONE = 0,
    JUMPING = 1, //has chance to jump over wall
    GLUTONY = 2, //if int range of food, will take path that is 'closest' to food.
    METABOLISIM = 3, //faster recovery from eating
    DIRECT = 4, //will allways take forward path if possible
    LAST
}

public struct RatGene
{

    public string fancy_name;
    public GeneticType type;
    public float value;
    public float likelyhood;
    public GeneticPerk perk;
}

public struct RatStats
{
    public float forward_speed;
    public float turn_rate;
    public float pathing;
}

public class RatGenetics
{
    List<RatGene> current_genes;
    public RatGenetics() {
        current_genes = new List<RatGene>();
    }

    public void AddGene(string name, GeneticType type,float value, float likelyhood, GeneticPerk perk) {
        RatGene gene = new RatGene();
        gene.fancy_name = name;
        gene.type = type;
        gene.value = value;
        gene.likelyhood = likelyhood;
        gene.perk = perk;
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
        r.pathing = 1.0f;
        foreach (RatGene gene in current_genes) {
            switch (gene.type)
            {
                case GeneticType.FORWARD_SPEED: // = 0, //speed to next tile
                    r.forward_speed += gene.value;
                    break;
                case GeneticType.TURN_RATE: // = 1, //turning speed
                    r.turn_rate += gene.value;
                    break;
                case GeneticType.PATHING: // = 2, //increased memory length
                    r.pathing += gene.value;
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


//deprecated, use player's new generation instead.
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
            GeneticPerk p = (GeneticPerk)Random.Range(0,(int)GeneticPerk.LAST);
            AddGene("Random",t,v,l,p);
        }
    }



    public void BuildFromList(List<RatGene> genes) {
        current_genes = genes;
    }

    public void ClearGenes()
    {
        current_genes.Clear();
    }

    public List<RatGene> GetGenes()
    {
        return current_genes;
    }
}
