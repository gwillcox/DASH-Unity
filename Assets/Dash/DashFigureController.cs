using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class ColorCycler
{
    static readonly List<Color> colorArray = new List<Color>() { new Color(229/255f, 157/255f, 36/255f, 1), new Color(130 / 255f, 78 / 255f, 160 / 255f, 1), new Color(19 / 255f, 139 / 255f, 66 / 255f, 1), Color.red, Color.magenta, Color.cyan, Color.yellow, Color.blue, Color.black, Color.gray };
    public static Color GetColor(int i = 0)
    {
        return colorArray[i % colorArray.Count];
    }
}

public class DashFigureController : MonoBehaviour
{
    public List<GraphWriter> writers = new List<GraphWriter>();
    public float xMin, xMax, yMin, yMax;
    public DashFigureSettings figureSettings = new DashFigureSettings();

    [Header("Objects")]
    public TMPro.TMP_Text titleObject;
    public TMPro.TMP_Text xLabelObject;
    public TMPro.TMP_Text yLabelObject;
    public Image graphObject;
    public LegendController legend;

    TMPro.TMP_Text xTickPrefab;
    TMPro.TMP_Text yTickPrefab;
    Image gridLinePrefab;

    List<TMPro.TMP_Text> xTicks = new List<TMPro.TMP_Text>();
    List<TMPro.TMP_Text> yTicks = new List<TMPro.TMP_Text>();

    NiceScale xScale;
    NiceScale yScale;

    List<Image> xGridLines = new List<Image>();
    List<Image> yGridLines = new List<Image>();

    private void Start()
    {
        legend = GetComponentInChildren<LegendController>();

        xTickPrefab = Resources.Load<TMPro.TMP_Text>("XAxisTick");
        yTickPrefab = Resources.Load<TMPro.TMP_Text>("YAxisTick");
        gridLinePrefab = Resources.Load<Image>("GridLine");
    }

    void LoadXTickPrefab() { xTickPrefab = Resources.Load<TMPro.TMP_Text>("XAxisTick"); }
    void LoadYTickPrefab() { yTickPrefab = Resources.Load<TMPro.TMP_Text>("YAxisTick"); }
    void LoadGridLinePrefab() { gridLinePrefab = Resources.Load<Image>("GridLine"); }

    public void DrawGraph()
    {
        MaybeSetXLims();
        MaybeSetYLims();
        DrawGrid();
        foreach (GraphWriter w in writers)
        {
            w.Redraw();
        }
        DrawLabels();
        MaybeDrawLegend();
    }

    void MaybeSetXLims()
    {
        float writerXMax, writerXMin;

        if (!figureSettings.userSetXLims)
        {
            ResetXLimits();

            foreach (GraphWriter w in writers)
            {
                (writerXMin, writerXMax) = w.GetXLimits();  // Allow each writer to extend the scope of the graph
                xMin = Mathf.Min(writerXMin, xMin);
                xMax = Mathf.Max(writerXMax, xMax);
            }
        }
    }

    void MaybeSetYLims()
    {
        float writerYMax, writerYMin;
        if (!figureSettings.userSetYLims)
        {
            ResetYLimits();

            foreach (GraphWriter w in writers)
            {
                (writerYMin, writerYMax) = w.GetYLimits();  // Allow each writer to extend the scope of the graph
                yMin = Mathf.Min(writerYMin, yMin);
                yMax = Mathf.Max(writerYMax, yMax);
            }
        }
    }

    void SetNewXTicks()
    {
        xScale = new NiceScale(xMin, xMax);
        if (!figureSettings.userSetXLims)
        {
            figureSettings.xTicks = xScale.GetTicks();
            xMin = xScale.GetNiceMin();
            xMax = xScale.GetNiceMax();
        }
        else
        {
            figureSettings.xTicks = xScale.GetTicks().Where(o => (o >= xMin && o <= xMax)).ToList();
        }
        figureSettings.xTickLabels = figureSettings.xTicks.Select(o => o.ToString()).ToList();
    }

    void SetNewYTicks() {
        yScale = new NiceScale(yMin, yMax);
        if (!figureSettings.userSetYLims)
        {
            figureSettings.yTicks = yScale.GetTicks();
            yMin = yScale.GetNiceMin();
            yMax = yScale.GetNiceMax();
        }
        else
        {
            figureSettings.yTicks = yScale.GetTicks().Where(o => (o >= yMin && o <= yMax)).ToList();
        }
        figureSettings.yTickLabels = figureSettings.yTicks.Select(o => o.ToString()).ToList();
    }

    public void ResetXLim()
    {
        figureSettings.userSetXLims = false;
    }

