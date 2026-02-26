using UnityEngine;
using UnityEngine.InputSystem;

public class DoubleBackToExit : MonoBehaviour
{
    private float backPressTime = 0f;
    private float exitDelay = 2f;

    void Update()
    {
        // Android back tuþu = Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Time.time - backPressTime < exitDelay)
            {
                Application.Quit();
            }
            else
            {
                backPressTime = Time.time;
                ShowToast("Uygulamayý kapatmak için 2 kez geri tuþuna basýn.");
            }
        }
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