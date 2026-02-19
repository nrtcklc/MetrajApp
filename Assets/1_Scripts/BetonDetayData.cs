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
    public string hesapOzet;
    public float hacim;
}
