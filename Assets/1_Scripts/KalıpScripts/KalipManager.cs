using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KalipManager : MonoBehaviour, IMetrajManager
{
    [Header("Metraj Bilgileri")]
    public TMP_InputField inputMetrajAdi;

    [Header("Çarpanlar")]
    public TMP_InputField inputBenzer;
    public TMP_InputField inputAdet;
    public TMP_InputField inputEn;
    public TMP_InputField inputBoy;

    [Header("Toplam")]
    public TMP_Text txtToplamKalip;

    [Header("Liste")]
    public GameObject satirPrefab;
    public Transform listParent;

    [Header("Scroll")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform listParentAnim;
    [SerializeField] float animSure = 0.25f;

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();
    private int otomatikIsimSayac = 1;

    // --------------------------------------------------
    // YENÝ METRAJ EKLE
    // --------------------------------------------------
    public void YeniMetrajEkle()
    {
        string metrajAdi = inputMetrajAdi.text;

        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Kalip_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        float benzer = ParseOrOne(inputBenzer.text);
        float adet = ParseOrOne(inputAdet.text);
        float en = ParseOrOne(inputEn.text);
        float boy = ParseOrOne(inputBoy.text);

        float alan = benzer * adet * en * boy;

        GameObject yeni = Instantiate(satirPrefab, listParent, false);
        yeni.transform.SetSiblingIndex(0);

        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

        satir.SetupNumeric(
            metrajAdi,
            benzer,
            adet,
            en,
            boy,
            1f,         // yükseklik 1 (görünmeyecek)
            alan,
            this
        );

        satirlar.Add(satir);

        ToplamGuncelle();
        InputlariTemizle();

        //Canvas.ForceUpdateCanvases();
        //scrollRect.verticalNormalizedPosition = 1f;
        yeni.transform.SetSiblingIndex(0);
        yeni.SetActive(false);

        StartCoroutine(SatirAnimasyon(yeni));
    }

    // --------------------------------------------------
    // SATIR SÝL
    // --------------------------------------------------
    public void SatirSil(MetrajSatir satir)
    {
        if (satirlar.Contains(satir))
        {
            satirlar.Remove(satir);
            Destroy(satir.gameObject);
            ToplamGuncelle();
        }
    }

    // --------------------------------------------------
    // TOPLAM HESAP
    // --------------------------------------------------
    public float GetToplamMetrajValue()
    {
        float toplam = 0f;

        foreach (var s in satirlar)
            toplam += s.GetHacim();

        return toplam;
    }

    void ToplamGuncelle()
    {
        float toplam = GetToplamMetrajValue();
        txtToplamKalip.text = "Toplam Kalýp: " + toplam.ToString("F2") + " m²";
    }

    // --------------------------------------------------
    // JSON ÜRET
    // --------------------------------------------------
    public string GetDetayJson()
    {
        BetonDetayData data = new BetonDetayData();

        foreach (var s in satirlar)
        {
            BetonSatirData satirData = new BetonSatirData();

            satirData.metrajAdi = s.GetMetrajAdi();
            satirData.benzer = s.GetBenzer();
            satirData.adet = s.GetAdet();
            satirData.en = s.GetEn();
            satirData.boy = s.GetBoy();
            satirData.yukseklik = 1f;
            satirData.hacim = s.GetHacim();

            data.satirlar.Add(satirData);
        }

        return JsonUtility.ToJson(data);
    }

    // --------------------------------------------------
    // JSON LOAD (þimdilik boþ)
    // --------------------------------------------------
    public void LoadFromJson(string json)
    {

        TumSatirlariTemizle();

        if (string.IsNullOrEmpty(json))
            return;

        BetonDetayData data = JsonUtility.FromJson<BetonDetayData>(json);

        foreach (var s in data.satirlar)
        {
            GameObject yeni = Instantiate(satirPrefab, listParent, false);
            yeni.transform.SetSiblingIndex(0);

            MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

            float alan = s.benzer * s.adet * s.en * s.boy;

            satir.SetupNumeric(
                s.metrajAdi,
                s.benzer,
                s.adet,
                s.en,
                s.boy,
                1f,
                alan,
                this
            );

            satirlar.Add(satir);
        }

        ToplamGuncelle();
    }

    // --------------------------------------------------
    // TÜM SATIRLARI TEMÝZLE
    // --------------------------------------------------
    public void TumSatirlariTemizle()
    {
        foreach (var s in satirlar)
            Destroy(s.gameObject);

        satirlar.Clear();
        otomatikIsimSayac = 1;

        ToplamGuncelle();
    }

    // --------------------------------------------------
    // METRAJ TÜRÜ
    // --------------------------------------------------
    public string GetMetrajTuru()
    {
        return "Kalýp";
    }

    // --------------------------------------------------
    // INPUT TEMÝZLE
    // --------------------------------------------------
    public void InputlariTemizle()
    {
        inputMetrajAdi.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputEn.text = "";
        inputBoy.text = "";
    }

    float ParseOrOne(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 1f;

        if (float.TryParse(value, NumberStyles.Any,
            CultureInfo.CurrentCulture, out float sonuc))
            return sonuc;

        if (float.TryParse(value.Replace(",", "."),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out sonuc))
            return sonuc;

        return 1f;
    }

    // --------------------------------------------------
    // ANÝMASYON
    // --------------------------------------------------
    IEnumerator SatirAnimasyon(GameObject yeniSatir)
    {
        // Layout zorla yerleþsin
        LayoutRebuilder.ForceRebuildLayoutImmediate(listParentAnim);

        yield return null; // 1 frame bekle

        if (listParentAnim.childCount == 0)
            yield break;

        yeniSatir.SetActive(true); // artýk görünür

        float satirYukseklik =
            ((RectTransform)listParentAnim.GetChild(0)).rect.height;

        Vector2 hedef = listParentAnim.anchoredPosition;
        Vector2 baslangic = hedef + Vector2.up * satirYukseklik;

        listParentAnim.anchoredPosition = baslangic;

        float t = 0f;

        while (t < animSure)
        {
            t += Time.deltaTime;

            float oran = t / animSure;
            oran = 1f - Mathf.Pow(1f - oran, 2f); // yumuþak bitiþ

            listParentAnim.anchoredPosition =
                Vector2.Lerp(baslangic, hedef, oran);

            yield return null;
        }

        listParentAnim.anchoredPosition = hedef;
    }
}