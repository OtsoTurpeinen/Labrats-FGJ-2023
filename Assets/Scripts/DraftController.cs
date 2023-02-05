using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DraftController : MonoBehaviour
{
    public List<RatUIController> PlayerRatUIs;
    public List<RatGenetics> DraftRats;

    public List<RatUIController> DraftRatUIs;

    private Player currentDrafter;
    
    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDraft()
    {
        DraftRats = new List<RatGenetics>();
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        GenerateDraftRats();
        int i = 0;
        
        foreach (var player in GameController.Instance.players)
        {
            player.my_ratUI = PlayerRatUIs[i];
            player.my_ratUI.LastScore.text = player.LastAddedScore+"";
            i++;
        }
        Player candidate = GameController.Instance.players.First();
        
        foreach (var player in GameController.Instance.players)
        {
            if (candidate.OverallScore >= player.OverallScore)
            {
                candidate = player;
                
            }
        }

        currentDrafter = candidate;
        currentDrafter.my_ratUI.HighlightRat();
    }
    
    
    private void GenerateDraftRats()
    {
        for (int j = 0; j < DraftRats.Count; j++)
        {
            DraftRats[j].GenerateRandom(5);
            DraftRatUIs[j].DisplayData(DraftRats[j]);
        }

    }

    public void OnClick(int value)
    {
        currentDrafter.NextGeneration(DraftRats[value].GetGenes());
        DraftRatUIs[value].DisableRat();
        currentDrafter.already_drafted = true;
        FindNextPlayerInOrder();   
    }

    public void FindNextPlayerInOrder()
    {
        Player candidate = GameController.Instance.players.First();

        bool allDrafted = false;
        foreach (var player in GameController.Instance.players)
        {
            allDrafted = true;
            if (!player.already_drafted)
            {
                if (player.OverallScore >= currentDrafter.OverallScore)
                {            
                    if (candidate.OverallScore >= player.OverallScore)
                    {
                        candidate = player;
                    }
                }
            }
            
        }

        currentDrafter.my_ratUI.DisableRat();
        if (allDrafted)
        {
            foreach (var rat in PlayerRatUIs)
            {
                rat.UndisableRat();
            }
            GameController.Instance.GameLoopStep();
        }
        currentDrafter = candidate;
        currentDrafter.my_ratUI.HighlightRat();
        currentDrafter.already_drafted = true;

    }
    
}
