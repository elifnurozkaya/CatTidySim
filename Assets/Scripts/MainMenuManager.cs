using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        // Ana menü sahnesi yüklendiði an fare imlecini görünür yap ve kilidi kaldýr
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Oyun duraklatýlmýþ (Pause) durumdan gelindiyse zamaný normale döndür
        Time.timeScale = 1f;
    }

    public void OyunaBasla()
    {
        // Banyo sahnesi ilk bölüm olduðu için onu yüklüyoruz.
        SceneManager.LoadScene("AraSahne_1");
    }

    public void OyundanCik()
    {
        Debug.Log("Oyundan çýkýldý!");
        Application.Quit();
    }
}