using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneUIController : MonoBehaviour
{

    public TextMeshProUGUI NameText;
    public Slider LikelyhoodBar; 

    [SerializeField]
    Image likelyhoodColor;
    float gene_value;
    GeneticType gene_type;
    GeneticPerk gene_perk;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddGeneData(RatGene gene)
    {
        NameText.text = gene.fancy_name;
        LikelyhoodBar.value = gene.likelyhood;
        gene_value = gene.value;
        gene_type = gene.type;
        gene_perk = gene.perk;
        likelyhoodColor.color = (gene_value > 0.0f)?Color.green:Color.red;
    }
}
