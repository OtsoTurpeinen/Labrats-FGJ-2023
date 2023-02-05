using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreController : MonoBehaviour
{

    public List<ScoreUI> Scores;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateOverallScores(List<Player> players)
    {
        List<Player> orderedList = players.OrderBy(x => x.OverallScore).ToList();
        for (int i = 0; i < orderedList.Count; i++)
        {
            Scores[i].ScoreText.text = orderedList[i].OverallScore + "";
            Scores[i].NameText.text = orderedList[i].Name;
        }
    }
}
