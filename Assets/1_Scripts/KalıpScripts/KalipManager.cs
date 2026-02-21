using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KalipManager : MonoBehaviour
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

    private List<MetrajSatir> satirlar = new List<MetrajSatir>();
    private int otomatikIsimSayac = 1;

    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform listParentAnim;
    [SerializeField] float animSure = 0.25f;

    public void YeniMetrajEkle()
    {
        string metrajAdi = inputMetrajAdi.text;

        if (string.IsNullOrEmpty(metrajAdi))
        {
            metrajAdi = "Kalýp_" + otomatikIsimSayac;
            otomatikIsimSayac++;
        }

        float benzer = GetFloatOrOne(inputBenzer.text);
        float adet = GetFloatOrOne(inputAdet.text);
        float en = GetFloatOrOne(inputEn.text);
        float boy = GetFloatOrOne(inputBoy.text);

        float alan = benzer * adet * en * boy;

        string hesapOzet =
            benzer + " x " +
            adet + " x " +
            en + " x " +
            boy;

        GameObject yeni = Instantiate(satirPrefab, listParent, false);
        yeni.transform.SetSiblingIndex(0);

        MetrajSatir satir = yeni.GetComponent<MetrajSatir>();

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
        StartCoroutine(ContentAsagiKay());

        satir.SetupNumeric(
            metrajAdi,
            benzer,
            adet,
            en,
            boy,
            1f,      // yukseklik yerine 1 gönderiyoruz
            alan,
            null     // Kalýp için manager referansý gerekmez
        );

        satirlar.Add(satir);

        ToplamGuncelle();
        InputlariTemizle();
    }

    float GetFloatOrOne(string deger)
    {
        if (string.IsNullOrWhiteSpace(deger))
            return 1f;

        if (float.TryParse(deger, NumberStyles.Any,
            CultureInfo.CurrentCulture, out float sonuc))
            return sonuc;

        if (float.TryParse(deger.Replace(",", "."),
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out sonuc))
            return sonuc;

        return 1f;
    }

    void ToplamGuncelle()
    {
        float toplam = 0f;

        foreach (var s in satirlar)
            toplam += s.GetHacim(); // burada hacim alan olarak kullanýlýyor

        txtToplamKalip.text = "Toplam Kalýp: " + toplam.ToString("F2") + " m²";
    }

    public float GetToplamMetrajValue()
    {
        float toplam = 0f;

        foreach (var s in satirlar)
            toplam += s.GetHacim();

        return toplam;
    }

    public string GetKalipDetayJson()
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

    IEnumerator ContentAsagiKay()
    {
        yield return null;

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

    public void InputlariTemizle()
    {
        inputMetrajAdi.text = "";
        inputBenzer.text = "";
        inputAdet.text = "";
        inputEn.text = "";
        inputBoy.text = "";
    }
}