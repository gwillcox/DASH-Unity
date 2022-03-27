using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HistogramSettings : BarSettings
{
    public int numBins;

    public HistogramSettings(List<string> _labels = null, List<Color> c = null,  bool _stacked = false, int _numBins = 10)
    {
        colors = c;
        labels = _labels;
        stacked = _stacked;
        numBins = _numBins;
    }
    public HistogramSettings(string _label, Color? c = null, bool _stacked = false, int _numBins = 10)
    {
        colors = new List<Color>(1) { c ?? Color.black };
        labels = new List<string>(1) { _label ?? "unlabeled" };
        stacked = _stacked;
        numBins = _numBins;
    }
}

public class HistogramWriter : BarBaseWriter
{
    public new HistogramSettings settings;

    public HistogramWriter(DashFigureController _fig) { figure = _fig; }

    public void Init(DashFigureController _fig, HistogramSettings _settings, List<List<float>> x)
    {
        figure = _fig;
        settings = _settings;
        LoadBarResource();
        UpdateData(x);
    }

    public void UpdateData(List<List<float>> newData)
    {
        data = newData;
        numDataSets = data.Count;
        SetBins();
        CalculateCounts();
    }

    public override (float, float) GetXLimits()
    {
        float xMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity;
        foreach (List<float> d in data)
        {
            xMax = Mathf.Max(xMax, d.Max());
            xMin = Mathf.Min(xMin, d.Min());
        }

        return (xMin, xMax);
    }

    public override (float, float) GetYLimits()
    {
        float maxCount = 0;
        if (settings.stacked)
        {
            for (int binID = 0; binID < numBins; binID++)
            {
                float currentCount = 0;
                for (int datasetID =0; datasetID < numDataSets; datasetID++)
                {
                    currentCount += Y[binID, datasetID];
                }
                maxCount = Mathf.Max(currentCount, maxCount);
            }
        }
        else
        {
            foreach (float c in Y)
            {
                maxCount = Mathf.Max(maxCount, c);
            }
        }

        return (0, maxCount);
    }


    public override (List<string>, List<Color>) GetLegendData()
    {
        return (settings.labels, settings.colors);
    }

    void SetBins()
    {
        (xMin, xMax) = GetXLimits();
        
        numBins = settings.numBins;
        binWidth = (xMax - xMin) / numBins;

        X = new float[numBins + 1];

        for (int i = 0; i <= numBins; i++)
        {
            X[i] = (xMin + i * (binWidth));
        }
    }

    void CalculateCounts()
    {
        Y = new float[numBins,numDataSets];

        for (int i =0; i<numDataSets; i++)
        {
            foreach (float d in data[i])
            {
                bool found = false;
                for (int j = 0; j < numBins; j++)
                {
                    if (d >= X[j] && d <= X[j + 1])  // Add if the number falls in the bin range
                    {
                        Y[j, i] += 1;
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Y[numBins - 1, i] += 1;
                }
            }
        }

        (yMin, yMax) = GetYLimits();
    }

    public override void Redraw()
    {
        foreach (Image i in bars)
        {
            Destroy(i);
        }

        if (settings.stacked) { DrawStackedBars(); }
        else { DrawUnstackedBars(); }
    }

    void DrawStackedBars()
    {
        float barWidth = figure.graphObject.rectTransform.rect.width / numBins;
        bars = new Image[numBins * numDataSets];
        for (int i = 0; i < numBins; i++)
        {
            float currentHeight = 0f;
            for (int j = 0; j < numDataSets; j++)
            {
                Image newBar = Instantiate<Image>(barImage, figure.graphObject.transform);
                newBar.transform.localPosition = new Vector3(i * barWidth, currentHeight, 0);
                newBar.rectTransform.sizeDelta = new Vector2(barWidth, Y[i, j] * figure.graphObject.rectTransform.rect.height / yMax);
                newBar.color = settings.GetColor(j);
                bars[i*numDataSets + j] = newBar;
                currentHeight += Y[i, j] * figure.graphObject.rectTransform.rect.height / yMax;
            }
        }
    }

    void DrawUnstackedBars()
    {
        float barWidth = figure.graphObject.rectTransform.rect.width / numBins / numDataSets;
        bars = new Image[numBins*numDataSets];
        for (int i = 0; i < numBins; i++)
        {
            for (int j = 0; j < numDataSets; j++)
            {
                Image newBar = Instantiate<Image>(barImage, figure.graphObject.transform);
                newBar.transform.localPosition = new Vector3((i*numDataSets + j) * barWidth, 0, 0);
                newBar.rectTransform.sizeDelta = new Vector2(barWidth, Y[i, j] * figure.graphObject.rectTransform.rect.height / yMax);
                newBar.color = settings.GetColor(j);
                bars[i*numDataSets+j] = newBar;
            }
        }
    }

}
