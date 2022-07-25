using System.Collections.Generic;

[System.Serializable]
public abstract class SoundShapeFunction
{
    public abstract bool ShapeFunctionParametersUpdated();
    public abstract List<float> GenerateSamples(int numberOfSamples, double angularFrequency);
}
