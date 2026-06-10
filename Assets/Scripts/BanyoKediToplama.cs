using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BanyoKediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 8;
    public GameObject kapiEngeli;

    [Header("Skor Sistemi")]
    // UIManager de�i�kenini s�n�f�n ���NE ald�k ve eski de�i�kenleri sildik.
    public UIManager uiManager;

    private GameObject agizdakiNesne = null;
    private GameObject yakindakiNesne = null;
    private bool sepeteYakinMi = false;
    private string sepetTuru = "";
    private int sepetlenenNesneSayisi = 0;

    void Start()
    {
        // Eski Text g�ncelleme kodlar�n� sildik.
        if (kapiEngeli != null)
        {
            kapiEngeli.SetActive(true);
        }

        if (uiManager != null)
        {
            uiManager.NesneSayaciniGuncelle(sepetlenenNesneSayisi, hedefNesneSayisi);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (agizdakiNesne == null && yakindakiNesne != null)
            {
                NesneyiAgzaAl();
            }
            else if (agizdakiNesne != null && sepeteYakinMi)
            {
                NesneyiSepeteBirak();
            }
        }
    }

    void NesneyiAgzaAl()
    {
        agizdakiNesne = yakindakiNesne;
        yakindakiNesne = null;

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        agizdakiNesne.transform.SetParent(agizNoktasi);
        agizdakiNesne.transform.localPosition = Vector3.zero;
        agizdakiNesne.transform.localRotation = Quaternion.identity;

        Debug.Log("Nesne agza alindi: " + agizdakiNesne.name);
    }

    void NesneyiSepeteBirak()
    {
        bool kirmiziNesneMi = agizdakiNesne.name.Contains("NesneK");
        bool maviNesneMi = agizdakiNesne.name.Contains("NesneM");

        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // --- NESNEN�N GER� ALINMASINI ENGELLEME ---
        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // --- PUANLAMA VE MESAJ KONTROL� (Sadece UIManager) ---
        if ((kirmiziNesneMi && sepetTuru == "Kirmizi") || (maviNesneMi && sepetTuru == "Mavi"))
        {
            // Do�ru e�le�me
            if (uiManager != null) uiManager.SkorEkle(10);
            Debug.Log("Dogru sepet! +10 Puan eklendi.");
        }
        else
        {
            // Yanl�� e�le�me
            if (uiManager != null) uiManager.SkorEkle(-5);
            Debug.LogWarning("Yanlis sepet! -5 Puan dusuldu.");
        }

        sepetlenenNesneSayisi++;
        Debug.Log("Sepete atilan toplam nesne: " + sepetlenenNesneSayisi + " / " + hedefNesneSayisi);

        if (uiManager != null)
        {
            uiManager.NesneSayaciniGuncelle(sepetlenenNesneSayisi, hedefNesneSayisi);
        }

        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            Debug.Log("TEBRIKLER! Tum nesneler bitti. Kapi acildi, simdi gecebilirsin!");
            if (kapiEngeli != null)
            {
                kapiEngeli.SetActive(false);
            }
        }

        agizdakiNesne = null;
    }

    private void OnTriggerEnter(Collider other)
{
    if ((other.gameObject.name.Contains("NesneK") || other.gameObject.name.Contains("NesneM")) && agizdakiNesne == null)
    {
        yakindakiNesne = other.gameObject;
    }

    if (other.gameObject.name == "Banyo_K_Sepet")
    {
        sepeteYakinMi = true;
        sepetTuru = "Kirmizi";
    }
    else if (other.gameObject.name == "Banyo_M_Sepet")
    {
        sepeteYakinMi = true;
        sepetTuru = "Mavi";
    }

    // --- KAPIDAN GEÇİŞ VE SKOR KONTROLÜ ---
    if (other.gameObject.name == "Salona_Gecis_Kapisi")
    {
        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            // Sahnede aktif olan UIManager'a ulaşıyoruz
            UIManager uiManager = FindFirstObjectByType<UIManager>();

            if (uiManager != null)
            {
                // Banyo barajı olan 50 puanı kontrol ediyoruz
                if (uiManager.mevcutSkor >= 50)
                {
                    Debug.Log("Tebrikler! Yeterli skorla Salon sahnesine geciliyor...");
                    SceneManager.LoadScene("AraSahne_2");
                }
                else
                {
                    // Eğer oyuncu tüm eşyaları topladı ama skoru 50'den düşükse (örn: -40)
                    Debug.Log("Skor yetersiz! Skor kaybetme paneli aciliyor...");
                    uiManager.OyunuKaybetSkor();
                }
            }
            else
            {
                // Güvenlik önlemi: Eğer UIManager sahnede bulunamazsa oyun kilitlenmesin diye direkt geçsin
                Debug.LogError("Sahnede UIManager bulunamadi!");
                SceneManager.LoadScene("AraSahne_2");
            }
        }
    }
}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == yakindakiNesne)
        {
            yakindakiNesne = null;
        }

        if (other.gameObject.name == "Banyo_K_Sepet" || other.gameObject.name == "Banyo_M_Sepet")
        {
            sepeteYakinMi = false;
            sepetTuru = "";
        }
    }
}