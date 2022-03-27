using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BarSettings
{
    public List<Color> colors;
    public List<string> labels;
    public bool stacked = false;
    public bool transposeY = false;

    public BarSettings(List<string> _labels = null, List<Color> c = null, bool _stacked = false, bool _transposeY = false)
    {
        colors = c;
        labels = _labels;
        stacked = _stacked;
        transposeY = _transposeY;
    }
    public BarSettings(string _label, Color? c = null, bool _stacked = false, bool _transposeY = false)
    {
        colors = new List<Color>(1) { c ?? Color.black };
        labels = new List<string>(1) { _label ?? "unlabeled" };
        stacked = _stacked;
        transposeY = _transposeY;   
    }

    public Color GetColor(int datasetID)
    {
        if (colors == null) { colors = new List<Color>(); }
        while (datasetID >= colors.Count) { colors.Add(ColorCycler.GetColor(datasetID)); }
        return colors[datasetID];
    }
}

public class BarBaseWriter : GraphWriter
{
    protected List<List<float>> data;
    protected float[] X;
    protected float[,] Y;               // this is the number of elements in each bin, with shape: [numBins,numDataSets]
    
    public int numDataSets;
    public int numBins;
    protected float binWidth;
    protected float xMin, xMax, yMin, yMax;
    protected Image[] bars = new Image[0];
    protected Image barImage;
    protected BarSettings settings = new BarSettings(_label: "unlabeled");

    public void LoadBarResource()
    {
        barImage = Resources.Load<Image>("Bar");
        bars = new Image[0];
    }

    public override (List<string>, List<Color>) GetLegendData()
    {
        return (settings.labels, settings.colors);
    }

    public override void Clear()
    {
        base.Clear();
        if (data != null) { data.Clear(); }

        foreach (Image i in bars)
        {
            Destroy(i.gameObject);
        }
        bars = new Image[0];
    }

}

