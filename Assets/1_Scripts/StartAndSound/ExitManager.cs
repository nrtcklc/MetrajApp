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
    public void ConfirmExit()
    {
        Application.Quit();
    }

    public void CancelExit()
    {
        exitWarningPanel.SetActive(false);
    }
}