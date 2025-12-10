using System.Windows.Controls;
namespace KinoApp.UI.Views.Placeholders
{
    public partial class SimpleTextView : UserControl
    {
        public string Text { get; set; }

        public SimpleTextView(string text)
        {
            InitializeComponent();
            Text = text;
            DataContext = this;
        }
    }
}
