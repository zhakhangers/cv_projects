using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderChangeValue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSliderChange() 
    {
        this.transform.Find("Text").gameObject.GetComponent<Text>().text =
                                            this.GetComponent<Slider>().value.ToString();
    
    }

}
