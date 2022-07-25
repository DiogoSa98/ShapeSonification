
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SinewaveSound : SoundShapeFunction
{
    public override bool ShapeFunctionParametersUpdated()
    {
        return false;
    }

    public override List<float> GenerateSamples(int numberOfSamples, double angularFrequency)
    {
        List<float> samples = new List<float>();
            
        for (int i = 0; i < numberOfSamples; i++)
        {
            samples.Add(Mathf.Sin((float)angularFrequency * i)); // FIXME hardcoded 2 channels
            samples.Add(Mathf.Cos((float)angularFrequency * i));
        }

        return samples;
    }
}
