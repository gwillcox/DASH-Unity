using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphWriter : MonoBehaviour
{
    [HideInInspector]
    public DashFigureController figure;

    // Start is called before the first frame update
    public virtual void Init(DashFigureController _fig) { }

    public virtual (List<string>, List<Color>) GetLegendData() { return (new List<string>() { "" }, new List<Color>() { Color.black }); }
    public virtual void Redraw() { }
    public virtual void Clear() { }
    public virtual (float, float) GetXLimits() { return (0f, 0f); }
    public virtual (float, float) GetYLimits() { return (0f, 0f); }
    public void SetLabels(string title, string xLabel, string yLabel) { figure.SetLabels(title, xLabel, yLabel); }
}

