using UnityEngine;
using System.Text;
using System.IO;
using System.Globalization;

public class MetrajShareManager : MonoBehaviour
{
    public void ShareBetonKaydi(string kayitAdi, string detayJson, float genelToplam)
    {
        BetonDetayData data = JsonUtility.FromJson<BetonDetayData>(detayJson);

        CultureInfo turkce = new CultureInfo("tr-TR");

        StringBuilder csv = new StringBuilder();

        int firstDataRow = 4;
        int lastDataRow = 3 + data.satirlar.Count;

        // GENEL TOPLAM FORMÜLÜ
        string toplamFormula = $"=TOPLA(G{firstDataRow}:G{lastDataRow})";

        // ÜST BÝLGÝ SATIRI (ARTIK FORMÜLLÜ)
        csv.AppendLine("Kayýt Adý;" + kayitAdi + ";Genel Toplam (m3);" + toplamFormula);
        csv.AppendLine("");

        // TABLO BAÞLIKLARI
        csv.AppendLine("Ýmalat;Benzer;Adet;En (m.);Boy (m.);Yükseklik (m.);Sonuç (m3)");

        // SATIRLAR
        for (int i = 0; i < data.satirlar.Count; i++)
        {
            var satir = data.satirlar[i];

            int excelRow = i + firstDataRow;

            string formula = $"=B{excelRow}*C{excelRow}*D{excelRow}*E{excelRow}*F{excelRow}";

            csv.AppendLine(
                satir.metrajAdi + ";" +
                satir.benzer.ToString("F2", turkce) + ";" +
                satir.adet.ToString("F2", turkce) + ";" +
                satir.en.ToString("F2", turkce) + ";" +
                satir.boy.ToString("F2", turkce) + ";" +
                satir.yukseklik.ToString("F2", turkce) + ";" +
                formula
            );
        }

        string fileName = kayitAdi.Replace(" ", "_") + ".csv";
        string path = Path.Combine(Application.temporaryCachePath, fileName);

        File.WriteAllText(path, csv.ToString(), new UTF8Encoding(true));

        new NativeShare()
            .AddFile(path)
            .SetSubject("Metraj Kaydý")
            .SetText("Metraj kaydýný paylaþýyorum.")
            .Share();
    }
}