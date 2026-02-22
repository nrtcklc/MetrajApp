using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BetonManager : MonoBehaviour, IMetrajManager
{
    [Header("Metraj Bilgileri")]
    public TMP_InputField inputMetrajAdi;

    [Header("Çarpanlar")]
    public TMP_InputField inputBenzer;
    public TMP_InputField inputAdet;
    public TMP_InputField inputEn;
    public TMP_InputField inputBoy;
    public TMP_InputField inputYukseklik;

    [Header("Toplam")]
    public TMP_Text txtToplamBeton;

    [Header("Liste")]
    public GameObject satirPrefab;
    public Transform listParent;

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();

    private int otomatikIsimSayac = 1;

    [SerializeField] ScrollRect scrollRect;

    [SerializeField] RectTransform listParentAnim;
    [SerializeField] float animSure = 0.25f;
    public void YeniMetrajEkle()
    {
        // --- METRAJ ADI ---
        string metrajAdi = inputMetrajAdi.text;

        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Metraj_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        // --- ÇARPANLAR (BOÞSA 1) ---
        float benzer = GetFloatOrOne(inputBenzer.text);
        float adet = GetFloatOrOne(inputAdet.text);
        float en = GetFloatOrOne(inputEn.text);
        float boy = GetFloatOrOne(inputBoy.text);
        float yukseklik = GetFloatOrOne(inputYukseklik.text);

        float hacim = benzer * adet * en * boy * yukseklik;

        string hesapOzet =
            benzer + " x " +
            adet + " x " +
            en + " x " +
            boy + " x " +
            yukseklik;

        GameObject yeni = Instantiate(satirPrefab, listParent, false);
        yeni.transform.SetSiblingIndex(0);

        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
        StartCoroutine(ContentAsagiKay());

        satir.Setup(metrajAdi, hesapOzet, hacim, this);
        satirlar.Add(satir);

        ToplamGuncelle();
        InputlariTemizle();
    }
    float GetFloatOrOne(string deger)
    {
        if (string.IsNullOrWhiteSpace(deger))
            return 1f;

        deger = deger.Trim();

        // 1 Önce cihazýn culture'ý ile dene (Türkçe cihazda 3,5 düzgün parse olur)
        if (float.TryParse(deger,
            NumberStyles.Any,
            CultureInfo.CurrentCulture,
            out float sonuc))
            return sonuc;

        // 2 Olmazsa evrensel format ile dene (3.5 gibi)
        if (float.TryParse(deger,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out sonuc))
            return sonuc;

        // 3 Son çare: virgül noktaya çevirip tekrar dene
        if (float.TryParse(deger.Replace(",", "."),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out sonuc))
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

        txtToplamBeton.text = "Toplam Metraj: " + toplam.ToString("F2") + " m³";
        StartCoroutine(ToplamPulse());
    }
    IEnumerator ToplamPulse()
    {
        float sure = 0.1f;
        float gecen = 0f;

        Vector3 normal = Vector3.one;
        Vector3 buyuk = Vector3.one * 1.05f;

        // büyü
        while (gecen < sure)
        {
            gecen += Time.deltaTime;
            float oran = gecen / sure;
            txtToplamBeton.transform.localScale =
                Vector3.Lerp(normal, buyuk, oran);
            yield return null;
        }

        gecen = 0f;

        // küçül
        while (gecen < sure)
        {
            gecen += Time.deltaTime;
            float oran = gecen / sure;
            txtToplamBeton.transform.localScale =
                Vector3.Lerp(buyuk, normal, oran);
            yield return null;
        }

        txtToplamBeton.transform.localScale = normal;
    }
    public void InputlariTemizle()
    {
        inputMetrajAdi.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputEn.text = "";
        inputBoy.text = "";
        inputYukseklik.text = "";
    }
    IEnumerator ContentAsagiKay()
    {
        yield return null; // Layout yerleþsin

        float satirYukseklik = ((RectTransform)listParentAnim.GetChild(0)).rect.height;

        Vector2 baslangic = listParentAnim.anchoredPosition + Vector2.up * satirYukseklik;
        Vector2 hedef = listParentAnim.anchoredPosition;

        listParentAnim.anchoredPosition = baslangic;

        float t = 0f;

        while (t < animSure)
        {
            t += Time.deltaTime;
            listParentAnim.anchoredPosition =
                Vector2.Lerp(baslangic, hedef, t / animSure);

            yield return null;
        }

        listParentAnim.anchoredPosition = hedef;
    }
    // --------------------------------------------------
    // KAYIT SÝSTEMÝ ÝÇÝN EKLENEN FONKSÝYONLAR
    // --------------------------------------------------

    public float GetToplamMetrajValue()
    {
        float toplam = 0f;

        foreach (var s in satirlar)
            toplam += s.GetHacim();

        return toplam;
    }

    public string GetBetonDetayJson()
    {
        BetonDetayData data = new BetonDetayData();

        foreach (var s in satirlar)
        {
            BetonSatirData satirData = new BetonSatirData();

            satirData.metrajAdi = s.GetMetrajAdi();

            // Yeni sistem
            satirData.benzer = s.GetBenzer();
            satirData.adet = s.GetAdet();
            satirData.en = s.GetEn();
            satirData.boy = s.GetBoy();
            satirData.yukseklik = s.GetYukseklik();

            // Eski sistem (þimdilik kalsýn)
            satirData.hesapOzet = s.GetHesapOzet();

            satirData.hacim = s.GetHacim();

            data.satirlar.Add(satirData);
        }

        return JsonUtility.ToJson(data);
    }

    public void LoadFromBetonJson(string json)
    {
        // Önce mevcut satýrlarý temizle
        foreach (var s in satirlar)
            Destroy(s.gameObject);

        satirlar.Clear();

        BetonDetayData data = JsonUtility.FromJson<BetonDetayData>(json);

        if (data == null || data.satirlar == null)
            return;

        foreach (var d in data.satirlar)
        {
            GameObject yeni = Instantiate(satirPrefab, listParent, false);
            MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

            // ARTIK STRING SETUP YOK
            satir.SetupNumeric(
                d.metrajAdi,
                d.benzer,
                d.adet,
                d.en,
                d.boy,
                d.yukseklik,
                d.hacim,
                this
            );

            satirlar.Add(satir);
        }

        //Sayaç güncelle
        otomatikIsimSayac = satirlar.Count + 1;

        ToplamGuncelle();

    }

    public void TumSatirlariTemizle()
    {
        foreach (var s in satirlar)
            Destroy(s.gameObject);

        satirlar.Clear();

        // Sayaç reset
        otomatikIsimSayac = 1;

        ToplamGuncelle();
        Debug.Log("Tüm satýrlar temizlendi.");
    }

    void OnEnable()
    {
        if (!string.IsNullOrEmpty(MetrajKayitManager.duzenlemeJson))
        {
            string json = MetrajKayitManager.duzenlemeJson;
            MetrajKayitManager.duzenlemeJson = null;

            LoadFromBetonJson(json);
        }
        else
        {
            // Boþ deðil, ama kullanýcýya "hiç metraj yok" mesajý gösterebilirsin
            // veya default bir satýr ekleyebilirsin
            InputlariTemizle();
        }
    }
    public string GetDetayJson()
    {
        return GetBetonDetayJson();
    }

    public void LoadFromJson(string json)
    {
        LoadFromBetonJson(json);
    }
    public string GetMetrajTuru()
    {
        return "Beton";
    }
}
