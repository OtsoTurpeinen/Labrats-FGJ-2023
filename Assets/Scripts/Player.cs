using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public bool is_human = false;
    public int score = 0;
    public bool draft_active = false;
    public List<RatGene> my_rat_genes;
    public RatGenetics my_rat_genetics;
    public GameObject my_rat;
    public List<List<RatGene>> roots;
    public RatUIController my_ratUI;
    public bool already_drafted;

    public void Init(bool bHuman) {
        is_human = bHuman;
        score = 0;
        draft_active = false;
        my_rat_genes = new List<RatGene>();
        my_rat = null;
        roots = new List<List<RatGene>>();
        /*
        float tempScore = Random.value * 10f;
        score = (int) tempScore;
        Debug.Log(score);
        */
        my_rat_genetics = new RatGenetics();
        my_rat_genetics.GenerateRandom(5);
        already_drafted = false;
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
