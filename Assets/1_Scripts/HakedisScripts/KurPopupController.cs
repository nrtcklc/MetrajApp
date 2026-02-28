using UnityEngine;
using TMPro;

public class KurPopupController : MonoBehaviour
{
    public HakedisManager manager;

    [Header("UI")]
    public GameObject panel;
    public TMP_Text varsayilanParaText;
    public TMP_InputField kurInput;

    void Start()
    {
        panel.SetActive(false);
    }

    public void PopupAc()
    {
        varsayilanParaText.text =
            "Varsayýlan Para Birimi: " +
            manager.aktif.varsayilanParaBirimi;

        kurInput.text = "";
        panel.SetActive(true);
    }

    public void Tamam()
    {
        double kur = 0;
        double.TryParse(kurInput.text, out kur);

        if (kur <= 0)
            return;

        manager.KurGuncelle(kur);

        panel.SetActive(false);
    }

    public void Kapat()
    {
        panel.SetActive(false);
    }
}