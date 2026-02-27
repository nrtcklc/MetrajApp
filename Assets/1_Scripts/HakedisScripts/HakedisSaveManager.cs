using UnityEngine;
using System.IO;

public class HakedisSaveManager : MonoBehaviour
{
    string klasorYolu;


    private void Awake()
    {
        klasorYolu = Application.persistentDataPath + "/Hakedisler/";

        if (!Directory.Exists(klasorYolu))
            Directory.CreateDirectory(klasorYolu);
    }

    public void Kaydet(HakedisData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string dosyaAdi = data.hakedisAdi + "_" + data.hakedisNo + ".json";
        File.WriteAllText(klasorYolu + dosyaAdi, json);
    }
}