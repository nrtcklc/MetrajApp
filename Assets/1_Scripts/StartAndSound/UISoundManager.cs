using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;

    [Header("UI Sesleri")]
    public AudioClip clickSound;
    public AudioClip successSound;
    public AudioClip errorSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlayClick()   => PlaySound(clickSound);
    public void PlaySuccess() => PlaySound(successSound);
    public void PlayError()   => PlaySound(errorSound);

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
     public void StopAll()
    {
        AudioSource[] allAudio = GameObject.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);

        foreach (AudioSource audio in allAudio)
        {
            audio.Stop();
        }
    }
}