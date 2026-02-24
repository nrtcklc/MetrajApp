using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MetrajBasiliTut : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image doldurulacakDaire;

    bool basiliMi = false;

    float sure = 0f;
    float hedefSure = 1f;

    MetrajKayitSatirUI satirUI;

    void Awake()
    {
        sure = 0f;
        basiliMi = false;

        doldurulacakDaire.fillAmount = 0f;
        doldurulacakDaire.gameObject.SetActive(false);
    }

    void Update()
    {
        if (basiliMi)
        {
            if (!doldurulacakDaire.gameObject.activeSelf)
                doldurulacakDaire.gameObject.SetActive(true);

            sure += Time.deltaTime;
        }
        else
        {
            if (sure > 0f)
                sure -= Time.deltaTime;
        }

        sure = Mathf.Clamp(sure, 0f, hedefSure);

        doldurulacakDaire.fillAmount = sure / hedefSure;

        if (sure <= 0f && !basiliMi)
        {
            doldurulacakDaire.gameObject.SetActive(false);
        }

        if (sure >= hedefSure)
        {
            sure = 0f;
            basiliMi = false;
            doldurulacakDaire.fillAmount = 0f;
            doldurulacakDaire.gameObject.SetActive(false);

            if (satirUI == null)
                satirUI = GetComponentInParent<MetrajKayitSatirUI>();

            satirUI.Sil();
        }
        Debug.Log("Update çalýþýyor");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        basiliMi = true;
        Debug.Log("Basildi");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        basiliMi = false;
    }
}