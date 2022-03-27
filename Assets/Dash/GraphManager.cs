using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    [SerializeField] string title;
    [SerializeField] string xLabel;
    [SerializeField] string yLabel;
    public float userXMin = Mathf.Infinity, userXMax = -Mathf.Infinity, userYMin = Mathf.Infinity, userYMax = -Mathf.Infinity;
    public List<GraphWriter> writers;
    [HideInInspector] public float xMin, xMax, yMin, yMax;


    [Header("Objects")]
    public TMPro.TMP_Text titleObject;
    public TMPro.TMP_Text xLabelObject;
    public TMPro.TMP_Text yLabelObject;
    public TMPro.TMP_Text xMinText;
    public TMPro.TMP_Text xMaxText;
    public TMPro.TMP_Text yMinText;
    public TMPro.TMP_Text yMaxText;
    public Image graphObject;

    public void SetLabels(string _title, string _xLabel, string _yLabel)
    {
        title = _title;
        xLabel = _xLabel;
        yLabel = _yLabel;
    }

    void DrawLabels()
    {
        titleObject.text = title;
        xLabelObject.text = xLabel;
        yLabelObject.text = yLabel;

        xMinText.text = xMin.ToString("E2");
        xMaxText.text = xMax.ToString("E2");
        yMinText.text = yMin.ToString("E2");
        yMaxText.text = yMax.ToString("E2");
    }

    public void RedrawGraph()
    {
        ResetLimits();
        foreach (GraphWriter w in writers)
        {
            /*w.MaybeSetAxesLimits();*/
        }
        foreach (GraphWriter w in writers)
        {
            w.Redraw();
        }
        DrawLabels();
    }

    public void ResetLimits()
    {
        xMin = Mathf.Infinity;
        xMax = -Mathf.Infinity;
        yMin = Mathf.Infinity; 
        yMax = -Mathf.Infinity;
        MaybeSetLimits(userXMin, userXMax, userYMin, userYMax);
    }

    public void MaybeSetLimits(float _xMin, float _xMax, float _yMin, float _yMax)
    {
        if (_xMin < xMin) { xMin = _xMin; }
        if (_xMax > xMax) { xMax = _xMax; }
        if (_yMin < yMin) { yMin = _yMin; }
        if (_yMax > yMax) { yMax = _yMax; }
    }

    private void Start()
    {
        writers = GetComponentsInChildren<GraphWriter>().ToList() ;
    }
}
