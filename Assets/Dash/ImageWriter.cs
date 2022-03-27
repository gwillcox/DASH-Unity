using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ImageSettings
{
    public int width;
    public int height;
    public Gradient gradient;

    public ImageSettings(int _width, int _height, Gradient _gradient = null)
    {
        width = _width;
        height = _height;
        gradient = _gradient??CreateDefaultGradient();
    }

    Gradient CreateDefaultGradient()
    {
        gradient = new Gradient();
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = Color.red;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Color.blue;
        colorKey[1].time = 1.0f;
        gradient.colorKeys = colorKey;

        return gradient;
    }
}

public class ImageWriter : GraphWriter
{
    public ImageSettings settings;
    float[,] data;
    Color[,] colors;
    List<Image> pixels = new List<Image>();
    Image pixelImage;

    public ImageWriter(DashFigureController _fig) { figure = _fig; }

    public void Init(DashFigureController _fig, ImageSettings _settings, Color[,] _colors)
    {
        figure = _fig;
        settings = _settings;
        LoadPixelResource();
        CreatePixels();
        UpdateData(_colors);
    }

    public void Init(DashFigureController _fig, ImageSettings _settings, float[,] _values)
    {
        figure = _fig;
        settings = _settings;
        LoadPixelResource();
        CreatePixels();
        data = _values;
        colors = ConvertValuesToColors(_values);
        UpdateData(colors);
    }

    Color[,] ConvertValuesToColors(float[,] values)
    {
        Color[,] colors = new Color[settings.height, settings.width];
        for (int x = 0; x < settings.width; x++)
        {
            for (int y = 0; y < settings.height; y++)
            {
                colors[y, x] = settings.gradient.Evaluate(values[y, x]);
            }
        }
        return colors;
    }

    void CreatePixels()
    {
        pixels = new List<Image>();
        for (int x = 0; x<settings.width; x++)
        {
            for (int y = 0; y < settings.height; y++) {
                Vector2 pos = new Vector2(x * figure.graphObject.rectTransform.rect.width / settings.width, y * figure.graphObject.rectTransform.rect.height / settings.height);
                Image newPixel = Instantiate(pixelImage, figure.graphObject.transform);
                newPixel.rectTransform.localPosition = pos;
                newPixel.rectTransform.localScale = new Vector3(figure.graphObject.rectTransform.rect.width / settings.width, figure.graphObject.rectTransform.rect.height / settings.height, 1f);
                pixels.Add(newPixel);
            }
        }
    }

    void LoadPixelResource()
    {
        pixelImage = Resources.Load<Image>("Pixel");
    }

    void CheckDataDimensions(Color[,] data)
    {
        if (settings.width != data.GetLength(1) || settings.height != data.GetLength(0))
        {
            Debug.LogError($"Data is not of correct format: Width={data.GetLength(1)} != {settings.width} OR height={data.GetLength(0)} != {settings.height}");
        }
    }
    void CheckDataDimensions(float[,] data)
    {
        if (settings.width != data.GetLength(1) || settings.height != data.GetLength(0))
        {
            Debug.LogError($"Data is not of correct format: Width={data.GetLength(1)} != {settings.width} OR height={data.GetLength(0)} != {settings.height}");
        }
    }

    public void UpdateData(Color[,] newPixelData)
    {
        CheckDataDimensions(newPixelData);
        colors = newPixelData;
        Redraw();
    }

    public void UpdateData(float[,] newPixelValues)
    {
        CheckDataDimensions(newPixelValues);
        colors = ConvertValuesToColors(newPixelValues);
        Redraw();
    }

    public override (float, float) GetXLimits()
    {
        return (0, settings.width);
    }

    public override (float, float) GetYLimits()
    {
        return (0, settings.height);
    }


    public override (List<string>, List<Color>) GetLegendData()
    {
        return (null, null);  // TODO: check if we can just return null. 
    }

    public override void Redraw()
    {
        if (data != null) { colors = ConvertValuesToColors(data); }  // Recalculate colors;
        for (int i = 0; i < pixels.Count; i++)
        {
            pixels[i].color = colors[i % settings.height, i / settings.height];
        }
    }
}
