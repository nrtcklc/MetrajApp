using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    [Header("Animasyon")]
    public RectTransform listParentAnim; // satirParent'ın RectTransform'u
    public float animSure = 0.2f;


    private List<int> mevcutCaplar = new List<int>()
    { 8, 10, 12, 14, 16, 18, 20, 22, 24, 25, 26, 28, 30, 32, 34, 36};

    private int varsayilanCap = 10;

    private Dictionary<Button, Color> orijinalRenkler = new Dictionary<Button, Color>();

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();

    private const float ozgulAgirlik = 7850f;
    private int otomatikIsimSayac = 1;

    private void Start()
    {
        CapButonlariniOlustur();
        capPanel.SetActive(false);

        inputCap.readOnly = true;
        inputCap.onSelect.AddListener(KlavyeKapat);
    }

    #region CAP PANEL

    void KlavyeKapat(string value)
    {
        inputCap.DeactivateInputField();
        EventSystem.current.SetSelectedGameObject(null);
    }

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
        float Kg = hacim * ozgulAgirlik * adet * benzer;

        string metrajAdi = inputMetrajAdi.text;

        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Demir_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        SatirEkle(metrajAdi, capMm, benzer, adet, boy, Kg);

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

    private void SatirEkle(string ad, float cap, float benzer, float adet, float boy, float kg)
    {
        GameObject yeni = Instantiate(metrajSatirPrefab, satirParent);
        yeni.transform.SetSiblingIndex(0);

        yeni.SetActive(false); // 👈 ilk frame gizle

        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();
        satir.SetupNumeric(ad, cap, benzer, adet, boy, 1f, kg, this);

        satirlar.Add(satir);

        StartCoroutine(SatirAnimasyon(yeni));
    }
    IEnumerator SatirAnimasyon(GameObject yeniSatir)
    {
        // Layout yerleşsin
        LayoutRebuilder.ForceRebuildLayoutImmediate(listParentAnim);

        yield return null; // 1 frame bekle

        yeniSatir.SetActive(true); // 👈 artık görünür

        float satirYukseklik =
            ((RectTransform)satirParent.GetChild(0)).rect.height;

        Vector2 hedef = listParentAnim.anchoredPosition;
        Vector2 baslangic = hedef + Vector2.up * satirYukseklik;

        listParentAnim.anchoredPosition = baslangic;

        float t = 0f;

        while (t < animSure)
        {
            t += Time.deltaTime;
            float oran = t / animSure;

            oran = 1f - Mathf.Pow(1f - oran, 2f); // hafif ease-out

            listParentAnim.anchoredPosition =
                Vector2.Lerp(baslangic, hedef, oran);

            yield return null;
        }

        listParentAnim.anchoredPosition = hedef;
    }

    private void GenelToplamGuncelle()
    {
        txtGenelToplam.text = "Toplam Metraj: " + GetGenelToplam().ToString("F2") + " Kg";
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