using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DemoDataObject
{
    public int ID;
    public float randomValue1;
    public float randomValue2;
    public float randomValue3;

    public void Init(int id)
    {
        ID = id;
        randomValue1 = Random.Range(-10f, 10f);
        randomValue2 = Random.insideUnitCircle.x;
        randomValue3 = randomValue1 / (2+randomValue2);
    }
}

public class RandomDataCreator : MonoBehaviour
{
    public DashBoardController dashBoard;
    public int numDataObjects = 2500;

    List<DemoDataObject> demoDataObjects;
    List<ScatterSettings> scatterSettings;
    public DataLogger logger;
    void UpdateGraphs()
    {
        List<List<float>> indexes, values;
        (indexes, values) = GenerateData();

        DashFigureController fig1 = dashBoard.GetGraphByName("Demo Scatter");
        ScatterplotWriter[] scatters = fig1.GetComponentsInChildren<ScatterplotWriter>();
        for (int i=0; i<scatters.Length; i++)
        {
            scatters[i].UpdateData(indexes[i], values[i]);
        }
        fig1.DrawGraph();

        logger.Log(values[0][0]);
    }

    private void Update()
    {
        UpdateGraphs();
    }


    List<DemoDataObject> InitObjects()
    {
        demoDataObjects = new List<DemoDataObject>();
        for (int i = 0; i < numDataObjects; i++)
        {
            DemoDataObject demoDataObject = new DemoDataObject();
            demoDataObject.Init(i);
            demoDataObjects.Add(demoDataObject);
        }

        return demoDataObjects;
    }

    private void RandomizeObjects()
    {
        demoDataObjects.Clear();

        for (int i = 0; i < numDataObjects; i++)
        {
            DemoDataObject demoDataObject = new DemoDataObject();
            demoDataObject.Init(i);
            demoDataObjects.Add(demoDataObject);
        }
    }

    (List<List<float>>, List<List<float>>) GenerateData()
    {
        RandomizeObjects();

        List<List<float>> indexes = new List<List<float>>();
        List<List<float>> values = new List<List<float>>();
        for (int i = 0; i < 3; i++)
        {
            indexes.Add(demoDataObjects.Select(o => (float)o.ID).ToList());
        }
        values.Add(demoDataObjects.Select(o => o.randomValue1).ToList());
        values.Add(demoDataObjects.Select(o => o.randomValue2).ToList());
        values.Add(demoDataObjects.Select(o => o.randomValue3).ToList());
        return (indexes, values);
    }

    void DrawSimpleScatter(List<List<float>> indexes, List<List<float>> values)
    {
        DashFigureController newGraph = dashBoard.AddNewGraph("Demo Scatter");
        newGraph.figureSettings.description = "A plot of the random datapoints";
        newGraph.SetLabels("Demonstration Graph", "Index", "Values");
        newGraph.Scatter(indexes, values, scatterSettings);
        newGraph.DrawGraph();
    }

    void DrawCustomScatter(List<List<float>> indexes, List<List<float>> values)
    {
        DashFigureController newGraph2 = dashBoard.AddNewGraph("Demo Scatter 2");
        newGraph2.SetLabels("Demonstration Graph", "Index", "Values");
        newGraph2.Scatter(indexes, values, scatterSettings);
        newGraph2.SetXLim(-50, numDataObjects + 76);
        newGraph2.SetYLim(-25, 11);
        List<float> newXTicks = new List<float>();
        newXTicks.Add(-50);
        newXTicks.Add(0);
        newXTicks.Add(50);
        newXTicks.Add(100);
        List<float> newYTicks = new List<float>();
        newYTicks.Add(-25);
        newYTicks.Add(-15);
        newYTicks.Add(0);
        newYTicks.Add(10);
        newGraph2.SetXTicks(newXTicks);
        newGraph2.SetYTicks(newYTicks);
        newGraph2.DrawGraph();
    }

