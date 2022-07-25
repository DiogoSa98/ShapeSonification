
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SupershapeSound : SoundShapeFunction
{
    private float gielisMaxRadius;

    [Range(0, 100)] public float M;
    [Range(0, 100)] public float A;
    [Range(0, 100)] public float B;
    [Range(0, 100)] public float N1;
    [Range(0, 100)] public float N2;
    [Range(0, 100)] public float N3;

    private float m;
    private float a;
    private float b;
    private float n1;
    private float n2;
    private float n3;

    public override bool ShapeFunctionParametersUpdated()
    {
        return m != M || a != A || b != B || n1 != N1 || n2 != N2 || n3 != N3;
    }

    public override List<float> GenerateSamples(int numberOfSamples, double angularFrequency)
    {
        List<float> samples = new List<float>();
        List<float> gielisAmplitude = new List<float>();
            
        gielisMaxRadius = 0.0f;
            
        float angularStep = 2 * Mathf.PI / numberOfSamples; // ANGULAR FREQUENCY??  angle should be angularFrequency * i???
        for (int i = 0; i < numberOfSamples; i++) 
        {
            float angle = i * angularStep;
            gielisAmplitude.Add(GenerateGielisSample(angle)); 
        }

        // normalize amplitude values and populate samples
        double phase = 0.0f;
        for (int i = 0; i < numberOfSamples; i++)
        {
            gielisAmplitude[i] /= gielisMaxRadius;

            samples.Add(gielisAmplitude[i] * Mathf.Sin((float)phase)); // FIXME hardcoded for 2 channels
            samples.Add(gielisAmplitude[i] * Mathf.Cos((float)phase));
            phase += angularFrequency;
        }

        return samples;
    }

    private float GenerateGielisSample(float angle)
    {
        // FIXME set gielis formula parameters here cause serialization not working
        m = 4.0f;
        a = 1.0f;
        b = 1.0f;
        n1 = 1.0f;
        n2 = 3.0f;
        n3 = 1.4f;

        // do the gielis calculation
        float radius = GielisFormula(angle);

        // update gielisMaxRadius
        if (radius > gielisMaxRadius)
            gielisMaxRadius = radius;

        return radius;
    }

    private float GielisFormula(float angle)
    {
        float part1 = Mathf.Pow(Mathf.Abs(Mathf.Cos((m * angle) / 4.0f) / a), n2);
        float part2 = Mathf.Pow(Mathf.Abs(Mathf.Sin((m * angle) / 4.0f) / b), n3);
        return Mathf.Pow(part1 + part2, -1.0f / n1);
    }
}
