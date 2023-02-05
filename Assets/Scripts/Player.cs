using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public bool is_human = false;
    //public int score = 0;
    public bool draft_active = false;
    public List<RatGene> my_rat_genes;
    public RatGenetics my_rat_genetics;
    public GameObject my_rat;
    public List<RatGenetics> roots;
    public RatUIController my_ratUI;
    public bool already_drafted;
    public Color color;
    public string Name;
    public int LastAddedScore;
    public int OverallScore;

    public bool reached_maze_end = false;

    public void Init(bool bHuman, RatUIController ratController) {
        is_human = bHuman;
        OverallScore = 0;
        LastAddedScore = 0;
        draft_active = false;
        my_rat_genes = new List<RatGene>();
        my_rat = null;
        roots = new List<RatGenetics>();

        my_rat_genetics = new RatGenetics();
        my_rat_genetics.GenerateRandom(5);
        already_drafted = false;
        my_ratUI = ratController;
        color = ratController.RatImage.color;
        Name = ratController.RatName.text;
    }

    public void NextGeneration(List<RatGene> b) {
        roots.Add(my_rat_genetics);
        List<RatGene> a = new List<RatGene>(my_rat_genetics.GetGenes());
        //Debug.Log("My rat genes: " + my_rat_genetics.GetGenes().Count);
        my_rat_genes = new List<RatGene>();
        //Debug.Log("Gene pool A: " + a.Count);
        for (int i = 0; i < a.Count; i++)
        {
            float f = Random.value * (a[i].likelyhood + b[i].likelyhood) - a[i].likelyhood;
            if (f < 0.0f) {
                my_rat_genes.Add(a[i]);
                //Debug.Log("Added Gene " + a[i].fancy_name + " with likelyhood " + a[i].likelyhood);
            } else {
                my_rat_genes.Add(b[i]);
                //Debug.Log("Added Gene " + b[i].fancy_name + " with likelyhood " + b[i].likelyhood);
            }
        }
        //Debug.Log("NEW RAT HAS GENES: " + my_rat_genes.Count);
        my_rat_genetics.BuildFromList(my_rat_genes);
        my_ratUI.DisplayData(my_rat_genetics);
    }

    public void AddScore(int i)
    {
        LastAddedScore = i;
        OverallScore += i;
    }
}
