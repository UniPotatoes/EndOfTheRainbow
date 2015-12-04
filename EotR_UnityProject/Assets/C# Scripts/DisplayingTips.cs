using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayingTips : MonoBehaviour {

    [SerializeField]private bool showTip = true;
    public Text tip;

    void DisplayTip(string tipText)
    {
        showTip = true;
        tip.text = tipText;
        tip.enabled = true;

    }
    void DisableTip()
    {
        showTip = false;
        tip.text = "disabled";
        tip.enabled = false;
    }
}
