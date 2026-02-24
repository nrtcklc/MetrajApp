using UnityEngine;
using TMPro;


public class MetrajKayitSatirUI : MonoBehaviour
{
    public TMP_Text txtKayitAdi;
    public TMP_Text txtTur;
    public TMP_Text txtTarih;
    public TMP_Text txtToplam;

    private string kayitId;
    private string detayJson;
    private string kayitTuru;

    private MetrajKayitManager kayitManager;

    public GameObject cember;

    [SerializeField] private AnaMenuManager anaMenuManager;

    public BetonManager betonManager;
    public KalipManager kalipManager;
    public DemirManager demirManager;

    public void Setup(MetrajKayitData data,
                      MetrajKayitManager manager)
    {
        kayitId = data.id;
        detayJson = data.detayJson;
        kayitTuru = data.kayitTuru;

        txtKayitAdi.text = data.kayitAdi;
        txtTur.text = data.kayitTuru;
        txtTarih.text = data.kayitTarihi;

        // Türüne göre birim
        string birim = "";

        if (kayitTuru == "Beton") birim = " m³";
        if (kayitTuru == "Kalýp") birim = " m²";
        if (kayitTuru == "Demir") birim = " Kg";

        txtToplam.text = data.toplamMetraj.ToString("F2") + birim;

        kayitManager = manager;
    }

    public void Duzenle()
    {
        if (anaMenuManager == null)
            anaMenuManager = FindObjectOfType<AnaMenuManager>();

        if (kayitTuru == "Beton")
        {
            anaMenuManager.PanelAc(2);

            if (betonManager == null)
                betonManager = FindObjectOfType<BetonManager>(true);

            betonManager.LoadFromJson(detayJson);
        }
        else if (kayitTuru == "Kalýp")
        {
            anaMenuManager.PanelAc(3);

            if (kalipManager == null)
                kalipManager = FindObjectOfType<KalipManager>(true);

            kalipManager.LoadFromJson(detayJson);
        }
        else if (kayitTuru == "Demir")
        {
            anaMenuManager.PanelAc(4);

            if (demirManager == null)
                demirManager = FindObjectOfType<DemirManager>(true);

            demirManager.LoadFromJson(detayJson);
        }
    }


    public void Sil()
    {
        kayitManager.KayitSil(kayitId);
        Destroy(gameObject);
    }


}