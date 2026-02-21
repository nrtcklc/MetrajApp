using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DemirManager : MonoBehaviour, IMetrajManager
{
    [Header("Input Alanlarý")]
    public TMP_InputField inputMetrajAdi;
    public TMP_InputField inputBenzer;
    public TMP_InputField inputAdet;
    public TMP_InputField inputCap;      // cm
    public TMP_InputField inputBoy;      // metre

    [Header("UI Referanslarý")]
    public Transform satirParent;
    public GameObject metrajSatirPrefab;
    public TMP_Text txtGenelToplam;

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();

    private const float ozgulAgirlik = 7850f; // kg/m³

    #region INTERFACE

    public float GetToplamMetrajValue()
    {
        return GetGenelToplam();
    }

    public string GetDetayJson()
    {
        return ""; // Þimdilik boþ
    }

    public void TumSatirlariTemizle()
    {
        foreach (var s in satirlar)
        {
            Destroy(s.gameObject);
        }

        satirlar.Clear();
        GenelToplamGuncelle();
    }

    public string GetMetrajTuru()
    {
        return "Demir";
    }

    public void Hesapla()
    {
        float benzer = Parse(inputBenzer.text);
        float adet = Parse(inputAdet.text);
        float capCm = Parse(inputCap.text);
        float boy = Parse(inputBoy.text);

        if (benzer <= 0 || adet <= 0 || capCm <= 0 || boy <= 0)
            return;

        // Çap metreye çevrilir
        float capMetre = capCm / 100f;
        float yaricap = capMetre / 2f;

        // Tek çubuk hacmi (m³)
        float hacim = Mathf.PI * yaricap * yaricap * boy;

        // Tek çubuk kg
        float kg = hacim * ozgulAgirlik;

        // Toplam kg
        float toplamKg = kg * adet * benzer;

        // Ton
        float ton = toplamKg / 1000f;

        SatirEkle(inputMetrajAdi.text, benzer, adet, capCm, boy, ton);

        InputTemizle();
        GenelToplamGuncelle();
    }

    public void SatirSil(MetrajSatir satir)
    {
        satirlar.Remove(satir);
        GenelToplamGuncelle();
    }

    public float GetGenelToplam()
    {
        float toplam = 0f;

        foreach (var s in satirlar)
            toplam += s.GetHacim();

        return toplam;
    }

    public void LoadFromJson(string json)
    {
        // Þimdilik boþ býrakýyoruz.
        // Beton ve Kalýp gibi istersen sonra doldururuz.
    }

    #endregion

    #region PRIVATE

    private void SatirEkle(string ad,
                           float benzer,
                           float adet,
                           float cap,
                           float boy,
                           float ton)
    {
        GameObject yeniSatir = Instantiate(metrajSatirPrefab, satirParent);
        MetrajSatir satir = yeniSatir.GetComponent<MetrajSatir>();

        satir.SetupNumeric(
            ad,
            benzer,
            adet,
            cap,     // en yerine çap
            boy,
            1f,      // yükseklik 1 (görünmeyecek)
            ton,
            this
        );

        satirlar.Add(satir);
    }

    private void GenelToplamGuncelle()
    {
        txtGenelToplam.text = GetGenelToplam().ToString("F3") + " ton";
    }

    private void InputTemizle()
    {
        inputMetrajAdi.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputCap.text = "";
        inputBoy.text = "";
    }

    private float Parse(string deger)
    {
        float.TryParse(deger, out float sonuc);
        return sonuc;
    }

    #endregion
}