using UnityEngine;

public class LogoRotate : MonoBehaviour
{
    public float rotationAngle = 5f; // Maksimum sağa-sola açı
    public float speed = 1f;         // Hızı

    private void Update()
    {
        // Sinüs dalgası ile sağa-sola dönüş
        float zRotation = Mathf.Sin(Time.time * speed) * rotationAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, zRotation);
    }
}