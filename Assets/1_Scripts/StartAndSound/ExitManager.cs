using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class ExitManager : MonoBehaviour
{
    [Header("Unsaved Content Parents")]
    [Tooltip("ScrollView Content objelerini buraya ekleyin")]
    public List<Transform> unsavedContents = new List<Transform>();

    [Header("UI")]
    public GameObject exitWarningPanel;

    void Update()
    {
        // Android geri tuþu
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TryExit();
        }
    }

    // =====================================
    // ORTAK ÇIKIÞ KONTROLÜ
    // =====================================
    public void TryExit()
    {
        if (HasAnyUnsavedData())
        {
            exitWarningPanel.SetActive(true);
        }
        else
        {
            Application.Quit();
        }
    }

    bool HasAnyUnsavedData()
    {
        foreach (Transform content in unsavedContents)
        {
            if (content == null)
                continue;

            if (content.childCount > 0)
                return true;
        }

        return false;
    }

    // =========================
    // PANEL BUTONLARI
    // =========================
    public void CancelExit()
    {
        exitWarningPanel.SetActive(false);
    }
    public void ConfirmExit()
    {
        ShowToast("Uygulamayý kapatmak için 2 kez geri tuþuna basýn.");
        Application.Quit();
    }

    void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    {
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");

        currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                "makeText",
                context,
                message,
                toastClass.GetStatic<int>("LENGTH_SHORT")
            );
            toastObject.Call("show");
        }));
    }
#endif
        Debug.Log(message);

    }
}