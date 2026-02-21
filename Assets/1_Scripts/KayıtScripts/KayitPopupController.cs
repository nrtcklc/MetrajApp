using UnityEngine;
using TMPro;
using System.Collections;

public class KayitPopupController : MonoBehaviour
{
    public TMP_InputField inputKayitAdi;
    public GameObject btnKaydet;
    public GameObject btnIptal;

    public TMP_Text txtMesaj; // Hazýrladýðýn mesaj texti

    public MetrajKayitManager kayitManager;
    public GameObject popupPanel;
    public AnaMenuManager anaMenuManager;

    public BetonManager betonManager;
    public KalipManager kalipManager;
    public DemirManager demirManager;

    public void KaydetButon()
    {
        IMetrajManager aktifManager = GetAktifManager();

        if (aktifManager == null)
        {
            Debug.LogError("Aktif manager bulunamadý!");
            return;
        }

        string ad = inputKayitAdi.text;

        if (string.IsNullOrEmpty(ad))
            ad = "kayit_" + Random.Range(1, 9999);

        float toplam = aktifManager.GetToplamMetrajValue();
        string detay = aktifManager.GetDetayJson();
        string tur = aktifManager.GetMetrajTuru();

        kayitManager.YeniKayitEkle(ad, tur, toplam, detay);

        aktifManager.TumSatirlariTemizle();

        inputKayitAdi.gameObject.SetActive(false);
        btnKaydet.SetActive(false);
        btnIptal.SetActive(false);

        txtMesaj.text = "Metraj \"" + ad + "\" ismiyle kaydedildi.";
        txtMesaj.gameObject.SetActive(true);

        StartCoroutine(KayitSonrasiKapat());
    }

    IEnumerator KayitSonrasiKapat()
    {
        yield return new WaitForSeconds(2f);

        // Popup kapat
        popupPanel.SetActive(false);

        // Ana menüye dön
        anaMenuManager.PanelAc(0);

        // UI resetle (bir sonraki açýlýþ için)
        inputKayitAdi.text = "";
        inputKayitAdi.gameObject.SetActive(true);
        btnKaydet.SetActive(true);
        btnIptal.SetActive(true);
        txtMesaj.gameObject.SetActive(false);
    }
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

    public void Iptal()
    {
        popupPanel.SetActive(false);
    }
}