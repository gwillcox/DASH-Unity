using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LegendEntryController : MonoBehaviour
{
    public TMPro.TMP_Text label;
    public Image icon;

    public void SetLabelAndIcon(string l, Color c)
    {
        label.text = l;
        icon.color = c;
    }
}
