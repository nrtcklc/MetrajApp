using System;
using System.Collections.Generic;
using UnityEngine;  

[Serializable]
public class DemirDetayData
{
    public List<DemirSatirData> satirlar = new List<DemirSatirData>();
}

[Serializable]
public class DemirSatirData
{
    public string metrajAdi;
    public float benzer;
    public float adet;
    public float cap;   // en yerine çap
    public float boy;
    public float hacim;
}