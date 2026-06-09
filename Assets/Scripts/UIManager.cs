using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Image bileþenini kullanabilmek için
using System.Collections; // Coroutine kullanabilmek için

public class UIManager : MonoBehaviour
{
    [Header("UI Elementleri")]
    [SerializeField] private TextMeshProUGUI skorText;
    [SerializeField] private TextMeshProUGUI sureText;
    [SerializeField] private GameObject pausePaneli;
    [SerializeField] private GameObject gameOverPaneli;
    [SerializeField] private GameObject winPaneli;

    [Header("Oyun Deðerleri")]
    [SerializeField, Min(0f)] private float kalanSure = 60f;
    [SerializeField] private int toplamSkor = 0;

    [Header("Ekran Efekti")]
    [SerializeField] private Image flashEkrani;
    [SerializeField] private Color dogruRenk = new Color(0f, 1f, 0f, 0.3f); // %30 saydam yeþil
    [SerializeField] private Color yanlisRenk = new Color(1f, 0f, 0f, 0.3f); // %30 saydam kýrmýzý
    [SerializeField] private float fadeHizi = 2f; // Rengin kaybolma hýzý

    [Header("Toplanan Nesne Gostergesi")]
    public TMP_Text toplananNesneText;
    private bool oyunDurduMu = false;

    public UnityEvent<int> SkorDegisti;
    public UnityEvent OyunBittiEvent;

    private void Awake()
    {
        Time.timeScale = 1f;
        // Oyuna baþlarken UI'nýn seçili bir öðeyle baþlamasýný engelle
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
    [Header("Sahneye Özel Ayarlar")]
    public float sahneSuresi = 90f; // Buraya Inspector'dan sahnene göre 30, 120 veya 90 yazacaksýn

    private void Start()
    {
        kalanSure = sahneSuresi; // Ýþte kilit nokta! Oyun baþlar baþlamaz süre buradaki deðer olacak.
        UpdateScoreUI();
        UpdateSureUI();
        if (pausePaneli) pausePaneli.SetActive(false);
        if (gameOverPaneli) gameOverPaneli.SetActive(false);
        if (winPaneli) winPaneli.SetActive(false);
    }

    private void Update()
    {
        // --- ESC TUÞU ÝLE DURAKLATMA KONTROLÜ ---
        // Oyun bitmediyse ESC tuþuna basýldýðýnda durumu deðiþtir (Durdur/Devam Ettir)
        if (kalanSure > 0f && Input.GetKeyDown(KeyCode.Escape))
        {
            OyunDurumuDegistir();
        }

        // Eðer oyun þu an duraklatýlmýþsa veya süre bittiyse aþaðýdaki süre sayacýný ÇALIÞTIRMA
        if (oyunDurduMu || kalanSure <= 0f) return;

        // --- SÜRE SAYACI ---
        kalanSure -= Time.deltaTime;

        if (kalanSure <= 0f)
        {
            kalanSure = 0f;
            UpdateSureUI();
            OyunBitti();
        }
        else
        {
            UpdateSureUI();
        }
    }

    public void SkorEkle(int miktar)
    {
        if (miktar == 0) return;
        toplamSkor += miktar;
        UpdateScoreUI();
        SkorDegisti?.Invoke(toplamSkor);

        // --- YENÝ EKLENEN EFEKT KISMI ---
        if (flashEkrani != null)
        {
            if (miktar > 0)
            {
                // Artý puansa yeþil flash
                StartCoroutine(EkranFlash(dogruRenk));
            }
            else
            {
                // Eksi puansa kýrmýzý flash
                StartCoroutine(EkranFlash(yanlisRenk));
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (skorText != null)
            skorText.text = $"Skor: {toplamSkor}";
    }

    // Bu fonksiyonu her eþya toplandýðýnda çaðýracaðýz
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
        sureText.text = $"Süre: {dakika:00}.{saniye:00}";
    }

    public void OyunuDurdur()
    {
        if (oyunDurduMu) return;
        oyunDurduMu = true;
        if (pausePaneli) pausePaneli.SetActive(true);
        Time.timeScale = 0f;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OyunuDevamEttir()
    {
        if (!oyunDurduMu) return;
        oyunDurduMu = false;
        if (pausePaneli) pausePaneli.SetActive(false);
        Time.timeScale = 1f;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OyunDurumuDegistir()
    {
        if (oyunDurduMu) OyunuDevamEttir(); else OyunuDurdur();
    }

    private void OyunBitti()
    {
        oyunDurduMu = true;
        Time.timeScale = 0f;
        if (gameOverPaneli) gameOverPaneli.SetActive(true);
        OyunBittiEvent?.Invoke();
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OyunuKazan()
    {
        oyunDurduMu = true;
        Time.timeScale = 0f; // Zamaný ve kedinin hareketini durdurur
        if (winPaneli) winPaneli.SetActive(true); // Kazanma ekranýný açar

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void YenidenBaslat()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AnaMenuyeDon(string sahneAdi = "AnaMenuSahnesi")
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sahneAdi);
    }

    public void OyunuSifirla(float yeniSure = 60f)
    {
        toplamSkor = 0;
        kalanSure = yeniSure;
        oyunDurduMu = false;
        if (gameOverPaneli) gameOverPaneli.SetActive(false);
        if (pausePaneli) pausePaneli.SetActive(false);
        Time.timeScale = 1f;
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
        UpdateScoreUI();
        UpdateSureUI();
    }

    private IEnumerator EkranFlash(Color hedefRenk)
    {
        // Ekraný anýnda belirlediðimiz renge (yarý saydam) boya
        flashEkrani.color = hedefRenk;
        Color suAnkiRenk = hedefRenk;

        // Saydamlýk (Alpha - a) deðeri 0 olana kadar yavaþça düþür
        while (suAnkiRenk.a > 0f)
        {
            suAnkiRenk.a -= Time.deltaTime * fadeHizi;
            flashEkrani.color = suAnkiRenk;
            yield return null; // Bir sonraki kareyi (frame) bekle
        }

        // Emin olmak için sonunda tamamen görünmez yap
        suAnkiRenk.a = 0f;
        flashEkrani.color = suAnkiRenk;
    }
}