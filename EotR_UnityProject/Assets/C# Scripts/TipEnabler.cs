using UnityEngine;
using System.Collections;

public class TipEnabler : MonoBehaviour {
    public GameObject UIText;
    public string tipContent;

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Object entered the trigger.");
        UIText.SendMessage("DisplayTip", tipContent);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        UIText.SendMessage("DisableTip");
    }
}
