#define FLT_MAX 3.402823466e+38
#define mod(x,y) (x-y*floor(x/y))

float sdSegment(in float2 p, in float2 a, in float2 b)
{
    float2 pa = p - a, ba = b - a;
    float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
    return length(pa - ba * h);
}


float sdSound(float2 uv, int totalSoundSamples, StructuredBuffer<float> soundSamplesBuffer)
{
    float hits;

    for (float i = 0.; i < totalSoundSamples; i+=4) {
        float2 p1 = float2(soundSamplesBuffer[i], soundSamplesBuffer[i + 1]);
        float2 p2 = float2(soundSamplesBuffer[i + 2], soundSamplesBuffer[i + 3]);

        hits += min(1., 1. / (sdSegment(uv, p1, p2) * 2500.));

    }
    
    return 200. * hits / totalSoundSamples;
}


float2 cube(float2 uv) {
    return mod((uv + .5) * 8., float2(1., 1.)) - .5;
}

float sdBox(in float2 p, in float2 b)
{
    float2 d = abs(p) - b;
    return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}