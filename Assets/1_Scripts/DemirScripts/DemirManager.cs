using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DemirManager : MonoBehaviour, IMetrajManager
{
    [Header("Input Alanları")]
    public TMP_InputField inputMetrajAdi;
    public TMP_InputField inputCap;
    public TMP_InputField inputBenzer;
    public TMP_InputField inputAdet;
    public TMP_InputField inputBoy;

    [Header("UI Referansları")]
    public Transform satirParent;
    public GameObject metrajSatirPrefab;
    public TMP_Text txtGenelToplam;

    [Header("CAP PANEL")]
    public GameObject capPanel;
    public Transform capGridParent;
    public GameObject capButtonPrefab;

    [Header("Ses (Opsiyonel)")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    private List<int> mevcutCaplar = new List<int>()
    { 8, 10, 12, 14, 16, 18, 20, 22, 25, 26, 28, 32, 34, 36, 38, 40 };

    private int varsayilanCap = 10;

    private Dictionary<Button, Color> orijinalRenkler = new Dictionary<Button, Color>();

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();

    private const float ozgulAgirlik = 7850f;
    private int otomatikIsimSayac = 1;

    private void Start()
    {
        CapButonlariniOlustur();
        capPanel.SetActive(false);

        Vurgula(varsayilanCap);
        inputCap.readOnly = true;
    }

    #region CAP PANEL

    public void PanelAc()
    {
        Vurgula(varsayilanCap);
        capPanel.SetActive(true);
    }

    public void PanelKapat()
    {
        inputCap.DeactivateInputField();
        capPanel.SetActive(false);
    }

    private void CapButonlariniOlustur()
    {
        // Önce eski butonları temizle
        foreach (Transform child in capGridParent)
            Destroy(child.gameObject);

        foreach (int cap in mevcutCaplar)
        {
            GameObject btnObj = Instantiate(capButtonPrefab, capGridParent);

            

            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            Button btn = btnObj.GetComponent<Button>();
            Image img = btnObj.GetComponent<Image>();

            orijinalRenkler.Add(btn, img.color);

            txt.text = cap.ToString();
            int secilen = cap;

            btn.onClick.AddListener(() =>
            {
                string deger = secilen.ToString();

                inputCap.SetTextWithoutNotify(deger);
                inputCap.text = deger;
                inputCap.ForceLabelUpdate();

                inputCap.caretPosition = deger.Length;

                Vurgula(secilen);
                SesCal();
                PanelKapat();
            });
        }
       
    }

    private void Vurgula(int secilenCap)
    {
        for (int i = 0; i < capGridParent.childCount; i++)
        {
            Transform child = capGridParent.GetChild(i);
            Button btn = child.GetComponent<Button>();
            Image img = child.GetComponent<Image>();

            int cap = mevcutCaplar[i];

            if (cap == secilenCap)
            {
                img.color = new Color(0.2f, 0.6f, 1f, 1f); // seçilen
            }
            else
            {
                img.color = orijinalRenkler[btn]; // prefab rengi
            }
        }
    }

    private void SesCal()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    #endregion

    #region INTERFACE (BOZULMADI)

    public void Hesapla()
    {
        float capMm = string.IsNullOrWhiteSpace(inputCap.text) ? 10f : GetFloatOrOne(inputCap.text);
        float benzer = GetFloatOrOne(inputBenzer.text);
        float adet = GetFloatOrOne(inputAdet.text);
        float boy = GetFloatOrOne(inputBoy.text);

        float capMetre = capMm / 1000f;
        float yaricap = capMetre / 2f;
        float hacim = Mathf.PI * yaricap * yaricap * boy;
        float toplamKg = hacim * ozgulAgirlik * adet * benzer;
        float ton = toplamKg / 1000f;

        string metrajAdi = inputMetrajAdi.text;

        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Demir_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        SatirEkle(metrajAdi, capMm, benzer, adet, boy, ton);

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

    public float GetToplamMetrajValue()
    {
        return GetGenelToplam();
    }

    public string GetMetrajTuru()
    {
        return "Demir";
    }

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
                cap = s.GetEn(),
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
            string ad = string.IsNullOrEmpty(d.metrajAdi)
                ? "Demir_" + otomatikIsimSayac++
                : d.metrajAdi;

            SatirEkle(ad, d.cap, d.benzer, d.adet, d.boy, d.hacim);
        }
    }

    #endregion

    #region PRIVATE

    private void SatirEkle(string ad, float cap, float benzer, float adet, float boy, float ton)
    {
        GameObject yeni = Instantiate(metrajSatirPrefab, satirParent);
        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

        satir.SetupNumeric(ad, cap, benzer, adet, boy, 1f, ton, this);
        satirlar.Add(satir);
    }

    private void GenelToplamGuncelle()
    {
        txtGenelToplam.text = GetGenelToplam().ToString("F2") + " ton";
    }

    public void InputTemizle()
    {
        inputMetrajAdi.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputBoy.text = "";

        inputCap.SetTextWithoutNotify("");
        TemizVurgu();
    }
    private void TemizVurgu()
    {
        for (int i = 0; i < capGridParent.childCount; i++)
        {
            Transform child = capGridParent.GetChild(i);
            Button btn = child.GetComponent<Button>();
            Image img = child.GetComponent<Image>();

            if (orijinalRenkler.ContainsKey(btn))
                img.color = orijinalRenkler[btn]; // prefab rengi
        }
    }

    private float GetFloatOrOne(string val)
    {
        if (string.IsNullOrWhiteSpace(val))
            return 1f;

        val = val.Trim();

        if (float.TryParse(val, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.CurrentCulture, out float sonuc))
            return sonuc;

        if (float.TryParse(val.Replace(",", "."),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out sonuc))
            return sonuc;

        return 1f;
    }

    #endregion
}