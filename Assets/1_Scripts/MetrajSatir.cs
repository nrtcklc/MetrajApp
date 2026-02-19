using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MetrajSatir : MonoBehaviour
{
    public TMP_Text txtMetrajAdi;
    public TMP_Text txtHesapDeger;
    public TMP_Text txtSonuc;

    private float hacimDegeri;
    private string metrajAdiDegeri;
    private string hesapOzetDegeri;

    private BetonManager manager;

    public void Setup(string metrajAdi,
                      string hesapDeger,
                      float hacim,
                      BetonManager betonManager)
    {
        txtMetrajAdi.text = metrajAdi;
        txtHesapDeger.text = hesapDeger;
        txtSonuc.text = hacim.ToString("F2") + " m³";

        metrajAdiDegeri = metrajAdi;
        hesapOzetDegeri = hesapDeger;
        hacimDegeri = hacim;
        manager = betonManager;
    }

    public void Sil()
    {
        manager.SatirSil(this);
        Destroy(gameObject);
    }

    public float GetHacim()
    {
        return hacimDegeri;
    }

    public string GetMetrajAdi()
    {
        return metrajAdiDegeri;
    }

    public string GetHesapOzet()
    {
        return hesapOzetDegeri;
    }
}
