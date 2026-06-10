using UnityEngine;

public class KapiKapanmaSesi : MonoBehaviour
{
    // Bu fonksiyon obje SetActive(false) olduğu an Unity tarafından otomatik çalıştırılır
    private void OnDisable()
    {
        // Kapı objesi yok olmadan veya gizlenmeden hemen önce sesi dünya genelinde o noktada çalar
        AudioSource sesSource = GetComponent<AudioSource>();
        if (sesSource != null && sesSource.clip != null)
        {
            // PlayClipAtPoint sayesinde obje gizlense bile ses kesilmeden sonuna kadar çalar
            AudioSource.PlayClipAtPoint(sesSource.clip, transform.position, sesSource.volume);
        }
    }
}