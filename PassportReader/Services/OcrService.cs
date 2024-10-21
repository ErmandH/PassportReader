using Microsoft.AspNetCore.Cors.Infrastructure;
using Tesseract;

namespace PassportReader.Services
{
    public class OcrService 
    {
        private readonly string _tesseractDataPath;

        public OcrService()
        {
            // wwwroot klasörüne göre Tesseract data yolunu belirleyin
            _tesseractDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
        }

        public string ExtractTextFromImage(string imagePath)
        {
            try
            {
                // Tesseract motorunu başlat ve Türkçe dil paketini kullan
                using (var engine = new TesseractEngine(_tesseractDataPath, "tur", EngineMode.Default))
                {
                    // Görüntüyü yükleyin ve OCR işlemini başlatın
                    using (var img = Pix.LoadFromFile(imagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            // OCR sonucunu metin olarak geri döndür
                            return page.GetText();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata durumunda ilgili hata mesajını geri döndür
                return $"Hata: {ex.Message}";
            }
        }
    }

}
