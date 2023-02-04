using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public bool is_human = false;
    public int score = 0;
    public bool darft_active = false;
    public List<RatGene> my_rat_genes;
    public GameObject my_rat;
    public List<List<RatGene>> roots;

    public void Init(bool human) {
        is_human = false;
        score = 0;
        darft_active = false;
        my_rat_genes = new List<RatGene>();
        my_rat = null;
        roots = new List<List<RatGene>>();
    }

    public void SetMyRat(GameObject rat) {
        my_rat = rat;

    }
}
