using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class TetSlot : MonoBehaviour {

    public bool isFilled=false;
        
    public void UpdateColor(Color color)
    {
        GetComponent<RawImage>().color = color;
    }

    public void UpdateToBlack()
    {
        GetComponent<RawImage>().color = Color.black;
        isFilled = false;
    }

   

    public void UpdateToFilled()
    {
        isFilled = true;
    }

   



}
