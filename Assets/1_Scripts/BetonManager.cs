using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Globalization;

public class BetonManager : MonoBehaviour
{
    [Header("Metraj Bilgileri")]
    public TMP_InputField inputMetrajAdi;

    [Header("Çarpanlar")]
    public TMP_InputField inputBenzer;
    public TMP_InputField inputAdet;
    public TMP_InputField inputBoy;
    public TMP_InputField inputEn;
    public TMP_InputField inputYukseklik;

    [Header("Toplam")]
    public TMP_Text txtToplamHacim;

    [Header("Liste")]
    public GameObject satirPrefab;
    public Transform listParent;

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();

    private int otomatikIsimSayac = 1;

    public void YeniMetrajEkle()
    {
        // --- METRAJ ADI ---
        string metrajAdi = inputMetrajAdi.text;

        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Hacim_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        // --- ÇARPANLAR (BOÞSA 1) ---
        float benzer = GetFloatOrOne(inputBenzer.text);
        float adet = GetFloatOrOne(inputAdet.text);
        float boy = GetFloatOrOne(inputBoy.text);
        float en = GetFloatOrOne(inputEn.text);
        float yukseklik = GetFloatOrOne(inputYukseklik.text);

        float hacim = benzer * adet * boy * en * yukseklik;

        string hesapOzet =
            benzer + " x " +
            adet + " x " +
            boy + " x " +
            en + " x " +
            yukseklik;

        GameObject yeni = Instantiate(satirPrefab, listParent);
        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

        int id = satirlar.Count;

        satir.Setup(metrajAdi, hesapOzet, hacim, this);
        satirlar.Add(satir);

        ToplamGuncelle();
        InputlariTemizle();
    }
    float GetFloatOrOne(string deger)
    {
        if (string.IsNullOrEmpty(deger))
            return 1f;

        if (float.TryParse(deger.Replace(",", "."),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out float sonuc))
            return sonuc;

        return 1f;
    }
    public void SatirSil(MetrajSatir satir)
    {
        satirlar.Remove(satir);
        ToplamGuncelle();
    }
    void ToplamGuncelle()
    {
        float toplam = 0f;

        foreach (var s in satirlar)
            toplam += s.GetHacim();

        txtToplamHacim.text = toplam.ToString("F2") + " m³";
    }
    void InputlariTemizle()
    {
        inputMetrajAdi.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputBoy.text = "";
        inputEn.text = "";
        inputYukseklik.text = "";
    }
}
