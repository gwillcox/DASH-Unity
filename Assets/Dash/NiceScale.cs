using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NiceScale 
{ 
    private float minPoint;
    private float maxPoint;
    private float maxTicks = 10;
    private float tickSpacing;
    private float range;
    private float niceMin;
    private float niceMax;

    /**
    * Instantiates a new instance of the NiceScale class.
    *
    * @ param min the minimum data point on the axis
    * @param max the maximum data point on the axis
    */
    public NiceScale(float min, float max)
    {
        this.minPoint = min;
        this.maxPoint = max;
        Calculate();
    }

    /**
    * Calculate and update values ​​for tick spacing and nice
    * minimum and maximum data points on the axis.
    */
    private void Calculate()
    {
        this.range = NiceNum(maxPoint - minPoint, false);
        this.tickSpacing = NiceNum(range / (maxTicks - 1), true);
        this.niceMin = Mathf.Floor(minPoint / tickSpacing) * tickSpacing;
        this.niceMax = Mathf.Ceil(maxPoint / tickSpacing) * tickSpacing;
    }

    /**
    * Returns a "nice" number approximately equal to range Rounds
    * the number if round = true Takes the ceiling if round = false.
    *
    * @param range the data range
    * @param round whether to round the result
    * @return a "nice" number to be used for the data range
    */

    private float NiceNum(float range, bool round)
    {
        float exponent; /** exponent of range */
        float fraction; /** fractional part of range */
        float niceFraction; /** nice, rounded fraction */

        exponent = Mathf.Floor(Mathf.Log10(range));
        fraction = range / Mathf.Pow(10, exponent);

        if (round)
        {
            if (fraction < 1.5)
                niceFraction = 1;
            else if (fraction < 3)
                niceFraction = 2;
            else if (fraction < 7)
                niceFraction = 5;
            else
                niceFraction = 10;
        }
        else
        {
            if (fraction <= 1)
                niceFraction = 1;
            else if (fraction <= 2)
                niceFraction = 2;
            else if (fraction <= 5)
                niceFraction = 5;
            else
                niceFraction = 10;
        }

        return niceFraction * Mathf.Pow(10, exponent);
    }

    /**
    * Sets the minimum and maximum data points for the axis.
    *
    * @param minPoint the minimum data point on the axis
    * @param maxPoint the maximum data point on the axis
    */
    public void SetMinMaxPoints(float minPoint, float maxPoint)
    {
        this.minPoint = minPoint;
        this.maxPoint = maxPoint;
        Calculate();
    }

    /**
    * Sets maximum number of tick marks we're comfortable with
    *
    * @param maxTicks the maximum number of tick marks for the axis
    */
    public void setMaxTicks(float maxTicks)
    {
        this.maxTicks = maxTicks;
        Calculate();
    }

    /**
    * Gets the tick spacing.
    *
    * @return the tick spacing
    */
    public float GetTickSpacing()
    {
        return tickSpacing;
    }

    /**
    * Gets the "nice" minimum data point.
    *
    * @return the new minimum data point for the axis scale
    */
    public float GetNiceMin()
    {
        return niceMin;
    }

    /* *
    * Gets the "nice" maximum data point.
    *
    * @return the new maximum data point for the axis scale
    */
    public float GetNiceMax()
    {
        return niceMax;
    }

    public List<float> GetTicks()
    {
        List<float> ticks = new List<float>();
        float tickVal = niceMin;
        while (tickVal <= niceMax)
        {
            ticks.Add(tickVal);
            tickVal += tickSpacing;
        } 
        return ticks;
    }
}
