using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class DiyalogSatiri
{
    public string konusanKisi; // "Kedi" veya "Adam"
    [TextArea(3, 10)]
    public string cumle;
}

public class DinamikDiyalog : MonoBehaviour
{
    [Header("UI Referanslarý")]
    public TMP_Text isimText;
    public TMP_Text mesajText;

    [Header("Karakter Görselleri")]
    public Image kediGorseli;
    public Image adamGorseli;

    [Header("Diyalog Ayarlarý")]
    public DiyalogSatiri[] diyaloglar;
    public string sonrakiSahneAdi;

    private int suAnkiIndex = 0;

    // Normal renk (Tamamen görünür) ve Soluk renk (Yarý saydam ve biraz karanlýk)
    // Normal renk (Tamamen görünür ve parlak)
    private Color aktifRenk = new Color(1f, 1f, 1f, 1f);
    // Pasif renk (Saydamlýk YOK, sadece gölgede kalmýþ gibi koyu gri)
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

        // Konuþan kiþiye göre görselleri parlat/soluklaþtýr
        if (aktifSatir.konusanKisi == "Kedi")
        {
            kediGorseli.color = aktifRenk;
            adamGorseli.color = pasifRenk;
        }
        else if (aktifSatir.konusanKisi == "Adam")
        {
            adamGorseli.color = aktifRenk;
            kediGorseli.color = pasifRenk;
        }
        else
        {
            // Eðer dýþ ses konuþuyorsa (örn: konusanKisi = "Sistem") ikisini de soluk yap
            kediGorseli.color = pasifRenk;
            adamGorseli.color = pasifRenk;
        }
    }
}