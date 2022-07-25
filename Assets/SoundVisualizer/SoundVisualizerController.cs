using UnityEngine;
//using Assets.SoundScripts;

[RequireComponent(typeof(Material))]
[RequireComponent(typeof(ProceduralSoundGenerator))]
public class SoundVisualizerController : MonoBehaviour
{
    // show lines or dots
    // get 2D buffer from ProceduralSoundGenerator and send it to the material shader property

    private Material material;
    private ProceduralSoundGenerator soundGenerator;

    private ComputeBuffer soundSamplesBuffer;

    private bool started = false;

    private void Start()
    {
        material = GetComponent<Renderer>().material;
        soundGenerator = GetComponent<ProceduralSoundGenerator>();
    }

    void SetupAndStartVisualizer()
    {
        InitializeBuffers();
        material.SetBuffer("soundSamplesBuffer", soundSamplesBuffer);
        material.SetInt("totalSoundSamples", soundGenerator.soundSamplesBufferCurrent.Count); 
    }


    void InitializeBuffers()
    {
        soundSamplesBuffer = new ComputeBuffer(soundGenerator.soundSamplesBufferCurrent.Count, sizeof(float));
        soundSamplesBuffer.SetData(soundGenerator.soundSamplesBufferCurrent);
    }

    void Update()
    {
        //TestSpectrumVisualizer();

        if (!started)
        {
            started = true;
            SetupAndStartVisualizer();
        }

        //if (soundGenerator.changedNumberOfSamplesOrChannels) //  size of buffer change won't happen atm
        //{
        //    DisposeBuffers();
        //    InitializeBuffers();
        //}

        if (soundGenerator.newSampleBufferData)
        {
            soundSamplesBuffer.SetData(soundGenerator.soundSamplesBufferCurrent);
        }
    }


    // https://docs.unity3d.com/ScriptReference/AudioSource.GetSpectrumData.html
    void TestSpectrumVisualizer()
    {
        float[] spectrum = new float[256];

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }

    }

    void DisposeBuffers()
    {
        if (soundSamplesBuffer != null) soundSamplesBuffer.Release();
    }
    private void OnDestroy()
    {
        DisposeBuffers();
    }
}
