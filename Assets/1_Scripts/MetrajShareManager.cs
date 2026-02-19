using UnityEngine;
using System.Text;
using System.IO;
using System.Globalization;

public class MetrajShareManager : MonoBehaviour
{
    public void ShareBetonKaydi(string kayitAdi, string detayJson, float genelToplam)
    {
        BetonDetayData data = JsonUtility.FromJson<BetonDetayData>(detayJson);

        StringBuilder csv = new StringBuilder();

        csv.AppendLine("Kayýt Adý:;" + kayitAdi);
        csv.AppendLine("Genel Toplam (m3);" + genelToplam.ToString("F2", CultureInfo.InvariantCulture));
        csv.AppendLine("");

        csv.AppendLine("Ýmalat;Hesap Özeti;Sonuç (m3)");

        foreach (var satir in data.satirlar)
        {
            csv.AppendLine(
                satir.metrajAdi + ";" +
                satir.hesapOzet + ";" +
                satir.hacim.ToString("F2", CultureInfo.InvariantCulture)
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
