using System;
using System.Collections.Generic;

[Serializable]
public class HakedisData
{
    // Firma
    public string anaFirma;
    public string altYuklenici;

    // Hakediþ
    public string hakedisAdi;
    public int hakedisNo;
    public string donem;
    public string tarih;

    // Kalemler
    public List<HakedisMetrajItem> metrajlar = new List<HakedisMetrajItem>();
    public string birim;

    // Kesintiler
    public List<HakedisKesintiItem> kesintiler = new List<HakedisKesintiItem>();

    // Toplamlar
    public double araToplam;
    public double toplamKesinti;
    public double genelToplam;
}

[Serializable]
public class HakedisMetrajItem
{
    public string ad;
    public double miktar;
    public string birim;
    public double birimFiyat;
    public string paraBirimi;
    public double tutar;
}

[Serializable]
public class HakedisKesintiItem
{
    public string ad;
    public double tutar;
    public string paraBirimi;   // EKLENDÝ
}