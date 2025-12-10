using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace KinoApp.UI.Views
{
    public partial class PdfViewerWindow : Window
    {
        private readonly string _path;

        public PdfViewerWindow(string pdfPath)
        {
            InitializeComponent();
            _path = pdfPath;
            Loaded += PdfViewerWindow_Loaded;
        }

        private async void PdfViewerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await EnsureWebViewInitialized();
                // WebView2 może otwierać pliki lokalne przez file://
                var uri = new Uri(_path).AbsoluteUri;
                webView.CoreWebView2.Navigate(uri);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd inicjalizacji podglądu PDF: " + ex.Message);
            }
        }

        private async Task EnsureWebViewInitialized()
        {
            if (webView.CoreWebView2 != null) return;
            var env = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(env);
        }
    }
}
