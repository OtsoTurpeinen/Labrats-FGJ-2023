using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatUIController : MonoBehaviour
{

    public RatGenetics Genetics;

    private RatStats rStats;
    public TextMeshProUGUI SpeedText;
    public TextMeshProUGUI AgilityText;
    public TextMeshProUGUI IntelligenceText;
    public TextMeshProUGUI PerksText;

    public GeneUIController Gene1;
    public GeneUIController Gene2;
    public GeneUIController Gene3;
    public GeneUIController Gene4;
    public GeneUIController Gene5;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (Genetics == null)
        {
            Genetics = new RatGenetics();
            Genetics.GenerateRandom(5);
            DisplayData(Genetics);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayData(RatGenetics genetics)
    {
        Genetics = genetics;
        rStats = genetics.GetStats();
        SpeedText.text = rStats.forward_speed + "";
        AgilityText.text = rStats.turn_rate + "";
        IntelligenceText.text = rStats.pathing + "";

        PerksText.text = "";

        if (genetics.HasPerk(GeneticPerk.DIRECT))
        {
            PerksText.text += "Tunnelvision ";
        }
        if (genetics.HasPerk(GeneticPerk.GLUTONY))
        {
            PerksText.text += "Gluttonous ";
        }
        if (genetics.HasPerk(GeneticPerk.JUMPING))
        {
            PerksText.text += "Jumpy ";
        }
        if (genetics.HasPerk(GeneticPerk.METABOLISIM))
        {
            PerksText.text += "Devourer ";
        }

        Gene1.AddGeneData(genetics.GetGene(0));
        Gene2.AddGeneData(genetics.GetGene(1));
        Gene3.AddGeneData(genetics.GetGene(2));
        Gene4.AddGeneData(genetics.GetGene(3));
        Gene5.AddGeneData(genetics.GetGene(4));
    }
}
