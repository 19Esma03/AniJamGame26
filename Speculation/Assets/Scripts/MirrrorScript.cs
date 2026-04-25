using UnityEngine;

public class MirrorScript : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform mirrorPlane; // Sahnedeki Quad
    public Camera playerCam;      // Main Camera

    void Start()
    {
        if (playerCam == null) playerCam = Camera.main;

        // Ölçek Kontrolü (Hata Önleyici)
        if (mirrorPlane != null)
        {
            Vector3 scale = mirrorPlane.localScale;
            if (scale.x == 0 || scale.y == 0 || scale.z == 0)
            {
                Debug.LogWarning("Guts, Quad'ýn ölçeði 0 olamaz! Otomatik olarak 1 yapýldý.");
                mirrorPlane.localScale = new Vector3(
                    scale.x == 0 ? 1 : scale.x,
                    scale.y == 0 ? 1 : scale.y,
                    scale.z == 0 ? 1 : scale.z
                );
            }
        }
    }

    void LateUpdate()
    {
        if (playerCam == null || mirrorPlane == null) return;

        // 1. DÜNYA KOORDÝNATLARINDA POZÝSYON (Planar Reflection Math)
        // Oyuncunun aynaya olan dik uzaklýðýný ve yönünü bulur
        Vector3 playerPos = playerCam.transform.position;
        Vector3 planePos = mirrorPlane.position;
        Vector3 planeNormal = mirrorPlane.forward; // Quad'ýn ön yüzü

        // Formül: P_refl = P - 2 * (n * (P - P_plane)) * n
        float distance = Vector3.Dot(planeNormal, playerPos - planePos);
        Vector3 reflectedPos = playerPos - 2 * distance * planeNormal;

        transform.position = reflectedPos;

        // 2. DÜNYA KOORDÝNATLARINDA ROTASYON
        // Oyuncunun bakýþ yönünü aynanýn normaline göre sektirir
        Vector3 lookDir = Vector3.Reflect(playerCam.transform.forward, planeNormal);
        transform.rotation = Quaternion.LookRotation(lookDir, mirrorPlane.up);
    }
}