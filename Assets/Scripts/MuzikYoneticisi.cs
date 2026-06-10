using UnityEngine;

public class MuzikYoneticisi : MonoBehaviour
{
    private static MuzikYoneticisi _instance;

    void Awake()
    {
        // Eğer zaten bir Müzik Yöneticisi varsa, yenisinin oluşmasını engelle (Çift müzik çalmasın)
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // Bu objeyi sahneler arası geçişte yok etme!
        DontDestroyOnLoad(gameObject);
    }
}