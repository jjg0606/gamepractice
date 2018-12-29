using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMaking : MonoBehaviour {

    public GameObject slotPrefab;
    public SlotController slotController;
    public PanelNextSlot panelNextSlot;

	// Use this for initialization
	void Start () {
        slotController = GetComponent<SlotController>();
        panelNextSlot = GetComponent<PanelNextSlot>();

        if(slotController!=null)
        { 
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    GameObject slot = Instantiate(slotPrefab, this.transform);
                    slot.name = "Slot(" + j + "," + i + ")";
                    TetSlot tetSlot = slot.GetComponent<TetSlot>();
                    slotController.slotList[10 * i + j] = tetSlot;
                }
            }
        }

        if(panelNextSlot!=null)
        { 
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject slot = Instantiate(slotPrefab, this.transform);
                    slot.name = "Slot(" + j + "," + i + ")";
                    TetSlot tetSlot = slot.GetComponent<TetSlot>();
                    panelNextSlot.slotList[4 * i + j] = tetSlot;


                }
            }
        }

    }

}