    public void SetXLim(float min, float max)
    {
        figureSettings.userSetXLims = true;
        xMin = min;
        xMax = max;
    }
    public void ResetYLim()
    {
        figureSettings.userSetYLims = false;
    }
    public void SetYLim(float min, float max)
    {
        figureSettings.userSetYLims = true;
        yMin = min;
        yMax = max;
    }

    public void SetXTicks(List<float> ticks, List<string> labels = null)
    {
        figureSettings.userSetXTicks = true;
        figureSettings.xTicks = ticks;
        figureSettings.xTickLabels = labels ?? ticks.Select(o => o.ToString()).ToList();
    }

    public void SetYTicks(List<float> ticks, List<string> labels = null)
    {
        figureSettings.userSetYTicks = true;
        figureSettings.yTicks = ticks;
        figureSettings.yTickLabels = labels ?? ticks.Select(o => o.ToString()).ToList();
    }

    void ManageTicksAndGridLines(List<TMPro.TMP_Text> ticks, List<Image> lines, int numDesired, TMPro.TMP_Text tickPrefab, Image linePrefab, Transform parent, bool showLines  )
    {
        for (int i = ticks.Count; i < numDesired; i++) // Add more ticks if necessary. 
        {
            ticks.Add(Instantiate(tickPrefab, parent));
        }
        for (int i = lines.Count; i < numDesired; i++) // Add more ticks if necessary. 
        {
            lines.Add(Instantiate(linePrefab, parent));
        }

        for (int i = 0; i < ticks.Count; i++)  // Turn off ticks that we don't need. 
        {
            if (i > numDesired)
            {
                ticks[i].enabled = false;
                lines[i].enabled = false;
            }
            else
            {
                ticks[i].enabled = true;
                lines[i].enabled = showLines;
            }
        }
    }

    void DrawXTicks() 
    {
        if (xTickPrefab==null) { LoadXTickPrefab(); }
        if (gridLinePrefab==null) { LoadGridLinePrefab(); }
        ManageTicksAndGridLines(xTicks, xGridLines, figureSettings.xTicks.Count, xTickPrefab, gridLinePrefab, graphObject.transform, figureSettings.showGridX);

        for (int i = 0; i < figureSettings.xTicks.Count; i++)
        {
            Vector3 position = new Vector3(graphObject.rectTransform.rect.width * (figureSettings.xTicks[i] - xMin) / (xMax - xMin), 0, 0);
            xTicks[i].transform.localPosition = position;
            xTicks[i].text = figureSettings.xTickLabels[i]; 

            if (figureSettings.showGridX)
            {
                xGridLines[i].rectTransform.localPosition = position;
            }
        }
    }

