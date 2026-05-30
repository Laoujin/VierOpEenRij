namespace ConnectFour.Desktop.Services;

internal static class WaveGenerator
{
    private const int SampleRate = 44_100;
    private const short BitsPerSample = 16;
    private const short Channels = 1;

    public static byte[] Tone(double frequencyHz, double durationSeconds, double amplitude = 0.4)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        var samples = new short[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)SampleRate;
            double envelope = Math.Min(1.0, Math.Min(t / 0.01, (durationSeconds - t) / 0.05));
            double sample = Math.Sin(2 * Math.PI * frequencyHz * t) * amplitude * envelope;
            samples[i] = (short)(sample * short.MaxValue);
        }
        return BuildWav(samples);
    }

    public static byte[] Chord(double[] frequenciesHz, double durationSeconds, double amplitude = 0.3)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        var samples = new short[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            double t = i / (double)SampleRate;
            double envelope = Math.Min(1.0, Math.Min(t / 0.01, (durationSeconds - t) / 0.10));
            double sum = 0;
            foreach (var f in frequenciesHz)
                sum += Math.Sin(2 * Math.PI * f * t);
            sum = sum / frequenciesHz.Length * amplitude * envelope;
            samples[i] = (short)(sum * short.MaxValue);
        }
        return BuildWav(samples);
    }

    private static byte[] BuildWav(short[] samples)
    {
        int dataSize = samples.Length * sizeof(short);
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);
        w.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        w.Write(36 + dataSize);
        w.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
        w.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        w.Write(16);
        w.Write((short)1);
        w.Write(Channels);
        w.Write(SampleRate);
        w.Write(SampleRate * Channels * BitsPerSample / 8);
        w.Write((short)(Channels * BitsPerSample / 8));
        w.Write(BitsPerSample);
        w.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        w.Write(dataSize);
        foreach (var s in samples) w.Write(s);
        return ms.ToArray();
    }
}
