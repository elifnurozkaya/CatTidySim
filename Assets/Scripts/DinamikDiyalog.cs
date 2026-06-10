using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class DiyalogSatiri
{
    public string konusanKisi; 
    [TextArea(3, 10)]
    public string cumle;
}

public class DinamikDiyalog : MonoBehaviour
{
    [Header("UI Referansları")]
    public TMP_Text isimText;
    public TMP_Text mesajText;

    [Header("Karakter Görselleri")]
    public Image kediGorseli;
    public Image evSahibiGorseli; // Değişken adını "evSahibiGorseli" olarak güncelledik

    [Header("Diyalog Ayarları")]
    public DiyalogSatiri[] diyaloglar;
    public string sonrakiSahneAdi;

    private int suAnkiIndex = 0;

    // Normal renk (Tamamen görünür ve parlak)
    private Color aktifRenk = new Color(1f, 1f, 1f, 1f);
    // Pasif renk (Saydamlık YOK, sadece gölgede kalmış gibi koyu gri)
    private Color pasifRenk = new Color(0.35f, 0.35f, 0.35f, 1f);

    void Start()
    {
        SatiriOynat();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            suAnkiIndex++;

            if (suAnkiIndex < diyaloglar.Length)
            {
                SatiriOynat();
            }
            else
            {
                SceneManager.LoadScene(sonrakiSahneAdi);
            }
        }
    }

    void SatiriOynat()
    {
        // Metinleri güncelle
        DiyalogSatiri aktifSatir = diyaloglar[suAnkiIndex];
        isimText.text = aktifSatir.konusanKisi;
        mesajText.text = aktifSatir.cumle;

        // Konuşan kişiye göre görselleri parlat/soluklaştır
        if (aktifSatir.konusanKisi == "Kedi")
        {
            kediGorseli.color = aktifRenk;
            evSahibiGorseli.color = pasifRenk;
        }
        else if (aktifSatir.konusanKisi == "Ev Sahibi") // Burayı "Ev Sahibi" olarak güncelledik
        {
            evSahibiGorseli.color = aktifRenk;
            kediGorseli.color = pasifRenk;
        }
        else
        {
            // Eğer dış ses konuşuyorsa (Örn: konusanKisi = "Sistem") ikisini de soluk yap
            kediGorseli.color = pasifRenk;
            evSahibiGorseli.color = pasifRenk;
        }
    }
}