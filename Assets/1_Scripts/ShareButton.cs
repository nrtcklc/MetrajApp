using UnityEngine;

public class ShareButton : MonoBehaviour
{
    public MetrajKayitData kayitData;

    public void ShareThisKayit()
    {
        MetrajShareManager manager = FindObjectOfType<MetrajShareManager>();

        if (manager == null)
        {
            Debug.LogError("MetrajShareManager bulunamadý!");
            return;
        }

        manager.ShareKayit(
    kayitData.kayitAdi,
    kayitData.kayitTuru,
    kayitData.detayJson);
    }
}
