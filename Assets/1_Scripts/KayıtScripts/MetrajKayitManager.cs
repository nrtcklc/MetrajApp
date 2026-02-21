using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MetrajKayitManager : MonoBehaviour
{
    private string path;
    private MetrajKayitList kayitList = new MetrajKayitList();

    [Header("Liste UI")]
    public GameObject satirPrefab;
    public Transform listParent;

    public static string duzenlemeJson;
    void Awake()
    {
        path = Application.persistentDataPath + "/metraj_kayitlar.json";
        Yukle();
        ListeyiDoldur();
    }

    // --------------------
    // KAYIT EKLE
    // --------------------
    public void YeniKayitEkle(string kayitAdi,
                              string tur,
                              float toplam,
                              string detayJson)
    {
        MetrajKayitData yeni = new MetrajKayitData();

        yeni.id = Guid.NewGuid().ToString();
        yeni.kayitAdi = kayitAdi;
        yeni.kayitTuru = tur;
        yeni.kayitTarihi = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
        yeni.toplamMetraj = toplam;
        yeni.detayJson = detayJson;

        kayitList.kayitlar.Add(yeni);

        Kaydet();
        ListeyiDoldur();
    }

    // --------------------
    // SÝL
    // --------------------
    public void KayitSil(string id)
    {
        kayitList.kayitlar.RemoveAll(x => x.id == id);
        Kaydet();
    }

    // --------------------
    // JSON YAZ
    // --------------------
    void Kaydet()
    {
        string json = JsonUtility.ToJson(kayitList, true);
        File.WriteAllText(path, json);
        Debug.Log("Kayýtlar kaydedildi");
    }

    // --------------------
    // JSON OKU
    // --------------------
    void Yukle()
    {
        if (!File.Exists(path))
        {
            kayitList = new MetrajKayitList();
            return;
        }

        string json = File.ReadAllText(path);
        kayitList = JsonUtility.FromJson<MetrajKayitList>(json);
    }

    // --------------------
    // LÝSTEYÝ DOLDUR
    // --------------------
    public void ListeyiDoldur()
    {
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        //BURAYI DEÐÝÞTÝR
        for (int i = kayitList.kayitlar.Count - 1; i >= 0; i--)
        {
            var kayit = kayitList.kayitlar[i];

            GameObject yeni = Instantiate(satirPrefab, listParent);
            MetrajKayitSatirUI ui = yeni.GetComponent<MetrajKayitSatirUI>();

            ui.Setup(kayit, this);

            ShareButton share = yeni.GetComponent<ShareButton>();
            share.kayitData = kayit;
        }
    }
}