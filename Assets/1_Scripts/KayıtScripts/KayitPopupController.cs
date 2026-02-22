using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

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

    private bool otomatikAnaMenu = false; // Coroutine flag

    private void OnEnable()
    {
        // Popup açýldýðýnda EventSystem üzerinden tüm butonlarý dinleyeceðiz
        EventTrigger trigger = popupPanel.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = popupPanel.AddComponent<EventTrigger>();

        // Her buton basýmýnda flag'i false yap
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener((data) => { KullaniciBaskaTusaBasti(); });
        trigger.triggers.Add(entry);
    }

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

        otomatikAnaMenu = true; // Coroutine için flag aktif
        StartCoroutine(KayitSonrasiKapat());
    }

    IEnumerator KayitSonrasiKapat()
    {
        yield return new WaitForSeconds(2f);

        // Popup kapat
        popupPanel.SetActive(false);

        // Kullanýcý 2 saniye içinde baþka bir tuþa basmadýysa ana menüye dön
        if (otomatikAnaMenu)
        {
            anaMenuManager.PanelAc(0);
        }

        // UI resetle
        inputKayitAdi.text = "";
        inputKayitAdi.gameObject.SetActive(true);
        btnKaydet.SetActive(true);
        btnIptal.SetActive(true);
        txtMesaj.gameObject.SetActive(false);

        otomatikAnaMenu = false; // Reset flag
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
        otomatikAnaMenu = false; // iptal edildiðinde flag'i kapat
    }

    public void KullaniciBaskaTusaBasti()
    {
        // Eðer popup açýksa, kullanýcý baþka tuþa basarsa otomatik ana menüyü iptal et
        otomatikAnaMenu = false;
    }
}