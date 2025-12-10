using KinoApp.Core.Models;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;

namespace KinoApp.UI.Services
{
    public interface IPdfService
    {
        byte[] GenerateTicketPdf(Bilet bilet, Rezerwacja rezerwacja);
    }

    public class PdfService : IPdfService
    {
        private readonly string? _logoPath;

        public PdfService()
        {
            var candidate = Path.Combine(AppContext.BaseDirectory, "Resources", "logo.png");
            _logoPath = File.Exists(candidate) ? candidate : null;
        }

        public byte[] GenerateTicketPdf(Bilet bilet, Rezerwacja rezerwacja)
        {
            if (rezerwacja == null) throw new ArgumentNullException(nameof(rezerwacja));

            // Generowanie QR
            using var qrGen = new QRCodeGenerator();
            var qrData = qrGen.CreateQrCode(bilet.Numer, QRCodeGenerator.ECCLevel.Q);
            using var qr = new PngByteQRCode(qrData);
            var qrBytes = qr.GetGraphic(20);

            var miejscaText = string.Join(", ", rezerwacja.Miejsca.Select(m => $"{m.Rzad}-{m.Kolumna}"));

            using var ms = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    // Rozmiar biletu
                    page.Size(new PageSize(180, 60, Unit.Millimetre));
                    page.Margin(10);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Black));

                    // GŁÓWNY UKŁAD TO WIERSZ (ROW)
                    page.Content().Row(row =>
                    {
                        // --- LEWA STRONA (DANE) - zajmuje 3/4 szerokości ---
                        row.RelativeItem(2).PaddingRight(10).Column(col =>
                        {
                            // 1. Logo i Nagłówek
                            col.Item().Row(headerRow =>
                            {
                                // ELEMENT 1: Logo (AutoItem - zajmuje tylko tyle miejsca ile potrzebuje)
                                headerRow.AutoItem().PaddingRight(10).AlignMiddle().Element(e =>
                                {
                                    if (!string.IsNullOrEmpty(_logoPath))
                                        // Zmniejszyłem Height, żeby ładnie pasowało do tekstu
                                        e.Height(30).Image(_logoPath); 
                                    else
                                        e.Text("NEON").Bold().FontSize(14).FontColor(Colors.Red.Medium);
                                });

                                // ELEMENT 2: Tytuł (RelativeItem - zajmuje całą środkową przestrzeń)
                                // AlignLeft sprawia, że tekst jest zaraz po logo.
                                headerRow.RelativeItem().AlignLeft().AlignMiddle().Text(rezerwacja.Seans.Film.Tytul)
                                    .Bold().FontSize(16).Italic().FontColor("#222");

                                // ELEMENT 3: Cena (AutoItem - przyklejona do prawej krawędzi)
                                // Jest wypchnięta na prawo przez RelativeItem tytułu.
                                headerRow.AutoItem().AlignRight().AlignMiddle().Text($"{bilet.Cena:C}")
                                    .Bold().FontSize(14).FontColor("#AA0000");
                            });

                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);


                            // 3. Miejsca (Bardzo duże)
                            col.Item().PaddingTop(2).Text($"Miejsca: {miejscaText}")
                                .FontSize(14).Bold();
                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            // 4. Sala i Data
                            col.Item().Text($"Sala: {rezerwacja.Seans.Sala.Nazwa} | {rezerwacja.Seans.DataCzas:g}")
                                .FontSize(14).Bold();

                            // 5. Małe info na dole po lewej
                            col.Item().Text($"Bilet: {bilet.Numer}")
                                .FontSize(10).FontColor(Colors.Grey.Darken1);
                            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            col.Item().PaddingTop(5).AlignMiddle().AlignCenter().Text("Prosimy o przybycie 10 min przed seansem.")
                                .FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                        });

                        // --- CZERWONA LINIA PIONOWA ---
                        row.AutoItem().LineVertical(2).LineColor(Colors.Red.Medium);

                        // --- PRAWA STRONA (QR) - zajmuje 1/4 szerokości ---
                        row.RelativeItem(1).PaddingLeft(10).AlignMiddle().AlignCenter().Element(e =>
                        {
                             // FitArea sprawi, że QR wypełni dostępną przestrzeń
                             e.Image(qrBytes).FitArea();
                        });
                    });
                });
            })
            .GeneratePdf(ms);

            return ms.ToArray();
        }
    }
}