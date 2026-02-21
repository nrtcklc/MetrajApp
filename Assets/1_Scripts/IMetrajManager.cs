using UnityEngine;

public interface IMetrajManager
{
    // Satýr silme iþlemi (MetrajSatir tarafýndan çaðrýlýr)
    void SatirSil(MetrajSatir satir);

    // Toplam deðeri döndürür (Beton m³, Kalýp m², Demir ton)
    float GetToplamMetrajValue();

    // Kayýt sistemi için JSON üretir
    string GetDetayJson();

    // Düzenleme için JSON’dan yükleme yapar
    void LoadFromJson(string json);

    // Tüm satýrlarý temizler
    void TumSatirlariTemizle();
}