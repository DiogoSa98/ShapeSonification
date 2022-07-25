using System;
using System.Collections.Generic;
using UnityEngine;

// TODO, add play/pause pitch/gain?? other stuff??

[RequireComponent(typeof(AudioSource))]
public class ProceduralSoundGenerator : MonoBehaviour
{
    // FIXME make custom serialization
    //[SerializeReference] public SoundShapeFunction soundShapeFunction = new SupershapeSound();

    //public SoundShapeFunction soundShapeFunction = new SinewaveSound();
    public SoundShapeFunction soundShapeFunction = new SupershapeSound();

    [Range(1, 10000)]
    public float newFrequency = 128.0f;

    private float frequency;
    private double phase;
    private float sampleRate;
    private double angularFrequency;
    private bool running = false;

    public List<float> soundSamplesBufferTemp { get; set; }
    public List<float> soundSamplesBufferCurrent { get; set; }
    public bool newSampleBufferData { get; set; }

    private int numberOfSamples;
    private int currentSampleIndex;

    void Start()
    {
        SetupAndPlaySound();
    }

    public void SetupAndPlaySound()
    {
        // this doesn't seem to work
        // to change the number of samples i.e data length on OnAudioFilterRead
        //var configuration = AudioSettings.GetConfiguration();
        //configuration.dspBufferSize = 1024;
        //AudioSettings.Reset(configuration);

        //numberOfSamples = AudioSettings.GetDSPBufferSize(); // 1024 FIXME, confirm
        numberOfSamples = 1024;

        currentSampleIndex = 0;

        frequency = newFrequency;

        phase = 0.0;

        sampleRate = AudioSettings.outputSampleRate;

        angularFrequency = 2 * Mathf.PI * frequency / sampleRate;

        soundSamplesBufferTemp = new List<float>();

        soundSamplesBufferCurrent = soundShapeFunction.GenerateSamples(numberOfSamples, angularFrequency);

        newSampleBufferData = false;

        running = true;
    }

    private void Update()
    {
        if (NeedNewSamples())
        {
            if (frequency != newFrequency) // REFACTOR checking twice
            {
                frequency = newFrequency;
                angularFrequency = 2 * Mathf.PI * frequency / sampleRate;
            }

            soundSamplesBufferTemp = soundShapeFunction.GenerateSamples(numberOfSamples, angularFrequency);
            newSampleBufferData = true;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        int dataLen = data.Length / channels;
        numberOfSamples = dataLen;

        currentSampleIndex = 0;
        while (currentSampleIndex < dataLen)
        {
            if (CanSwapSamplesBuffer())
                SwapSamplesBuffer();

            int channelXid = currentSampleIndex * channels;
            int channelYid = currentSampleIndex * channels + 1;

            // populate the two channels
            data[channelXid] = soundSamplesBufferCurrent[channelXid];
            data[channelYid] = soundSamplesBufferCurrent[channelYid];

            phase += angularFrequency; // probably should use modulo to keep track of phase
            currentSampleIndex++;
        }
    }

    // swap sample buffers only if new buffer is ready and we have finished reproducing all samples from the previous buffer
    private bool CanSwapSamplesBuffer()
    {
        return newSampleBufferData && (currentSampleIndex == (numberOfSamples - 1));
    }

    private void SwapSamplesBuffer()
    {
        soundSamplesBufferCurrent = soundSamplesBufferTemp;
        newSampleBufferData = false;
    }


    // when shape parameters OR frequency/numberOfSamples/sampleRate get updatedstart calculating new samples
    // numberOfSamples and sampleRate will be static for now I think
    private bool NeedNewSamples()
    {
        return soundShapeFunction.ShapeFunctionParametersUpdated() || newFrequency != frequency;
    }
}
