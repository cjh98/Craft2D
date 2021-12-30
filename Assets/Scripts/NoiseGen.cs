using SimplexNoise;

public static class NoiseGen
{
    public static float GenerateNoise(float x, float y, float scale, float offset, int octaves, float frequency, float amplitude)
    {
        float value = 0;
        x = (x + 0.1f) * scale + offset;
        y = (y + 0.1f) * scale + offset;

        for (int i = 0; i < octaves; i++)
        {
            value += Noise.Generate(frequency * x, frequency * y) * amplitude;
            amplitude *= 0.5f;
            frequency *= 2.0f;
        }

        return value;
    }
}
