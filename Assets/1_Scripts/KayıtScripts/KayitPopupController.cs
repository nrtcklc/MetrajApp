using UnityEngine;
using TMPro;

public class KayitPopupController : MonoBehaviour
{
    public TMP_InputField inputKayitAdi;

    public BetonManager betonManager;
    public MetrajKayitManager kayitManager;

    public GameObject popupPanel;

    public void KaydetButon()
    {
        string ad = inputKayitAdi.text;

        if (string.IsNullOrEmpty(ad))
            ad = "kayit_" + Random.Range(1, 9999);

        float toplam = betonManager.GetToplamMetrajValue();
        string detay = betonManager.GetBetonDetayJson();

        kayitManager.YeniKayitEkle(ad, "Beton", toplam, detay);

        betonManager.TumSatirlariTemizle();

        inputKayitAdi.text = "";
        popupPanel.SetActive(false);
    }

    public void Iptal()
    {
        popupPanel.SetActive(false);
    }
}
