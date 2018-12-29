using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelNextSlot : MonoBehaviour {

    public TetSlot[] slotList = new TetSlot[16];
    public TetBlock tetBlock = null;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateTetBlock(TetBlock block)
    {
        TetBlock tempBlock = new TetBlock(block.shape);
        tempBlock.RotateCounterClockwise();
        if(tetBlock!=null)
        {
            foreach(var pos in tetBlock.position)
            {
                slotList[(int)(5 + pos.x + 4 * (pos.y))].UpdateToBlack();                
            }
        }
        tetBlock = tempBlock;

        foreach (var pos in tetBlock.position)
        {
            slotList[(int)(5 + pos.x + 4 * (pos.y))].UpdateColor(block.color);
        }

    }
}
