using CommunityToolkit.Mvvm.ComponentModel;
using ConnectFour.Engine;

namespace ConnectFour.Desktop.ViewModels;

public sealed partial class CellViewModel : ViewModelBase
{
    public int Row { get; }
    public int Column { get; }

    [ObservableProperty]
    private CellState _state = CellState.Empty;

    [ObservableProperty]
    private bool _isWinningCell;

    [ObservableProperty]
    private bool _isLandingPreview;

    public CellViewModel(int row, int column)
    {
        Row = row;
        Column = column;
    }
}
