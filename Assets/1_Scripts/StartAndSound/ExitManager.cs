using UnityEngine;
using UnityEngine.InputSystem;

public class ExitManager : MonoBehaviour
{
    [Header("Unsaved Tables")]
    public bool betonUnsaved = false;
    public bool kalipUnsaved = false;
    public bool demirUnsaved = false;
    public bool hakedisUnsaved = false;

    [Header("UI")]
    public GameObject exitWarningPanel;

    void Update()
    {
        // Android back tuþu
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
        return betonUnsaved || kalipUnsaved || demirUnsaved || hakedisUnsaved;
    }

    // =========================
    // BETON
    // =========================
    public void SetBetonUnsavedTrue() => betonUnsaved = true;
    public void SetBetonUnsavedFalse() => betonUnsaved = false;

    // =========================
    // KALIP
    // =========================
    public void SetKalipUnsavedTrue() => kalipUnsaved = true;
    public void SetKalipUnsavedFalse() => kalipUnsaved = false;

    // =========================
    // DEMÝR
    // =========================
    public void SetDemirUnsavedTrue() => demirUnsaved = true;
    public void SetDemirUnsavedFalse() => demirUnsaved = false;

    // =========================
    // HAKEDÝÞ
    // =========================
    public void SetHakedisUnsavedTrue() => hakedisUnsaved = true;
    public void SetHakedisUnsavedFalse() => hakedisUnsaved = false;

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