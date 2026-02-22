using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DemirManagerOld : MonoBehaviour, IMetrajManager
{
    [Header("Input Alanlarý")]
    public TMP_InputField inputMetrajAdi;
    public TMP_InputField inputCap;
    public TMP_InputField inputBenzer;
    public TMP_InputField inputAdet;
    public TMP_InputField inputBoy;

    [Header("UI Referanslarý")]
    public Transform satirParent;
    public GameObject metrajSatirPrefab;
    public TMP_Text txtGenelToplam;

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();
    private const float ozgulAgirlik = 7850f;
    private int otomatikIsimSayac = 1;

    #region INTERFACE

    public void Hesapla()
    {
        // Boþ býrakýlýrsa 1 kabul et
        float capCm = GetFloatOrOne(inputCap.text);
        float benzer = GetFloatOrOne(inputBenzer.text);
        float adet = GetFloatOrOne(inputAdet.text);
        float boy = GetFloatOrOne(inputBoy.text);

        float capMetre = capCm / 100f;
        float yaricap = capMetre / 2f;
        float hacim = Mathf.PI * yaricap * yaricap * boy;
        float toplamKg = hacim * ozgulAgirlik * adet * benzer;
        float ton = toplamKg / 1000f;

        // Metraj adý boþsa otomatik isim
        string metrajAdi = inputMetrajAdi.text;
        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Demir_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        // Satýrý ekle
        SatirEkle(metrajAdi, capCm, benzer, adet, boy, ton);

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

    public float GetToplamMetrajValue() => GetGenelToplam();

    public string GetMetrajTuru() => "Demir";

    public void TumSatirlariTemizle()
    {
        foreach (var s in satirlar)
            Destroy(s.gameObject);
        satirlar.Clear();
        otomatikIsimSayac = 1;
        GenelToplamGuncelle();
    }

    public string GetDetayJson()
    {
        DemirDetayData data = new DemirDetayData();
        foreach (var s in satirlar)
        {
            data.satirlar.Add(new DemirSatirData
            {
                metrajAdi = s.GetMetrajAdi(),
                cap = s.GetEn(),     // Çap
                benzer = s.GetBenzer(),
                adet = s.GetAdet(),
                boy = s.GetBoy(),
                hacim = s.GetHacim()
            });
        }
        return JsonUtility.ToJson(data);
    }

    public void LoadFromJson(string json)
    {
        TumSatirlariTemizle();
        if (string.IsNullOrEmpty(json)) return;

        DemirDetayData data = JsonUtility.FromJson<DemirDetayData>(json);
        if (data == null || data.satirlar == null) return;

        foreach (var d in data.satirlar)
        {
            string ad = string.IsNullOrEmpty(d.metrajAdi) ? "Demir_" + otomatikIsimSayac++ : d.metrajAdi;
            SatirEkle(ad, d.cap, d.benzer, d.adet, d.boy, d.hacim);
        }
    }

    #endregion

    #region PRIVATE

    private void SatirEkle(string ad, float cap, float benzer, float adet, float boy, float ton)
    {
        GameObject yeni = Instantiate(metrajSatirPrefab, satirParent);
        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();
        // Çap - Benzer - Adet - Boy sýralamasý
        satir.SetupNumeric(ad, benzer, adet, cap, boy, 1f, ton, this);
        satirlar.Add(satir);
    }

    private void GenelToplamGuncelle()
    {
        txtGenelToplam.text = GetGenelToplam().ToString("F2") + " ton";
    }

    private void InputTemizle()
    {
        inputMetrajAdi.text = "";
        inputCap.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputBoy.text = "";
    }

    private float GetFloatOrOne(string val)
    {
        if (string.IsNullOrWhiteSpace(val))
            return 1f;

        val = val.Trim();

        if (float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out float sonuc))
            return sonuc;

        if (float.TryParse(val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sonuc))
            return sonuc;

        if (float.TryParse(val.Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sonuc))
            return sonuc;

        return 1f;
    }

    #endregion
}