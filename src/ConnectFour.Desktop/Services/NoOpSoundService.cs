namespace ConnectFour.Desktop.Services;

public sealed class NoOpSoundService : ISoundService
{
    public void PlayDrop() { }
    public void PlayWin() { }
}
