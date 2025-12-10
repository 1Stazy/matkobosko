using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace KinoApp.UI.Controls
{
    public partial class SeatMapControl : UserControl
    {
        public SeatMapControl()
        {
            InitializeComponent();
        }

        // Seats jako IEnumerable - bardziej tolerancyjne. Binduj do ObservableCollection<SeatCellViewModel>.
        public static readonly DependencyProperty SeatsProperty =
            DependencyProperty.Register(nameof(Seats), typeof(IEnumerable), typeof(SeatMapControl), new PropertyMetadata(null));

        public IEnumerable Seats
        {
            get => (IEnumerable)GetValue(SeatsProperty);
            set => SetValue(SeatsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(SeatMapControl), new PropertyMetadata(12));

        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }
    }
}
