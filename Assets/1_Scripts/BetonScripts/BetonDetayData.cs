using System;
using System.Collections.Generic;

[Serializable]
public class BetonDetayData
{
    public List<BetonSatirData> satirlar = new List<BetonSatirData>();
}

[Serializable]
public class BetonSatirData
{
    public string metrajAdi;

    public float benzer;
    public float adet;
    public float en;
    public float boy;
    public float yukseklik;

    public string hesapOzet;
    public float hacim;
}