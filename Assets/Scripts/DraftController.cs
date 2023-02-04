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
        DraftRats = new List<RatGenetics>();
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        DraftRats.Add(new RatGenetics());
        GenerateDraftRats();
        int i = 0;
        Debug.Log(GameController.Instance.players.Count);
        
        foreach (var player in GameController.Instance.players)
        {
            player.my_ratUI = PlayerRatUIs[i];
            player.my_ratUI.LastScore.text = player.score+"";
            i++;
        }
        Player candidate = GameController.Instance.players.First();
        
        foreach (var player in GameController.Instance.players)
        {
            if (candidate.score >= player.score)
            {
                candidate = player;
                
            }
         
            
        }

        currentDrafter = candidate;
        currentDrafter.my_ratUI.HighlightRat();
    }
    
    // Update is called once per frame
    void Update()
    {
        
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
        Debug.Log(DraftRats[value].GetGenes().Count);
        currentDrafter.my_rat_genetics.Mix(DraftRats[value].GetGenes());
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
                Debug.Log(player.score);
                if (player.score >= currentDrafter.score)
                {            
                    if (candidate.score >= player.score)
                    {
                        candidate = player;
                    }
                }
            }
            
        }

        if (allDrafted)
        {
            //TODO: Move to maze running phase
        }
        currentDrafter.my_ratUI.DisableRat();
        currentDrafter = candidate;
        currentDrafter.my_ratUI.HighlightRat();
        currentDrafter.already_drafted = true;

    }
    
}
