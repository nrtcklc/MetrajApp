using UnityEngine;
using TMPro;
using System.IO;
using System.Text;

public class MetrajKayitSatirUI : MonoBehaviour
{
    public TMP_Text txtKayitAdi;
    public TMP_Text txtTur;
    public TMP_Text txtTarih;
    public TMP_Text txtToplam;

    private string kayitId;
    private string detayJson;

    private MetrajKayitManager kayitManager;
    private BetonManager betonManager;

    MetrajKayitData mevcutKayit;
    MetrajKayitManager manager;

    public void Setup(MetrajKayitData data,
                  MetrajKayitManager manager,
                  BetonManager beton)
    {
        kayitId = data.id;
        detayJson = data.detayJson;

        txtKayitAdi.text = data.kayitAdi;
        txtTur.text = data.kayitTuru;
        txtTarih.text = data.kayitTarihi;
        txtToplam.text = data.toplamMetraj.ToString("F2") + " m³";

        kayitManager = manager;
        betonManager = beton;

    kayitId = data.id;
    detayJson = data.detayJson;

    txtKayitAdi.text = data.kayitAdi;
    txtTur.text = data.kayitTuru;
    txtTarih.text = data.kayitTarihi;
    txtToplam.text = data.toplamMetraj.ToString("F2") + " m³";

    kayitManager = manager;
    betonManager = beton;

    mevcutKayit = data; 
    }

    // -------------------
    // DÜZENLE BUTONU
    // -------------------
    public void Duzenle()
    {
        betonManager.LoadFromBetonJson(detayJson);

        // burada beton sayfasýný açtýrýyorsan onu da çaðýrabilirsin
    }

    // -------------------
    // SÝL BUTONU
    // -------------------
    public void Sil()
    {
        kayitManager.KayitSil(kayitId);
        Destroy(gameObject);
    }
}
