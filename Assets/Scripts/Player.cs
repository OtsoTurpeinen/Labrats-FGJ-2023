using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public bool is_human = false;
    public int score = 0;
    public bool draft_active = false;
    public List<RatGene> my_rat_genes;
    public GameObject my_rat;
    public List<List<RatGene>> roots;

    public void Init(bool bHuman) {
        is_human = bHuman;
        score = 0;
        draft_active = false;
        my_rat_genes = new List<RatGene>();
        my_rat = null;
        roots = new List<List<RatGene>>();
    }

    public void NextGeneration(List<RatGene> b) {
        roots.Add(my_rat_genes);
        List<RatGene> a = new List<RatGene>(my_rat_genes);
        my_rat_genes = new List<RatGene>();
        for (int i = 0; i < a.Count; i++)
        {
            float f = Random.value * (a[i].likelyhood + a[i].likelyhood) - a[i].likelyhood;
            if (f < 0.0f) {
                my_rat_genes.Add(a[i]);
            } else {
                my_rat_genes.Add(a[i]);
            }
        }
    }
}
