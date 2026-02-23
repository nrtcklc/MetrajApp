using UnityEngine;
using TMPro;
using System.Collections;

public class KayitPopupController : MonoBehaviour
{
    public TMP_InputField inputKayitAdi;
    public GameObject btnKaydet;
    public GameObject btnIptal;
    public TMP_Text txtMesaj;

    public MetrajKayitManager kayitManager;
    public GameObject popupPanel;
    public AnaMenuManager anaMenuManager;

    public BetonManager betonManager;
    public KalipManager kalipManager;
    public DemirManager demirManager;

    private Coroutine kapanmaCoroutine;

    // ==============================
    // KAYDET
    // ==============================
    public void KaydetButon()
    {
        IMetrajManager aktifManager = GetAktifManager();
        if (aktifManager == null)
        {
            Debug.LogError("Aktif manager bulunamadý!");
            return;
        }

        string ad = string.IsNullOrEmpty(inputKayitAdi.text)
            ? "kayit_" + Random.Range(1, 9999)
            : inputKayitAdi.text;

        float toplam = aktifManager.GetToplamMetrajValue();
        string detay = aktifManager.GetDetayJson();
        string tur = aktifManager.GetMetrajTuru();

        kayitManager.YeniKayitEkle(ad, tur, toplam, detay);
        aktifManager.TumSatirlariTemizle();

        // UI gizle
        inputKayitAdi.gameObject.SetActive(false);
        btnKaydet.SetActive(false);
        btnIptal.SetActive(false);

        txtMesaj.text = "Metraj \"" + ad + "\" ismiyle kaydedildi.";
        txtMesaj.gameObject.SetActive(true);

        // Coroutine baþlat
        kapanmaCoroutine = StartCoroutine(KayitSonrasiKapat());
    }

    // ==============================
    // 2 SN SONRA KAPAT
    // ==============================
    IEnumerator KayitSonrasiKapat()
    {
        yield return new WaitForSeconds(2f);

        popupPanel.SetActive(false);
        anaMenuManager.PanelAc(0);

        UIReset();
    }

    // ==============================
    // DIÞARIDAN ÝPTAL
    // ==============================
    public void OtomatikKapanmayiIptalEt()
    {
        if (kapanmaCoroutine != null)
        {
            StopCoroutine(kapanmaCoroutine);
            kapanmaCoroutine = null;
        }
    }

    // ==============================
    // ÝPTAL BUTONU
    // ==============================
    public void Iptal()
    {
        OtomatikKapanmayiIptalEt();
        popupPanel.SetActive(false);
        UIReset();
    }

    // ==============================
    // MANAGER BUL
    // ==============================
    private IMetrajManager GetAktifManager()
    {
        int aktifIndex = anaMenuManager.GetAktifPanelIndex();

        switch (aktifIndex)
        {
            case 2: return betonManager;
            case 3: return kalipManager;
            case 4: return demirManager;
            default: return null;
        }
    }

    // ==============================
    // UI RESET
    // ==============================
    private void UIReset()
    {
        inputKayitAdi.text = "";
        inputKayitAdi.gameObject.SetActive(true);
        btnKaydet.SetActive(true);
        btnIptal.SetActive(true);
        txtMesaj.gameObject.SetActive(false);
    }
}