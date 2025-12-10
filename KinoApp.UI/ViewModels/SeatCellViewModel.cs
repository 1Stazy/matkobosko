using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KinoApp.UI.ViewModels
{
    public class SeatCellViewModel : ObservableObject
    {
        public int MiejsceId { get; set; }
        public int Rzad { get; set; }
        public int Kolumna { get; set; }

        private bool isAvailable;
        public bool IsAvailable
        {
            get => isAvailable;
            set => SetProperty(ref isAvailable, value);
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public string Display => $"{Rzad}-{Kolumna}";
    }
}
