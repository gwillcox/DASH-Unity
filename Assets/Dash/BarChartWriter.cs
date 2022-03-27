using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class BarChartWriter : BarBaseWriter
{
    public BarChartWriter(DashFigureController _fig) { figure = _fig; }

    public void Init(DashFigureController _fig, BarSettings _settings, List<float> _bins, List<List<float>> _height)
    {
        figure = _fig;
        settings = _settings;
        LoadBarResource();
        UpdateData(_bins, _height);
        while (settings.labels.Count < numDataSets) { settings.labels.Add("unlabeled"); }  // Ensure a full set of labels. 
    }

    public void UpdateData(List<float> _bins, List<List<float>> _counts)
    {
        X = _bins.ToArray<float>();
        numBins = X.Count();
        if (settings.transposeY)
        {
            numDataSets = _counts[0].Count();       // Assumes the incoming height data is in the form: [bins, datasets]
            Y = new float[numBins, numDataSets];    // Y must be shape: [Bins, datasets]
            for (int i = 0; i < numDataSets; i++)
            {
                for (int j = 0; j < numBins; j++)
                {
                    Y[j, i] = _counts[j][i];
                }
            }
        }
        else
        {
            numDataSets = _counts.Count();          // Assumes the incoming height data is in the form: [datasets, bins]
            Y = new float[numBins, numDataSets];    // Y must be shape: [Bins, datasets]
            for (int i =0; i< numDataSets; i++)
            {
                for (int j=0; j< numBins; j++)
                {
                    Y[j, i] = _counts[i][j]; 
                }
            }

        }

        // Then, calculate the relevant variables. 
        (xMin, xMax) = CalculateDataMinMax();
        (yMin, yMax) = GetYLimits();
        binWidth = (xMax - xMin) / X.Count();
        xMax += binWidth;  // show the full last bin. 
    }

    public (float, float) CalculateDataMinMax()
    {

        float xMin = float.PositiveInfinity;
        float xMax = float.NegativeInfinity;
        foreach (float d in X)
        {
            xMax = Mathf.Max(xMax, d);
            xMin = Mathf.Min(xMin, d);
        }
        return (xMin, xMax);
    }


    public override (float, float) GetXLimits()
    {
        (xMin, xMax) = CalculateDataMinMax();
        xMax += binWidth;  // show the full last bin. 
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
                for (int datasetID = 0; datasetID < numDataSets; datasetID++)
                {
                    currentCount += Y[binID, datasetID];
                }
                maxCount = Mathf.Max(currentCount, maxCount);
            }
        }
        else
        {
            foreach (float d in Y)
            {
                maxCount = Mathf.Max(maxCount, d);
            }
        }

        return (0, maxCount);
    }


    public override (List<string>, List<Color>) GetLegendData()
    {
        return (settings.labels, settings.colors);
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
                newBar.transform.localPosition = new Vector3((X[i]-figure.xMin) * figure.graphObject.rectTransform.rect.width / (figure.xMax - figure.xMin), currentHeight, 0);
                newBar.rectTransform.sizeDelta = new Vector2(barWidth, (Y[i, j]-figure.yMin) * figure.graphObject.rectTransform.rect.height / (figure.yMax - figure.yMin));
                newBar.color = settings.GetColor(j);
                bars[i * numDataSets + j] = newBar;
                currentHeight += Y[i, j] * figure.graphObject.rectTransform.rect.height / figure.yMax;
            }
        }
    }

    void DrawUnstackedBars()
    {
        float barWidth = figure.graphObject.rectTransform.rect.width / numBins / numDataSets;
        bars = new Image[numBins * numDataSets];
        for (int i = 0; i < numBins; i++)
        {
            for (int j = 0; j < numDataSets; j++)
            {
                Image newBar = Instantiate<Image>(barImage, figure.graphObject.transform);
                newBar.transform.localPosition = new Vector3((X[i] - figure.xMin) * figure.graphObject.rectTransform.rect.width / (figure.xMax - figure.xMin) + j * barWidth, 0, 0);
                newBar.rectTransform.sizeDelta = new Vector2(barWidth, (Y[i, j]-figure.yMin) * figure.graphObject.rectTransform.rect.height / (figure.yMax - figure.yMin));
                newBar.color = settings.GetColor(j);
                bars[i * numDataSets + j] = newBar;
            }
        }
    }
}
