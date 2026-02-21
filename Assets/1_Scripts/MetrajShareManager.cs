using UnityEngine;
using System.Text;
using System.IO;
using System.Globalization;

public class MetrajShareManager : MonoBehaviour
{
    public void ShareKayit(string kayitAdi, string tur, string detayJson)
    {
        CultureInfo turkce = new CultureInfo("tr-TR");
        StringBuilder csv = new StringBuilder();

        int firstDataRow = 4;

        if (tur == "Beton")
        {
            BetonDetayData data =
                JsonUtility.FromJson<BetonDetayData>(detayJson);

            int lastDataRow = 3 + data.satirlar.Count;
            string toplamFormula =
                $"=TOPLA(G{firstDataRow}:G{lastDataRow})";

            csv.AppendLine(
                "Kayýt Adý;" + kayitAdi +
                ";Genel Toplam (m3);" + toplamFormula);

            csv.AppendLine("");
            csv.AppendLine(
                "Ýmalat;Benzer;Adet;En;Boy;Yükseklik;Sonuç (m3)");

            for (int i = 0; i < data.satirlar.Count; i++)
            {
                var s = data.satirlar[i];
                int excelRow = i + firstDataRow;

                string formula =
                    $"=B{excelRow}*C{excelRow}*D{excelRow}*E{excelRow}*F{excelRow}";

                csv.AppendLine(
                    s.metrajAdi + ";" +
                    s.benzer.ToString("F2", turkce) + ";" +
                    s.adet.ToString("F2", turkce) + ";" +
                    s.en.ToString("F2", turkce) + ";" +
                    s.boy.ToString("F2", turkce) + ";" +
                    s.yukseklik.ToString("F2", turkce) + ";" +
                    formula
                );
            }
        }

        else if (tur == "Kalýp")
        {
            BetonDetayData data =
                JsonUtility.FromJson<BetonDetayData>(detayJson);

            int lastDataRow = 3 + data.satirlar.Count;
            string toplamFormula =
                $"=TOPLA(F{firstDataRow}:F{lastDataRow})";

            csv.AppendLine(
                "Kayýt Adý;" + kayitAdi +
                ";Genel Toplam (m2);" + toplamFormula);

            csv.AppendLine("");
            csv.AppendLine(
                "Ýmalat;Benzer;Adet;En;Boy;Sonuç (m2)");

            for (int i = 0; i < data.satirlar.Count; i++)
            {
                var s = data.satirlar[i];
                int excelRow = i + firstDataRow;

                string formula =
                    $"=B{excelRow}*C{excelRow}*D{excelRow}*E{excelRow}";

                csv.AppendLine(
                    s.metrajAdi + ";" +
                    s.benzer.ToString("F2", turkce) + ";" +
                    s.adet.ToString("F2", turkce) + ";" +
                    s.en.ToString("F2", turkce) + ";" +
                    s.boy.ToString("F2", turkce) + ";" +
                    formula
                );
            }
        }

        string fileName = kayitAdi.Replace(" ", "_") + ".csv";
        string path = Path.Combine(
            Application.temporaryCachePath, fileName);

        File.WriteAllText(path, csv.ToString(),
            new UTF8Encoding(true));

        new NativeShare()
            .AddFile(path)
            .SetSubject("Metraj Kaydý")
            .SetText("Metraj kaydýný paylaþýyorum.")
            .Share();
    }
}