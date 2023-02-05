using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    Player winner;

    [SerializeField]
    RatUIController final;
    [SerializeField]
    RatUIController first;
    [SerializeField]
    RatUIController second;
    [SerializeField]
    RatUIController third;

    public void SetupWinner(Player winner) {
        this.winner = winner;


        first.DisplayData(winner.roots[0]);
        first.RatName.text = winner.Name;
        first.RatImage.color = winner.color;

        second.DisplayData(winner.roots[1]);
        second.RatName.text = winner.Name;
        second.RatImage.color = winner.color;

        third.DisplayData(winner.roots[2]);
        third.RatName.text = winner.Name;
        third.RatImage.color = winner.color;
        
        final.DisplayData(winner.my_rat_genetics);
        final.RatName.text = winner.Name;
        final.RatImage.color = winner.color;
        final.Highlight.SetActive(true);
    }
}
