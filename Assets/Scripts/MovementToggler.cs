using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementToggler : MonoBehaviour
{
    public GameObject player;
    public void UseBase() {
        Behaviour script = player.GetComponent<ImprovedMovement>();
        if (script != null) script.enabled = false;
        script = player.GetComponent<CelesteMovement>();
        if (script != null) script.enabled = false;
        script = player.GetComponent<BaseMovement>();
        if (script != null) script.enabled = true;
    }
    public void UseImproved() {
        Behaviour script = player.GetComponent<BaseMovement>();
        if (script != null) script.enabled = false;
        script = player.GetComponent<CelesteMovement>();
        if (script != null) script.enabled = false;
        script = player.GetComponent<ImprovedMovement>();
        if (script != null) script.enabled = true;
    }
    public void UseGroupImproved(){
        Behaviour script = player.GetComponent<BaseMovement>();
        if (script != null) script.enabled = false;
        script = player.GetComponent<ImprovedMovement>();
        if (script != null) script.enabled = false;
        script = player.GetComponent<CelesteMovement>();
        if (script != null) script.enabled = true;
    }
}
