using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Globalization;

public class MetrajSatir : MonoBehaviour
{
    public TMP_Text txtMetrajAdi;
    public TMP_Text txtHesapDeger;
    public TMP_Text txtSonuc;

    private float hacimDegeri;
    private string metrajAdiDegeri;
    private string hesapOzetDegeri;

    // Sayýsal deðerler
    private float benzerDegeri;
    private float adetDegeri;
    private float enDegeri;
    private float boyDegeri;
    private float yukseklikDegeri;

    private IMetrajManager manager;

    // ------------------------------------------------------------------
    // STRING SETUP (Eski sistem - beton uyumlu)
    // ------------------------------------------------------------------
    public void Setup(string metrajAdi,
                      string hesapDeger,
                      float hacim,
                      IMetrajManager gelenManager)
    {
        manager = gelenManager;

        metrajAdiDegeri = metrajAdi;
        hesapOzetDegeri = hesapDeger;
        hacimDegeri = hacim;

        txtMetrajAdi.text = metrajAdi;
        txtHesapDeger.text = hesapDeger;
        txtSonuc.text = hacim.ToString("F2") + GetBirim();

        ParseHesapOzet(hesapDeger);
    }

    // ------------------------------------------------------------------
    // NUMERIC SETUP (JSON Load için)
    // ------------------------------------------------------------------
    public void SetupNumeric(string metrajAdi,
                             float benzer,
                             float adet,
                             float en,
                             float boy,
                             float yukseklik,
                             float sonuc,
                             IMetrajManager gelenManager)
    {
        manager = gelenManager;

        metrajAdiDegeri = metrajAdi;

        benzerDegeri = benzer;
        adetDegeri = adet;
        enDegeri = en;
        boyDegeri = boy;
        yukseklikDegeri = yukseklik;

        hacimDegeri = sonuc;

        hesapOzetDegeri = HesapOzetOlustur();

        txtMetrajAdi.text = metrajAdiDegeri;
        txtHesapDeger.text = hesapOzetDegeri;
        txtSonuc.text = hacimDegeri.ToString("F2") + GetBirim();
    }

    // ------------------------------------------------------------------
    // ÇARPIM ÖZETÝNÝ OLUÞTUR (Yükseklik 1 ise göstermez)
    // ------------------------------------------------------------------
    private string HesapOzetOlustur()
    {
        string ozet =
            benzerDegeri.ToString(CultureInfo.InvariantCulture) + " x " +
            adetDegeri.ToString(CultureInfo.InvariantCulture) + " x " +
            enDegeri.ToString(CultureInfo.InvariantCulture) + " x " +
            boyDegeri.ToString(CultureInfo.InvariantCulture);

        // Eðer yükseklik 1 deðilse ekle
        if (!Mathf.Approximately(yukseklikDegeri, 1f))
        {
            ozet += " x " + yukseklikDegeri.ToString(CultureInfo.InvariantCulture);
        }

        return ozet;
    }

    // ------------------------------------------------------------------
    // HESAP ÖZETÝ PARSE (CSV için gerekli)
    // ------------------------------------------------------------------
    private void ParseHesapOzet(string ozet)
    {
        if (string.IsNullOrEmpty(ozet))
            return;

        string temiz = ozet.Replace(" ", "");
        string[] parcalar = temiz.Split('x');

        if (parcalar.Length >= 4)
        {
            float.TryParse(parcalar[0], NumberStyles.Any, CultureInfo.InvariantCulture, out benzerDegeri);
            float.TryParse(parcalar[1], NumberStyles.Any, CultureInfo.InvariantCulture, out adetDegeri);
            float.TryParse(parcalar[2], NumberStyles.Any, CultureInfo.InvariantCulture, out enDegeri);
            float.TryParse(parcalar[3], NumberStyles.Any, CultureInfo.InvariantCulture, out boyDegeri);

            if (parcalar.Length >= 5)
                float.TryParse(parcalar[4], NumberStyles.Any, CultureInfo.InvariantCulture, out yukseklikDegeri);
            else
                yukseklikDegeri = 1f;
        }
    }

    // ------------------------------------------------------------------
    // TÜRE GÖRE BÝRÝM
    // ------------------------------------------------------------------
    private string GetBirim()
    {
        if (manager == null)
            return "";

        switch (manager.GetMetrajTuru())
        {
            case "Beton":
                return " m³";
            case "Kalýp":   // <-- düzelttik
                return " m²";
            case "Demir":
                return " Kg";
            default:
                return "";
        }
    }

    // ------------------------------------------------------------------
    public void Sil()
    {
        if (manager != null)
            manager.SatirSil(this);

        Destroy(gameObject);
    }

    // ------------------------------------------------------------------
    // GETTER'LAR (CSV / JSON için)
    // ------------------------------------------------------------------
    public float GetHacim() => hacimDegeri;
    public string GetMetrajAdi() => metrajAdiDegeri;
    public string GetHesapOzet() => hesapOzetDegeri;

    public float GetBenzer() => benzerDegeri;
    public float GetAdet() => adetDegeri;
    public float GetEn() => enDegeri;
    public float GetBoy() => boyDegeri;
    public float GetYukseklik() => yukseklikDegeri;
}