    void DrawYTicks()
    {
        if (yTickPrefab == null) { LoadYTickPrefab(); }
        if (gridLinePrefab == null) { LoadGridLinePrefab(); }
        ManageTicksAndGridLines(yTicks, yGridLines, figureSettings.yTicks.Count, yTickPrefab, gridLinePrefab, graphObject.transform, figureSettings.showGridY);

        for (int i = 0; i < figureSettings.yTicks.Count; i++)
        {
            Vector3 position = new Vector3(0, graphObject.rectTransform.rect.height * (figureSettings.yTicks[i] - yMin) / (yMax - yMin), 0);
           
            yTicks[i].transform.localPosition = position;
            yTicks[i].text = figureSettings.yTickLabels[i];

            if (figureSettings.showGridY)
            {
                yGridLines[i].rectTransform.localPosition = position;
                yGridLines[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            }
        }
    }

    void DrawGrid()
    {
        if (!figureSettings.userSetXTicks) { SetNewXTicks(); }
        DrawXTicks();

        if (!figureSettings.userSetYTicks) { SetNewYTicks(); }
        DrawYTicks();
    }

    void MaybeDrawLegend()
    {
        if (figureSettings.showLegend)
        {
            DrawLegend();
        }
        else { legend.gameObject.SetActive(false); }
    }

    void DrawLegend()
    {
        legend.gameObject.SetActive(true);

        legend.UpdateLegend(writers);
    }

    void CheckEqual(int size1, int size2)
    {
        if (size1 != size2) { Debug.LogError($"Data is not of the same size: {size1}!={size2}"); }
    }

    public void Scatter(List<List<float>> x, List<List<float>> y, List<ScatterSettings> scatterSettings=null)
    {
        CheckEqual(x.Count, y.Count);

        if (scatterSettings == null)   // Ensure that we have some settings. 
        {
            scatterSettings = new List<ScatterSettings>();
            for (int i = 0; i < x.Count; i++) { scatterSettings.Add(new ScatterSettings(_scatterID:i)); }
        }
        
        for (int i=0; i<x.Count; i++)
        {
            Scatter(x[i], y[i], scatterSettings[i]);
        }
    }

    public void Scatter(List<float> x, List<float> y, ScatterSettings settings=null, int scatterID=0)
    {
        CheckEqual(x.Count, y.Count);
        settings = settings ?? new ScatterSettings();                                   // Ensure we have some settings 

        ScatterplotWriter scatterplot = gameObject.AddComponent<ScatterplotWriter>(); 
        scatterplot.Init(this, settings, x, y);                                         // Create the scatterplotWriter
        writers.Add(scatterplot);                                                       // Track this scatterplotWriter
    }

    public void Hist(List<float> x, HistogramSettings settings = null)                  // Allow histograms to be created with a single List<float> with this wrapper. 
    {
        List<List<float>> newX = new List<List<float>>();
        newX.Add(x);
        Hist(newX, settings);
    }

    public void Hist(List<List<float>> x, HistogramSettings settings = null)
    {
        settings = settings ?? new HistogramSettings();                                 // Ensure we have some settings 

        HistogramWriter hist = gameObject.AddComponent<HistogramWriter>();
        hist.Init(this, settings, x);                                                   // Create the writer
        writers.Add(hist);                                                              // Track this writer
    }

    public void Bar(List<float> bins, List<float> heights, BarSettings settings = null) // Allow histograms to be created with a single List<float> with this wrapper. 
    {
        CheckEqual(bins.Count, heights.Count);
        List<List<float>> newHeights = new List<List<float>>();
        newHeights.Add(heights);
        Bar(bins, newHeights, settings);
    }

    public void Bar(List<float> bins, List<List<float>> heights, BarSettings settings = null)
    {
        settings = settings ?? new BarSettings();                                                   // Ensure we have some settings 

        BarChartWriter bar = gameObject.AddComponent<BarChartWriter>();
        bar.Init(this, settings, bins, heights);                                                    // Create the writer
        writers.Add(bar);                                                                           // Track this writer
    }

    public void Image(Color[,] colors, ImageSettings settings = null)
    {
        settings = settings ?? new ImageSettings(colors.GetLength(1), colors.GetLength(0));         // Ensure we have some settings 

        ImageWriter image = gameObject.AddComponent<ImageWriter>();
        image.Init(this, settings, colors);                                                         // Create the writer
        writers.Add(image);                                                                         // Track this writer
    }


    public void Image(float[,] values, ImageSettings settings = null)
    {
        settings = settings ?? new ImageSettings(values.GetLength(1), values.GetLength(0));         // Ensure we have some settings 

        ImageWriter image = gameObject.AddComponent<ImageWriter>();
        image.Init(this, settings, values);                                                         // Create the writer
        writers.Add(image);                                                                         // Track this writer
    }

    public void Clear()
    {
        ClearWriters();
        ClearVisibleGraph();
    }

    void ClearWriters()
    {
        foreach (GraphWriter w in writers)
        {
            w.Clear();
            GameObject.Destroy(w);
        }
        writers.Clear();
    }

    void ClearVisibleGraph()
    {
    }

    public void SetLabels(string _title = null, string _xLabel = null, string _yLabel = null)
    {
        if (_title != null) { figureSettings.title = _title; }
        if (_title != null) { figureSettings.xLabel = _xLabel; }
        if (_title != null) { figureSettings.yLabel = _yLabel; }
    }

    public void MaybeSetLimits(float _xMin, float _xMax, float _yMin, float _yMax)
    {
        if (_xMin < xMin) { xMin = _xMin; }
        if (_xMax > xMax) { xMax = _xMax; }
        if (_yMin < yMin) { yMin = _yMin; }
        if (_yMax > yMax) { yMax = _yMax; }
    }

    public void ResetXLimits()
    {
        xMin = Mathf.Infinity;
        xMax = -Mathf.Infinity;
    }

    public void ResetYLimits()
    {
        yMin = Mathf.Infinity;
        yMax = -Mathf.Infinity;
    }

    void DrawLabels()
    {
        titleObject.text = figureSettings.title;
        xLabelObject.text = figureSettings.xLabel;
        yLabelObject.text = figureSettings.yLabel;
    }

}




[CustomEditor(typeof(DashFigureController))]
public class DashFigureInspector : Editor
{
    DashFigureController figure;

    void OnEnable()
    {
        figure = (DashFigureController)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Redraw")) { figure.DrawGraph(); }
    }
}
