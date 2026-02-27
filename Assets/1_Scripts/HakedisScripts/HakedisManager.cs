using UnityEngine;
using System.Text;

public class HakedisManager : MonoBehaviour
{
    public HakedisData aktif;

    void Awake()
    {
        Yeni();
    }

    public void Yeni()
    {
        aktif = new HakedisData();
    }

    // ===============================
    // FÝRMA
    // ===============================
    public void FirmaGuncelle(string ana, string alt, string ad, int no, string donem, string tarih)
    {
        aktif.anaFirma = ana;
        aktif.altYuklenici = alt;
        aktif.hakedisAdi = ad;
        aktif.hakedisNo = no;
        aktif.donem = donem;
        aktif.tarih = tarih;
    }

    // ===============================
    // METRAJ
    // ===============================
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
        m.tutar = miktar * fiyat;

        aktif.metrajlar.Add(m);
        Hesapla();
    }

    // ===============================
    // KESÝNTÝ
    // ===============================
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

    // ===============================
    // KDV
    // ===============================
    public void KdvGuncelle(double oran, string paraBirimi)
    {
        if (oran <= 0)
        {
            aktif.kesintiler.RemoveAll(x => x.ad == "KDV");
            Hesapla();
            return;
        }

        double kdvTutar = aktif.araToplam * oran / 100.0;
        KesintiGuncelle("KDV", kdvTutar, paraBirimi);
    }

    // ===============================
    // HESAPLA
    // ===============================
    public void Hesapla()
    {
        double ara = 0;

        foreach (var m in aktif.metrajlar)
        {
            m.tutar = m.miktar * m.birimFiyat;
            ara += m.tutar;
        }

        double kes = 0;

        foreach (var k in aktif.kesintiler)
        {
            kes += k.tutar;
        }

        aktif.araToplam = ara;
        aktif.toplamKesinti = kes;
        aktif.genelToplam = ara - kes;
    }

    // ===============================
    // METÝN
    // ===============================
    public string MetinOlustur()
    {
        StringBuilder sb = new StringBuilder();

        if (!string.IsNullOrEmpty(aktif.anaFirma))
            sb.AppendLine("Ana Firma: " + aktif.anaFirma);

        if (!string.IsNullOrEmpty(aktif.altYuklenici))
            sb.AppendLine("Alt Yüklenici: " + aktif.altYuklenici);

        sb.AppendLine("--------------------------------");

        foreach (var m in aktif.metrajlar)
        {
            sb.AppendLine(
                m.ad + " | " +
                m.miktar + " " + m.birim +
                " x " + m.birimFiyat + " " + m.paraBirimi +
                " = " + m.tutar + " " + m.paraBirimi
            );
        }

        if (aktif.metrajlar.Count > 0)
            sb.AppendLine("Ara Toplam: " + aktif.araToplam);

        if (aktif.kesintiler.Count > 0)
        {
            sb.AppendLine("--------------------------------");

            foreach (var k in aktif.kesintiler)
            {
                sb.AppendLine(
                    k.ad + " : -" + k.tutar + " " + k.paraBirimi
                );
            }

            sb.AppendLine("Toplam Kesinti: -" + aktif.toplamKesinti);
        }

        if (aktif.metrajlar.Count > 0)
        {
            sb.AppendLine("================================");
            sb.AppendLine("GENEL TOPLAM: " + aktif.genelToplam);
        }

        return sb.ToString();
    }
}