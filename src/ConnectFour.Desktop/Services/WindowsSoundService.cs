using System.Media;
using System.Runtime.Versioning;

namespace ConnectFour.Desktop.Services;

[SupportedOSPlatform("windows")]
public sealed class WindowsSoundService : ISoundService, IDisposable
{
    private readonly SoundPlayer _drop;
    private readonly SoundPlayer _win;

    public WindowsSoundService()
    {
        _drop = new SoundPlayer(new MemoryStream(WaveGenerator.Tone(180, 0.18)));
        _drop.Load();
        _win = new SoundPlayer(new MemoryStream(WaveGenerator.Chord(new[] { 523.25, 659.25, 783.99 }, 0.6)));
        _win.Load();
    }

    public void PlayDrop() => _drop.Play();
    public void PlayWin() => _win.Play();

    public void Dispose()
    {
        _drop.Dispose();
        _win.Dispose();
    }
}
