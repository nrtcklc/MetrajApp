using UnityEngine;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

public class HakedisManager : MonoBehaviour
{
    public HakedisData aktif;

    double aktifKur = 1;
    bool kurGerekli = false;
    double kdvOran = 0;   // KDV oraný burada tutulur

    void Awake()
    {
        Yeni();
    }

    public void Yeni()
    {
        aktif = new HakedisData();
        aktif.varsayilanParaBirimi = "TL";
        aktifKur = 1;
        kurGerekli = false;
        kdvOran = 0;
    }

    // ===================================================
    // FÝRMA
    // ===================================================

    public void FirmaGuncelle(string ana, string alt, string ad, int no, string donem, string tarih)
    {
        aktif.anaFirma = ana;
        aktif.altYuklenici = alt;
        aktif.hakedisAdi = ad;
        aktif.hakedisNo = no;
        aktif.donem = donem;
        aktif.tarih = tarih;
    }

    // ===================================================
    // VARSAYILAN PARA
    // ===================================================

    public void VarsayilanParaBirimiAyarla(string pb)
    {
        aktif.varsayilanParaBirimi = pb;
        KurKontrol();
    }

    void KurKontrol()
    {
        kurGerekli = false;

        foreach (var m in aktif.metrajlar)
            if (m.paraBirimi != aktif.varsayilanParaBirimi)
                kurGerekli = true;

        foreach (var k in aktif.kesintiler)
            if (k.paraBirimi != aktif.varsayilanParaBirimi)
                kurGerekli = true;

        if (!kurGerekli)
            aktifKur = 1;
    }

    public void KurGuncelle(double kur)
    {
        aktifKur = kur;
        kurGerekli = false;
        Hesapla();
    }

    // ===================================================
    // METRAJ
    // ===================================================

    public void MetrajEkle(string ad, double miktar, string birim, double fiyat, string para)
    {
        if (miktar <= 0 || fiyat <= 0) return;
        if (string.IsNullOrEmpty(ad)) return;

        HakedisMetrajItem m = new HakedisMetrajItem();
        m.ad = ad;
        m.miktar = miktar;
        m.birim = birim;
        m.birimFiyat = fiyat;
        m.paraBirimi = para;

        aktif.metrajlar.Add(m);

        KurKontrol();
        Hesapla();
    }

    // ===================================================
    // KESÝNTÝ
    // ===================================================

    public void KesintiGuncelle(string ad, double tutar, string paraBirimi)
    {
        if (tutar <= 0)
        {
            aktif.kesintiler.RemoveAll(x => x.ad == ad);
            Hesapla();
            return;
        }

        var mevcut = aktif.kesintiler.Find(x => x.ad == ad);

        if (mevcut != null)
        {
            mevcut.tutar = tutar;
            mevcut.paraBirimi = paraBirimi;
        }
        else
        {
            HakedisKesintiItem k = new HakedisKesintiItem();
            k.ad = ad;
            k.tutar = tutar;
            k.paraBirimi = paraBirimi;

            aktif.kesintiler.Add(k);
        }

        Hesapla();
    }

    // ===================================================
    // KDV
    // ===================================================

    public void KdvGuncelle(double oran)
    {
        kdvOran = oran;
        Hesapla();
    }

    // ===================================================
    // HESAP
    // ===================================================

    public void Hesapla()
    {
        double ara = 0;

        foreach (var m in aktif.metrajlar)
        {
            double deger = m.miktar * m.birimFiyat;

            if (m.paraBirimi != aktif.varsayilanParaBirimi)
                deger *= aktifKur;

            ara += deger;
        }

        aktif.araToplam = ara;

        // ---- KDV SADECE METRAJ ÜZERÝNDEN ----

        aktif.kesintiler.RemoveAll(x => x.ad == "KDV");

        if (kdvOran > 0)
        {
            double kdvTutar = ara * kdvOran / 100.0;

            HakedisKesintiItem k = new HakedisKesintiItem();
            k.ad = "KDV";
            k.tutar = kdvTutar;
            k.paraBirimi = aktif.varsayilanParaBirimi;

            aktif.kesintiler.Add(k);
        }

        // ---- TOPLAM KESÝNTÝ ----

        double kes = 0;

        foreach (var k in aktif.kesintiler)
        {
            double deger = k.tutar;

            if (k.paraBirimi != aktif.varsayilanParaBirimi)
                deger *= aktifKur;

            kes += deger;
        }

        aktif.toplamKesinti = kes;
        aktif.genelToplam = ara - kes;
    }

    // ===================================================
    // FORMAT
    // ===================================================

    string FormatPara(double deger)
    {
        return deger.ToString("N2", new CultureInfo("tr-TR"))
               + " " + aktif.varsayilanParaBirimi;
    }

    // ===================================================
    // METÝN
    // ===================================================

    public string MetinOlustur()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Ana Firma: " + aktif.anaFirma);
        sb.AppendLine("Alt Yüklenici: " + aktif.altYuklenici);
        sb.AppendLine("--------------------------------");

        foreach (var m in aktif.metrajlar)
        {
            sb.AppendLine(
                m.ad + " = " +
                FormatPara(m.miktar * m.birimFiyat)
            );
        }

        sb.AppendLine("--------------------------------");
        sb.AppendLine("Ara Toplam: " + FormatPara(aktif.araToplam));

        if (aktif.kesintiler.Count > 0)
        {
            sb.AppendLine("--------------------------------");

            foreach (var k in aktif.kesintiler)
                sb.AppendLine(k.ad + " : " + FormatPara(k.tutar));

            sb.AppendLine("Toplam Kesinti: " + FormatPara(aktif.toplamKesinti));
        }

        sb.AppendLine("================================");
        sb.AppendLine("GENEL TOPLAM: " + FormatPara(aktif.genelToplam));

        return sb.ToString();
    }
}