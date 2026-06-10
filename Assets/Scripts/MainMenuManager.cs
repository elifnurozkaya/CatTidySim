using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçişleri için şart

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject nasilOynanirPaneli;
    public GameObject emegiGecenlerPaneli;

    private void Start()
    {
        // Ana menü yüklendiğinde farenin her koşulda görünür olduğundan emin ol
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }
    // 1. OYUNU BAŞLAT BUTONU
    public void OyunuBaslat()
    {
        SceneManager.LoadScene("AraSahne_1"); // İlk diyaloğa gönderir
    }

    // 2. NASIL OYNANIR BUTONU
    public void NasilOynanirAc()
    {
        nasilOynanirPaneli.SetActive(true); // Paneli görünür yapar
    }

    // 3. EMEĞİ GEÇENLER BUTONU
    public void EmegiGecenlerAc()
    {
        emegiGecenlerPaneli.SetActive(true); // Paneli görünür yapar
    }

    // 4. GERİ DÖN BUTONLARI
    public void PanelleriKapat()
    {
        // İki paneli de kapatır, hangisi açıksa o kapanmış olur
        nasilOynanirPaneli.SetActive(false);
        emegiGecenlerPaneli.SetActive(false);
    }

    // 5. ÇIKIŞ BUTONU
    public void OyundanCik()
    {
        Debug.Log("Oyundan çıkılıyor..."); // Editor'de çalıştığını anlamamız için
        Application.Quit(); // Gerçek (Build alınmış) oyunu kapatır
    }
}