using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Globalization;

public class MetrajKayitManager : MonoBehaviour
{
    private string path;
    private MetrajKayitList kayitList = new MetrajKayitList();

    [Header("Liste UI")]
    public GameObject satirPrefab;
    public Transform listParent;
    public BetonManager betonManager;


    public static string duzenlemeJson;


    void Awake()
    {
        path = Application.persistentDataPath + "/metraj_kayitlar.json";
        Yukle();
    }

    // --------------------
    // KAYIT EKLE
    // --------------------
    public void YeniKayitEkle(string kayitAdi,
                              string tur,
                              float toplam,
                              string detayJson)
    {
        MetrajKayitData yeni = new MetrajKayitData();

        yeni.id = Guid.NewGuid().ToString();
        yeni.kayitAdi = kayitAdi;
        yeni.kayitTuru = tur;
        yeni.kayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        yeni.toplamMetraj = toplam;
        yeni.detayJson = detayJson;

        kayitList.kayitlar.Add(yeni);

        Kaydet();
        ListeyiDoldur(); 
        betonManager.InputlariTemizle();
    }

    // --------------------
    // SÝL
    // --------------------
    public void KayitSil(string id)
    {
        kayitList.kayitlar.RemoveAll(x => x.id == id);
        Kaydet();
    }

    // --------------------
    // TÜM LÝSTE
    // --------------------
    public List<MetrajKayitData> TumKayitlariGetir()
    {
        return kayitList.kayitlar;
    }

    // --------------------
    // JSON YAZ
    // --------------------
    void Kaydet()
    {
        string json = JsonUtility.ToJson(kayitList, true);
        File.WriteAllText(path, json);
        Debug.Log("Kayýtlar kaydedildi: ");
    }

    // --------------------
    // JSON OKU
    // --------------------
    void Yukle()
    {
        if (!File.Exists(path))
        {
            kayitList = new MetrajKayitList();
            return;
        }

        string json = File.ReadAllText(path);
        kayitList = JsonUtility.FromJson<MetrajKayitList>(json);
    }
    // --------------------
    // LÝSTEYÝ DOLDUR
    // --------------------
    public void ListeyiDoldur()
    {
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        foreach (var kayit in kayitList.kayitlar)
        {
            GameObject yeni = Instantiate(satirPrefab, listParent);
            MetrajKayitSatirUI ui = yeni.GetComponent<MetrajKayitSatirUI>();
            ui.Setup(kayit, this, betonManager);
            ShareButton share = yeni.GetComponent<ShareButton>();
            share.kayitData = kayit;
        }
    }

    public string SeciliKaydiCsvOlustur(MetrajKayitData kayit)
    {
        StringBuilder sb = new StringBuilder();

        BetonDetayData detayData =
            JsonUtility.FromJson<BetonDetayData>(kayit.detayJson);

        // ÜST BÝLGÝ
        sb.AppendLine($"KAYIT: {kayit.kayitAdi}");
        sb.AppendLine($"GENEL TOPLAM: {kayit.toplamMetraj.ToString("F2", CultureInfo.InvariantCulture)} m3");
        sb.AppendLine("");

        // TABLO BAÞLIK
        sb.AppendLine("Ýmalat,Benzer,Adet,En,Boy,Yükseklik,Sonuç (m3)");

        foreach (var satir in detayData.satirlar)
        {
            string benzer = "";
            string adet = "";
            string en = "";
            string boy = "";
            string yukseklik = "";

            if (!string.IsNullOrEmpty(satir.hesapOzet) && satir.hesapOzet.Contains("="))
            {
                string solTaraf = satir.hesapOzet.Split('=')[0];  // 1x2x2x3x4
                string[] parcalar = solTaraf.Split('x');

                if (parcalar.Length >= 5)
                {
                    benzer = parcalar[0];
                    adet = parcalar[1];
                    en = parcalar[2];
                    boy = parcalar[3];
                    yukseklik = parcalar[4];
                }
            }

            sb.AppendLine(
                $"{satir.metrajAdi}," +
                $"{benzer}," +
                $"{adet}," +
                $"{en}," +
                $"{boy}," +
                $"{yukseklik}," +
                $"{satir.hacim.ToString("F2", CultureInfo.InvariantCulture)}"
            );
        }

        return sb.ToString();
    }
}
