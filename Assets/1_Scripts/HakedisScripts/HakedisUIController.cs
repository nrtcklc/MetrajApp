using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HakedisUIController : MonoBehaviour
{
    [Header("Manager")]
    public HakedisManager manager;

    [Header("Varsayýlan Para Birimi")]
    public TMP_Dropdown varsayilanParaDropdown;

    [Header("Hakediþ Bilgileri")]
    public TMP_InputField anaFirmaInput;
    public TMP_InputField altYukleniciInput;
    public TMP_InputField hakedisAdiInput;
    public TMP_InputField hakedisNoInput;
    public TMP_InputField donemInput;
    public TMP_InputField tarihInput;

    [Header("Önceki Metraj Seç")]
    public TMP_Dropdown oncekiMetrajDropdown;
    public List<MetrajKayitData> kayitliMetrajlar;
    public TMP_InputField birimFiyatMetraInput;
    public TMP_Dropdown paraBirimiMetrajDropdown;

    [Header("Manuel Ýþ Kalemi")]
    public TMP_InputField isKalemiAdiInput;
    public TMP_InputField miktarInput;
    public TMP_Dropdown birimDropdown;
    public TMP_InputField birimFiyatInput;
    public TMP_Dropdown paraBirimiDropdown;

    [Header("Önceki Hakediþ Kesinti")]
    public TMP_Dropdown oncekiHakedisDropdown;
    public List<HakedisData> kayitliHakedisler;

    [Header("Kesintiler")]
    public TMP_InputField avansInput;
    public TMP_Dropdown avansParaDropdown;

    public TMP_InputField isgInput;
    public TMP_Dropdown isgParaDropdown;

    public TMP_InputField malzemeInput;
    public TMP_Dropdown malzemeParaDropdown;

    public TMP_InputField yevmiyeInput;
    public TMP_Dropdown yevmiyeParaDropdown;

    public TMP_InputField digerInput;
    public TMP_Dropdown digerParaDropdown;

    public TMP_InputField teminatInput;
    public TMP_Dropdown teminatParaDropdown;

    public TMP_InputField kdvInput;

    [Header("Sonuç")]
    public TMP_Text sonucText;

    [Header("Kur Popup")]
    public KurPopupController kurPopup;

    // ===================================================
    // VARSAYILAN PARA BÝRÝMÝ
    // ===================================================
    public void VarsayilanParaDegisti()
    {
        string pb = varsayilanParaDropdown
            .options[varsayilanParaDropdown.value].text;

        manager.VarsayilanParaBirimiAyarla(pb);

        Guncelle();
    }

    // ===================================================
    // FÝRMA
    // ===================================================
    public void FirmaGuncelle()
    {
        int no = 0;
        int.TryParse(hakedisNoInput.text, out no);

        manager.FirmaGuncelle(
            anaFirmaInput.text,
            altYukleniciInput.text,
            hakedisAdiInput.text,
            no,
            donemInput.text,
            tarihInput.text
        );

        Guncelle();
    }

    public void HakedisBilgileriniGuncelle()
    {
        int no = 0;
        int.TryParse(hakedisNoInput.text, out no);

        manager.FirmaGuncelle(
            anaFirmaInput.text,
            altYukleniciInput.text,
            hakedisAdiInput.text,
            no,
            donemInput.text,
            tarihInput.text
        );
    }

    // ===================================================
    // METRAJ
    // ===================================================
    string BirimGetir(string kayitTuru)
    {
        switch (kayitTuru)
        {
            case "Beton": return "m³";
            case "Kalýp": return "m²";
            case "Demir": return "kg";
            default: return "";
        }
    }

    public void OncekiMetrajdanEkle()
    {
        if (kayitliMetrajlar == null || kayitliMetrajlar.Count == 0)
            return;

        int index = oncekiMetrajDropdown.value;
        if (index >= kayitliMetrajlar.Count)
            return;

        MetrajKayitData secilen = kayitliMetrajlar[index];

        double fiyat = 0;
        double.TryParse(birimFiyatMetraInput.text, out fiyat);
        if (fiyat <= 0) return;

        string birim = BirimGetir(secilen.kayitTuru);

        manager.MetrajEkle(
            secilen.kayitAdi,
            secilen.toplamMetraj,
            birim,
            fiyat,
            paraBirimiMetrajDropdown.options[
                paraBirimiMetrajDropdown.value].text
        );

        birimFiyatMetraInput.text = "";

        string secilenPB =
    paraBirimiMetrajDropdown.options[
        paraBirimiMetrajDropdown.value].text;

        if (secilenPB != manager.aktif.varsayilanParaBirimi)
        {
            kurPopup.PopupAc();
        }
        Guncelle();
    }

    public void ManuelEkle()
    {
        double miktar = 0;
        double fiyat = 0;

        double.TryParse(miktarInput.text, out miktar);
        double.TryParse(birimFiyatInput.text, out fiyat);

        manager.MetrajEkle(
            isKalemiAdiInput.text,
            miktar,
            birimDropdown.options[birimDropdown.value].text,
            fiyat,
            paraBirimiDropdown.options[paraBirimiDropdown.value].text
        );

        isKalemiAdiInput.text = "";
        miktarInput.text = "";
        birimFiyatInput.text = "";

        string secilenPB =
        paraBirimiDropdown.options[paraBirimiDropdown.value].text;

        if (secilenPB != manager.aktif.varsayilanParaBirimi)
        {
            kurPopup.PopupAc();
        }

        Guncelle();
    }

    // ===================================================
    // KESÝNTÝLER
    // ===================================================
    public void KesintileriGuncelle()
    {
        KesintiIsle("Avans", avansInput, avansParaDropdown);
        KesintiIsle("ÝSG", isgInput, isgParaDropdown);
        KesintiIsle("Malzeme", malzemeInput, malzemeParaDropdown);
        KesintiIsle("Yevmiye", yevmiyeInput, yevmiyeParaDropdown);
        KesintiIsle("Diðer", digerInput, digerParaDropdown);
        KesintiIsle("Teminat", teminatInput, teminatParaDropdown);

        // KDV artýk sadece oran alýyor
        double kdvOran = 0;
        double.TryParse(kdvInput.text, out kdvOran);
        manager.KdvGuncelle(kdvOran);


        Guncelle();
    }

    void KesintiIsle(string ad, TMP_InputField input, TMP_Dropdown paraDropdown)
    {
        double tutar = 0;
        double.TryParse(input.text, out tutar);

        string secilenPB =
            paraDropdown.options[paraDropdown.value].text;

        if (secilenPB != manager.aktif.varsayilanParaBirimi)
        {
            kurPopup.PopupAc();
        }

        manager.KesintiGuncelle(
            ad,
            tutar,
            secilenPB
        );
    }

    // ===================================================
    // GENEL GÜNCELLE
    // ===================================================
    void Guncelle()
    {
        HakedisBilgileriniGuncelle();
        manager.Hesapla();
        sonucText.text = manager.MetinOlustur();
    }
}