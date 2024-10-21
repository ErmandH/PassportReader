using System.Diagnostics;
using Newtonsoft.Json;

namespace PassportReader.Services
{
    public class MrzService
    {
        public static DateTime ParseMrzDate(string mrzDate)
        {
            // İlk iki rakam yılı temsil eder, 1900 veya 2000 olup olmadığını kontrol edelim.
            int year = int.Parse(mrzDate.Substring(0, 2));
            int month = int.Parse(mrzDate.Substring(2, 2));
            int day = int.Parse(mrzDate.Substring(4, 2));

            // 2000'den sonraki tarihler için 2000 yılı, aksi takdirde 1900 yılı kullanıyoruz.
            if (year < 50)  // 2050'den önceki tarihleri kapsar
            {
                year += 2000;
            }
            else
            {
                year += 1900;
            }

            return new DateTime(year, month, day);
        }
        public MrzData ParseMrz(string imagePath)
        {
            // Terminalde çalıştırmak istediğimiz komut
            string tesseractPath = @"C:\Program Files\Tesseract-OCR";
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + tesseractPath);
            string command = $"mrz {imagePath} --json";

            // Terminalde çalıştır ve sonucu al
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Eğer hata varsa hata mesajını da al
            string error = process.StandardError.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception("Error executing MRZ command: " + error);
            }

            // JSON formatındaki sonucu deserialize et
            var mrzData = JsonConvert.DeserializeObject<MrzData>(result);

            DateTime birthDate = ParseMrzDate(mrzData.date_of_birth);
            DateTime expirationDate = ParseMrzDate(mrzData.expiration_date);

            mrzData.date_of_birth = birthDate.ToString("dd/MM/yyyy");
            mrzData.expiration_date = expirationDate.ToString("dd/MM/yyyy");

            string tcno = mrzData.personal_number;
            mrzData.personal_number = new string(tcno.Where(char.IsDigit).ToArray());

            return mrzData;
        }
    }

    public class MrzData
    {

        public string country { get; set; }
        public string number { get; set; }
        public string date_of_birth { get; set; }
        public string expiration_date { get; set; }
        public string nationality { get; set; }
        public string sex { get; set; }
        public string names { get; set; }
        public string surname { get; set; }
        public string personal_number { get; set; }
        public string filename { get; set; }
    }
}
