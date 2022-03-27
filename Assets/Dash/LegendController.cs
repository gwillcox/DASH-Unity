using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegendController : MonoBehaviour
{
    public List<LegendEntryController> legendEntries;
    List<string> allLabels = new List<string>();
    List<Color> allColors = new List<Color>();

    public void AddEntry(string label, Color c)
    {
        LegendEntryController newEntry = Instantiate(Resources.Load<GameObject>("LegendEntry"), transform).GetComponent<LegendEntryController>();
        newEntry.SetLabelAndIcon(label, c);
        legendEntries.Add(newEntry);
    }

    public void UpdateLegend(List<GraphWriter> writers)
    {
        allLabels.Clear();
        allColors.Clear();
        for (int i = 0; i < writers.Count; i++)
        {
            (List<string> labels, List<Color> colors) = writers[i].GetLegendData();
            if (labels == null || colors == null) { continue; }
            for (int j=0; j<labels.Count; j++)
            {
                allLabels.Add(labels[j]);
                allColors.Add(colors[j]);
            }
        }
        int numEntries = allLabels.Count;

        for (int i = legendEntries.Count; i < numEntries; i++) { legendEntries.Add(Instantiate(Resources.Load<GameObject>("LegendEntry"), transform).GetComponent<LegendEntryController>()); }  // Make sure we don't create and destroy elements unneccessarily.
        if (legendEntries.Count > numEntries) { legendEntries = legendEntries.GetRange(0, numEntries); }

        for (int i = 0; i < numEntries; i++)
        {
            legendEntries[i].SetLabelAndIcon(allLabels[i], allColors[i]);
        }
    }

    public void Clear()
    {
        foreach (LegendEntryController entry in legendEntries)
        {
            Destroy(entry.gameObject);
        }
        legendEntries.Clear();
    }
}
