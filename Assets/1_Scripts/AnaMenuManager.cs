using UnityEngine;
using System.Collections;

public class AnaMenuManager : MonoBehaviour
{
    public IMetrajManager aktifManager;
    ///ana Menü Slider'ı için fonksiyon

    public RectTransform obje1;
    public RectTransform obje2;
    public RectTransform obje3;
    public RectTransform obje4;
    public RectTransform obje5;
    public RectTransform obje6;
    public RectTransform obje7;

    private int aktifPanelIndex;

    void Start()
    {
        StartCoroutine(Animasyon());
    }


    [SerializeField] private GameObject[] paneller;

    public void PanelAc(int panelIndex)
    {
        for (int i = 0; i < paneller.Length; i++)
            paneller[i].SetActive(i == panelIndex);

        aktifPanelIndex = panelIndex;
    }

    public int GetAktifPanelIndex()
    {
        return aktifPanelIndex;
    }


    IEnumerator Animasyon()
    {
        float zaman = 0f;

        // SCALE başlangıç
        obje1.localScale = Vector3.zero;
        obje3.localScale = Vector3.zero;
        obje4.localScale = Vector3.zero;
        obje5.localScale = Vector3.zero;
        obje6.localScale = Vector3.zero;

        obje2.gameObject.SetActive(false);

        // OBJE2 Pozisyon
        Vector2 obje2Bas = new Vector2(-550f, obje2.anchoredPosition.y);
        Vector2 obje2Hedef = new Vector2(0f, obje2.anchoredPosition.y);
        obje2.anchoredPosition = obje2Bas;

        // OBJE7 Pozisyon
        Vector2 obje7Bas = new Vector2(obje7.anchoredPosition.x, -300f);
        Vector2 obje7Hedef = new Vector2(obje7.anchoredPosition.x, 0f);
        obje7.anchoredPosition = obje7Bas;

        while (zaman < 2.5f)
        {
            zaman += Time.deltaTime;

            // OBJE1 (0 - 0.5)
            if (zaman <= 0.5f)
                Overshoot(obje1, zaman, 0f, 0.5f);

            //OBJE2 (0.5 - 1.5)
            if (zaman >= 0.5f)
            {
                obje2.gameObject.SetActive(true);

                float t = Mathf.Clamp01((zaman - 0.5f) / 1f);
                t = 1 - Mathf.Pow(1 - t, 3); // EaseOutCubic
                obje2.anchoredPosition = Vector2.Lerp(obje2Bas, obje2Hedef, t);
            }

            // OBJE3 (0.25 - 1.25)
            if (zaman >= 0.25f)
                Overshoot(obje3, zaman, 0.25f, 1f);

            // OBJE4 (0.5 - 1.5)
            if (zaman >= 0.5f)
                Overshoot(obje4, zaman, 0.5f, 1f);

            //OBJE5 (0.75 - 1.75)
            if (zaman >= 0.75f)
                Overshoot(obje5, zaman, 0.75f, 1f);

            // OBJE6 (1 - 2)
            if (zaman >= 1f)
                Overshoot(obje6, zaman, 1f, 1f);

            // OBJE7 (1 - 1.5)
            if (zaman >= 1f)
            {
                float t = Mathf.Clamp01((zaman - 1f) / 0.5f);
                t = 1 - Mathf.Pow(1 - t, 3); // EaseOutCubic
                obje7.anchoredPosition = Vector2.Lerp(obje7Bas, obje7Hedef, t);
            }

            yield return null;
        }
    }
    void Overshoot(RectTransform obje, float zaman, float baslangic, float sure)
    {
        float t = Mathf.Clamp01((zaman - baslangic) / sure);

        // 0 → 1.15 → 1 gerçek overshoot
        if (t < 0.8f)
        {
            float t1 = t / 0.8f;
            obje.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.15f, t1);
        }
        else
        {
            float t2 = (t - 0.8f) / 0.2f;
            obje.localScale = Vector3.Lerp(Vector3.one * 1.15f, Vector3.one, t2);
        }
    }

}