    void DrawSinglePointScatter()
    {
        DashFigureController newGraph3 = dashBoard.AddNewGraph("Demo Scatter 3");
        newGraph3.Scatter(new List<float>() { 1f }, new List<float>() { 1f });
        newGraph3.DrawGraph();
    }

    private void Start()
    {
        demoDataObjects = InitObjects();

        List<List<float>> indexes, values;
        (indexes, values) = GenerateData();

        scatterSettings = new List<ScatterSettings>();
        scatterSettings.Add(new ScatterSettings(Color.red, 5f, "Random Value 1", pad: 0));
        scatterSettings.Add(new ScatterSettings(Color.blue, 5f, "Random Value 2", pad: 0));
        scatterSettings.Add(new ScatterSettings(Color.green, 5f, "Random Value 3", pad: 0));

        // Add a scatterplot!
        DrawSimpleScatter(indexes, values);

        DrawCustomScatter(indexes, values);

        DrawSinglePointScatter();
        
        DashFigureController flatHist = dashBoard.AddNewGraph("Flat Histogram");
        HistogramSettings flatHistSettings = new HistogramSettings(_labels: new List<string>() { "Data 1", "Data 2", "Data 3" }, _stacked: false, _numBins: 25);
        flatHist.Hist(values, flatHistSettings);
        flatHist.SetLabels("Demo Flat Hist", "Values", "Counts");
        flatHist.DrawGraph();

        DashFigureController stackedHist = dashBoard.AddNewGraph("Stacked Histogram");
        HistogramSettings stackedHistSettings = new HistogramSettings(_labels: new List<string>() { "Data 1", "Data 2", "Data 3" }, _stacked: true, _numBins: 25);
        stackedHist.Hist(values, stackedHistSettings);
        stackedHist.SetLabels("Demo Stacked Hist", "Values", "Stacked Counts");
        stackedHist.DrawGraph();

        List<float> bins = new List<float>() { 0, 2, 4, 6, 8 };
        List<List<float>> heights = new List<List<float>>() { 
            new List<float>(){ 1, 1, 1, 1, 1 }, 
            new List<float>(){ 0, 1, 2, 3, 4 }};
        DashFigureController stackedBar = dashBoard.AddNewGraph("Stacked Bar Chart");
        stackedBar.Bar(bins, heights, new BarSettings(_labels: new List<string>() { "Data 1", "Data 2" }, _stacked: true, _transposeY: false));
        stackedBar.SetLabels("Demo Stacked Hist", "Values", "Stacked Counts");
        stackedBar.DrawGraph();
        DashFigureController sideBar = dashBoard.AddNewGraph("Bar Chart");
        sideBar.Bar(bins, heights, new BarSettings(_labels: new List<string>() { "Data 1", "Data 2" }, _stacked: false, _transposeY: false));
        sideBar.SetLabels("Demo Stacked Hist", "Values", "Stacked Counts");
        sideBar.DrawGraph();


        DashFigureController colorMap = dashBoard.AddNewGraph("Image");
        Color[,] imColors = new Color[15, 18];
        for (int x = 0; x < imColors.GetLength(1); x++)
        {
            for (int y = 0; y < imColors.GetLength(0); y++)
            {
                imColors[y, x] = ColorCycler.GetColor(x + y);
            }
        }
        colorMap.Image(imColors);
        colorMap.SetLabels("Demo Heatmap", "Values", "Stacked Counts");
        colorMap.DrawGraph();


        DashFigureController heatMap = dashBoard.AddNewGraph("Heatmap");
        float[,] imFloats = new float[50, 35];
        for (int x = 0; x < imFloats.GetLength(1); x++)
        {
            for (int y = 0; y < imFloats.GetLength(0); y++)
            {
                imFloats[y, x] = ((x + y) %23) /23f;
            }
        }
        heatMap.Image(imFloats);
        heatMap.SetLabels("Demo Heatmap", "Values", "Stacked Counts");
        heatMap.DrawGraph();

        logger = new DataLogger("First value");
        logger.Log(demoDataObjects[0].randomValue1);
    }
}
