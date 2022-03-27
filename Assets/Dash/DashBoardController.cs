using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DashBoardController : MonoBehaviour
{
    public GameObject graphContainer;
    List<DashFigureController> figures = new List<DashFigureController>();


    // Start is called before the first frame update
    void Start()
    {
        foreach (DashFigureController f in GetComponentsInChildren<DashFigureController>())
        {
            figures.Add(f);
        }
    }

    public DashFigureController AddNewGraph(string graphName = "dashFigure")  // TODO: change figure size? 
    {
        GameObject newDashFigure = Instantiate((GameObject)Resources.Load("DashFigure"), Vector3.zero, Quaternion.identity, graphContainer.transform);
        newDashFigure.name = graphName;
        DashFigureController figureController = newDashFigure.GetComponent<DashFigureController>();
        figures.Add(figureController);
        return figureController;
    }

    public DashFigureController GetGraphByName(string graphName)
    {
        DashFigureController fig = null;
        try
        {
            fig = figures.Where(o => o.name == graphName).First();   // Error
        }
        catch (InvalidOperationException e)
        {
            Debug.LogError($"Figure Not Found: {graphName}, list of figure names: {figures.ConvertAll<string>(o => o.name)}");
        }
        return figures.Where(o => o.name == graphName).First();
    }

    public void FilterGraphsByString(string s)
    {
        s = s.ToLower();
        // Find all graphs which contain a case-insensitive string S in their (i) name, (ii) title, or (iii) description.  
        foreach (DashFigureController fig in figures)
        {
            if (fig.name.ToLower().Contains(s) || fig.figureSettings.title.ToLower().Contains(s) || fig.figureSettings.description.ToLower().Contains(s))
            {
                fig.gameObject.SetActive(true);
            }
            else
            {
                fig.gameObject.SetActive(false);
            }
        }
    }
    public void SaveData()
    {
        Debug.LogError("Data Saving Not Implemented! DashFigureController is a monobehavior. ");
        string json = JsonUtility.ToJson(figures);
        File.WriteAllText("Assets/Data/DashBoardSaveState_" + DateTime.Now.ToFileTimeUtc().ToString() + ".json", json);
    }

    public void LoadData()
    {
        Debug.LogError("Data Loading Not Implemented! First fix saving. ");
    }
}
