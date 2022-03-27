using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ScatterMarkerType {Circle, Square, X};

public class ScatterSettings
{
    public Color markerColor = Color.black;
    public float markerSize = 10f;
    public string label = "";
    public ScatterMarkerType markerType;
    public float padding = 0.01f;
    public int scatterID;

    public ScatterSettings(Color? c = null, float size = 10f, string _label = "", ScatterMarkerType marker = ScatterMarkerType.Circle, float pad = 0.01f, int _scatterID = 0)
    {
        scatterID = _scatterID;
        markerColor = c ?? ColorCycler.GetColor(scatterID);
        markerSize = size;
        label = _label;
        markerType = marker;
        padding = pad;
    }
}

public class ScatterplotWriter : GraphWriter
{
    public List<float> dataX = new List<float>();
    public List<float> dataY = new List<float>();
    public GameObject scatterMarker;
    public ScatterSettings settings;

    float xMin, xMax, yMin, yMax;
    List<GameObject> markers = new List<GameObject>();
    int numPoints;

    public void AssignMarkerFromSettings()
    {
        if (settings.markerType == ScatterMarkerType.Circle)
        {
            scatterMarker = Resources.Load<GameObject>("MarkerCircle");
        }
        else if (settings.markerType == ScatterMarkerType.X)
        {
            scatterMarker = Resources.Load<GameObject>("MarkerX");
        }
        else if (settings.markerType == ScatterMarkerType.Square)
        {
            scatterMarker = Resources.Load<GameObject>("MarkerSquare");
        }
    }

    public void Init(DashFigureController _fig, ScatterSettings _settings, List<float> x, List<float> y)
    {
        figure = _fig;
        settings = _settings;
        AssignMarkerFromSettings();
        UpdateData(x, y);
    }

    void CheckSizesEqual(int size1, int size2)
    {
        if (size1 != size2) { Debug.LogError($"Data is not of the same size: {size1}!={size2}"); }
    }

    public void UpdateData(List<float> newDataX, List<float> newDataY)
    {
        dataX = newDataX;
        dataY = newDataY;
        numPoints = dataX.Count;
        SetLimits();
    }

    public override (List<string>, List<Color>) GetLegendData()
    {
        return (new List<string>() { settings.label }, new List<Color>() { settings.markerColor });
    }

    void SetLimits()
    {
        xMin = dataX.Min() - Mathf.Epsilon;
        xMax = dataX.Max() + Mathf.Epsilon;
        xMin -= (xMax - xMin) * settings.padding;
        xMax += (xMax - xMin) * settings.padding;
        yMin = dataY.Min();
        yMax = dataY.Max();
        yMin -= (yMax - yMin) * settings.padding;
        yMax += (yMax - yMin) * settings.padding;

        if (yMax == yMin)
        {
            yMax += 0.1f;
            yMin -= 0.1f;
        }
        if (xMax == xMin)
        {
            xMax += 0.1f;
            xMin -= 0.1f;
        }
    }


    public override (float, float) GetXLimits()
    {
        return (xMin, xMax);
    }

    public override (float, float) GetYLimits()
    {
        return (yMin, yMax);
    }

    public override void Clear()
    {
        dataX.Clear();
        dataY.Clear();
        foreach (GameObject marker in markers)
        {
            Destroy(marker);
        }
    }

    public override void Redraw()
    {
        int markerCount = markers.Count;
        float figWidthScale = figure.graphObject.rectTransform.rect.height / (figure.xMax - figure.xMin);
        float figHeightScale = figure.graphObject.rectTransform.rect.height / (figure.yMax - figure.yMin);
        Image markerImage;
        if (markerCount > numPoints) { markers = markers.GetRange(0, numPoints); Debug.Log($"Removed points: {numPoints}, {markers.Count}"); }
        else if (markerCount < numPoints) { for (int i = 0; i < numPoints - markerCount; i++) { markers.Add(Instantiate(scatterMarker, figure.graphObject.transform)); } Debug.Log($"Added points: {numPoints}, {markers.Count}"); }   // Ensure Markers is the right size. 

        for (int i = 0; i < numPoints; i++)
        {
            if (float.IsNaN(dataX[i]) || float.IsNaN(dataY[i]) ) { Debug.Log("Skipped"); continue; } // Skip drawing if the data is NaN

            Vector3 position = new Vector3(
                figWidthScale * (dataX[i] - figure.xMin),
                figHeightScale * (dataY[i] - figure.yMin),
                0f
                );

            markers[i].transform.localPosition = position;
            markerImage = markers[i].GetComponent<Image>();
            markerImage.rectTransform.sizeDelta = new Vector2(settings.markerSize, settings.markerSize);
            markerImage.color = settings.markerColor;
        }
    }
}
