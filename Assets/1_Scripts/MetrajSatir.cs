using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MetrajSatir : MonoBehaviour
{
    public TMP_Text txtMetrajAdi;
    public TMP_Text txtHesapDeger;
    public TMP_Text txtSonuc;

    private float hacimDegeri;
    private string metrajAdiDegeri;
    private string hesapOzetDegeri;

    // YENÝ: Gerçek sayýsal deðerler
    private float benzerDegeri;
    private float adetDegeri;
    private float enDegeri;
    private float boyDegeri;
    private float yukseklikDegeri;

    private BetonManager manager;

    public void Setup(string metrajAdi,
                      string hesapDeger,
                      float hacim,
                      BetonManager betonManager)
    {
        txtMetrajAdi.text = metrajAdi;
        txtHesapDeger.text = hesapDeger;
        txtSonuc.text = hacim.ToString("F2") + " m³";

        metrajAdiDegeri = metrajAdi;
        hesapOzetDegeri = hesapDeger;
        hacimDegeri = hacim;
        manager = betonManager;

        // Hesap özetinden sayýsal deðerleri parse ediyoruz
        ParseHesapOzet(hesapDeger);
    }

    private void ParseHesapOzet(string ozet)
    {
        if (string.IsNullOrEmpty(ozet))
            return;

        string temiz = ozet.Replace(" ", "");
        string[] parcalar = temiz.Split('x');

        if (parcalar.Length >= 5)
        {
            float.TryParse(parcalar[0], out benzerDegeri);
            float.TryParse(parcalar[1], out adetDegeri);
            float.TryParse(parcalar[2], out enDegeri);
            float.TryParse(parcalar[3], out boyDegeri);
            float.TryParse(parcalar[4], out yukseklikDegeri);
        }
    }

    public void Sil()
    {
        manager.SatirSil(this);
        Destroy(gameObject);
    }

    public float GetHacim()
    {
        return hacimDegeri;
    }

    public string GetMetrajAdi()
    {
        return metrajAdiDegeri;
    }

    public string GetHesapOzet()
    {
        return hesapOzetDegeri;
    }

    // YENÝ GETTER'LAR (CSV ve JSON için kullanýlacak)

    public float GetBenzer() => benzerDegeri;
    public float GetAdet() => adetDegeri;
    public float GetEn() => enDegeri;
    public float GetBoy() => boyDegeri;
    public float GetYukseklik() => yukseklikDegeri;


    // YENÝ SAYISAL SETUP (JSON Load için)
    public void SetupNumeric(string metrajAdi,
                             float benzer,
                             float adet,
                             float en,
                             float boy,
                             float yukseklik,
                             float hacim,
                             BetonManager betonManager)
    {
        metrajAdiDegeri = metrajAdi;

        benzerDegeri = benzer;
        adetDegeri = adet;
        enDegeri = en;
        boyDegeri = boy;
        yukseklikDegeri = yukseklik;

        hacimDegeri = hacim;
        manager = betonManager;

        // Hesap özetini artýk biz oluþturuyoruz
        hesapOzetDegeri =
            benzer + " x " +
            adet + " x " +
            en + " x " +
            boy + " x " +
            yukseklik;

        txtMetrajAdi.text = metrajAdiDegeri;
        txtHesapDeger.text = hesapOzetDegeri;
        txtSonuc.text = hacimDegeri.ToString("F2") + " m³";
    }
}