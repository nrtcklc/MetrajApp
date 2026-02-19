using System;
using System.Collections.Generic;

[Serializable]
public class MetrajKayitData
{
    public string id;
    public string kayitAdi;
    public string kayitTuru;
    public string kayitTarihi;
    public float toplamMetraj;
    public string detayJson;
}

[Serializable]
public class MetrajKayitList
{
    public List<MetrajKayitData> kayitlar = new List<MetrajKayitData>();
}
