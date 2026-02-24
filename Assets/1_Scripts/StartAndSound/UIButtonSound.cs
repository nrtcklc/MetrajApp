using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public enum SoundType { Click, Success, Error }
    public SoundType soundType = SoundType.Click;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlaySound);
    }

    private void PlaySound()
    {
        if (UISoundManager.Instance == null) return;

        switch (soundType)
        {
            case SoundType.Click:
                UISoundManager.Instance.PlayClick();
                break;
            case SoundType.Success:
                UISoundManager.Instance.PlaySuccess();
                break;
            case SoundType.Error:
                UISoundManager.Instance.PlayError();
                break;
        }
    }
}