using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI; 
using System.Collections; 

public class UIManager : MonoBehaviour
{
    [Header("UI Elementleri")]
    [SerializeField] private TextMeshProUGUI skorText;
    [SerializeField] private TextMeshProUGUI sureText;
    [SerializeField] private GameObject pausePaneli;
    [SerializeField] private GameObject gameOverPanelSure; // Süre bitince açılacak panel
    [SerializeField] private GameObject gameOverPanelSkor; // Puan yetmeyince açılacak panel
    [SerializeField] private GameObject winPaneli;

    [Header("Oyun Değerleri")]
    [SerializeField, Min(0f)] private float kalanSure = 180f; // Konuştuğumuz gibi fix 3 dakika (180 saniye)
    
    // Diğer scriptlerin (kapıların) hedefleri kontrol edebilmesi için public yapıldı
    public int mevcutSkor = 0; 

    [Header("Ekran Efekti")]
    [SerializeField] private Image flashEkrani;
    [SerializeField] private Color dogruRenk = new Color(0f, 1f, 0f, 0.3f); // %30 saydam yeşil
    [SerializeField] private Color yanlisRenk = new Color(1f, 0f, 0f, 0.3f); // %30 saydam kırmızı
    [SerializeField] private float fadeHizi = 2f; // Rengin kaybolma hızı

    [Header("Toplanan Nesne Gostergesi")]
    public TMP_Text toplananNesneText;
    private bool oyunDurduMu = false;

    public UnityEvent<int> SkorDegisti;
    public UnityEvent OyunBittiEvent;

    private void Awake()
    {
        Time.timeScale = 1f;
        // Oyuna başlarken UI'nın seçili bir öğeyle başlamasını engelle
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
    
    [Header("Sahneye Özel Ayarlar")]
    public float sahneSuresi = 180f; // Buraya Inspector'dan 180 yazabilirsin

    private void Start()
    {
        kalanSure = sahneSuresi; 
        UpdateScoreUI();
        UpdateSureUI();
        
        // Başlangıçta tüm panellerin kapalı olduğundan emin ol
        if (pausePaneli) pausePaneli.SetActive(false);
        if (gameOverPanelSure) gameOverPanelSure.SetActive(false);
        if (gameOverPanelSkor) gameOverPanelSkor.SetActive(false);
        if (winPaneli) winPaneli.SetActive(false);
    }

    private void Update()
    {
        // --- KRİTİK KONTROL: Kaybetme veya Kazanma ekranları açıksa ---
        bool surePaneliAcik = gameOverPanelSure != null && gameOverPanelSure.activeSelf;
        bool skorPaneliAcik = gameOverPanelSkor != null && gameOverPanelSkor.activeSelf;
        bool kazanmaPaneliAcik = winPaneli != null && winPaneli.activeSelf;

        if (surePaneliAcik || skorPaneliAcik || kazanmaPaneliAcik)
        {
            Time.timeScale = 0f; // Arkadaki tüm fizik ve zamanı dondurur
            Cursor.lockState = CursorLockMode.None; // Fareyi serbest bırakır
            Cursor.visible = true; // Fareyi görünür yapar
            return; // ESC tuşu dahil aşağıdaki hiçbir girdiyi okumaz!
        }

        // --- ESC TUŞU İLE DURAKLATMA KONTROLÜ ---
        if (kalanSure > 0f && Input.GetKeyDown(KeyCode.Escape))
        {
            OyunDurumuDegistir();
        }

        if (oyunDurduMu || kalanSure <= 0f) return;

        // --- SÜRE SAYACI ---
        kalanSure -= Time.deltaTime;

        if (kalanSure <= 0f)
        {
            kalanSure = 0f;
            UpdateSureUI();
            OyunuKaybetSure(); // Süre bittiğinde direkt süre kaybetme fonksiyonunu çağır
        }
        else
        {
            UpdateSureUI();
        }
    }

    public void SkorEkle(int miktar)
    {
        if (miktar == 0) return;
        mevcutSkor += miktar;
        UpdateScoreUI();
        SkorDegisti?.Invoke(mevcutSkor);

        if (flashEkrani != null)
        {
            if (miktar > 0)
            {
                StartCoroutine(EkranFlash(dogruRenk));
            }
            else
            {
                StartCoroutine(EkranFlash(yanlisRenk));
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (skorText != null)
            skorText.text = $"Skor: {mevcutSkor}";
    }

    public void NesneSayaciniGuncelle(int toplanan, int hedef)
    {
        if (toplananNesneText != null)
        {
            toplananNesneText.text = toplanan + " / " + hedef;
        }
    }

    private void UpdateSureUI()
    {
        if (sureText == null) return;
        int dakika = Mathf.FloorToInt(kalanSure / 60f);
        int saniye = Mathf.FloorToInt(kalanSure % 60f);
        sureText.text = $"Süre: {dakika:00}:{saniye:00}"; // Görsellik için nokta yerine iki nokta konuldu
    }

    public void OyunuDurdur()
    {
        if (oyunDurduMu) return;
        oyunDurduMu = true;
        if (pausePaneli) pausePaneli.SetActive(true);
        Time.timeScale = 0f;
        
        // Duraklatıldığında butona basabilmek için fareyi aç
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OyunuDevamEttir()
    {
        if (!oyunDurduMu) return;
        oyunDurduMu = false;
        if (pausePaneli) pausePaneli.SetActive(false);
        Time.timeScale = 1f;
        
        // Oyun devam edince fareyi kameraya geri kilitle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OyunDurumuDegistir()
    {
        if (oyunDurduMu) OyunuDevamEttir(); else OyunuDurdur();
    }

    // 1. SÜRE BİTİNCE ÇAĞRILACAK FONKSİYON
    public void OyunuKaybetSure()
    {
        oyunDurduMu = true;
        Time.timeScale = 0f;
        if (gameOverPanelSure) gameOverPanelSure.SetActive(true);
        OyunBittiEvent?.Invoke();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 2. PUAN YETMEYİNCE ÇAĞRILACAK FONKSİYON
    public void OyunuKaybetSkor()
    {
        oyunDurduMu = true;
        Time.timeScale = 0f;
        if (gameOverPanelSkor) gameOverPanelSkor.SetActive(true);
        OyunBittiEvent?.Invoke();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OyunuKazan()
    {
        oyunDurduMu = true;
        Time.timeScale = 0f; 
        if (winPaneli) winPaneli.SetActive(true); 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void YenidenBaslat()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AnaMenuyeDon(string sahneAdi = "AnaMenuSahnesi") // Ana menü sahnesinin adını Inspector'dan da girebilirsin
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sahneAdi);
    }

    public void OyunuSifirla(float yeniSure = 180f)
    {
        mevcutSkor = 0;
        kalanSure = yeniSure;
        oyunDurduMu = false;
        
        if (gameOverPanelSure) gameOverPanelSure.SetActive(false);
        if (gameOverPanelSkor) gameOverPanelSkor.SetActive(false);
        if (pausePaneli) pausePaneli.SetActive(false);
        
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
            
        UpdateScoreUI();
        UpdateSureUI();
    }

    private IEnumerator EkranFlash(Color hedefRenk)
    {
        flashEkrani.color = hedefRenk;
        Color suAnkiRenk = hedefRenk;

        while (suAnkiRenk.a > 0f)
        {
            suAnkiRenk.a -= Time.deltaTime * fadeHizi;
            flashEkrani.color = suAnkiRenk;
            yield return null; 
        }

        suAnkiRenk.a = 0f;
        flashEkrani.color = suAnkiRenk;
    }
}