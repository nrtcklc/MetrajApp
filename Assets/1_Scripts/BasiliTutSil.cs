using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BasiliTutSil : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    Image doldurulacakDaire;

    bool butonbasilimi;

    float basilitutmaSuresi;
    float toplamBasilacakSure = 1f;

    MetrajSatir metrajSatir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }
    void Update()
    {
        if (butonbasilimi)
        {
            basilitutmaSuresi += Time.deltaTime;
            if (basilitutmaSuresi >= toplamBasilacakSure)
            {
                butonbasilimi = false;
                metrajSatir = GetComponentInParent<MetrajSatir>();
                metrajSatir.Sil();
            }
            doldurulacakDaire.fillAmount = basilitutmaSuresi / toplamBasilacakSure;
        }
        if (!butonbasilimi)
        {
            basilitutmaSuresi -= Time.deltaTime;
            if (basilitutmaSuresi <= 0)
            {
                basilitutmaSuresi = 0;
            }
            doldurulacakDaire.fillAmount = basilitutmaSuresi / toplamBasilacakSure;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        butonbasilimi = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        butonbasilimi = false;
    }
}
