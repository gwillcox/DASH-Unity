using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LegendPosition { TopRight, Top, TopLeft, BottomRight, Bottom, BottomLeft}

[System.Serializable]
public class DashFigureSettings
{
    public string title = "";
    public string xLabel = "";
    public string yLabel = "";
    public bool showLegend = true;
    public LegendPosition legendPosition = LegendPosition.TopLeft;
    public bool showGridX = true;
    public bool showGridY = true;
    public List<float> xTicks = new List<float>();
    public List<string> xTickLabels = new List<string>();
    public List<float> yTicks = new List<float>();
    public List<string> yTickLabels = new List<string>();
    public bool userSetXTicks = false;
    public bool userSetXLims = false;
    public bool userSetYTicks = false;
    public bool userSetYLims = false;
    public string description = "no description";
}
