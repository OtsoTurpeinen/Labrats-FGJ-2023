using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneUIController : MonoBehaviour
{

    public TextMeshProUGUI NameText;
    public Slider LikelyhoodBar; 
    
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
    }
}
