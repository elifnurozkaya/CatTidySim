using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SalonKediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 9; // Hiyerar�ide toplam 9 nesne (3 Sari, 3 Kirmizi, 3 Mor) 
    public GameObject kapiEngeli;

    [Header("Skor Sistemi")]
    // UIManager de�i�kenini s�n�f�n ���NE ald�k ve eski de�i�kenleri tamamen temizledik.
    public UIManager uiManager;

    private GameObject agizdakiNesne = null;
    private GameObject yakindakiNesne = null;
    private bool sepeteYakinMi = false;
    private string sepetTuru = ""; // "Sari", "Kirmizi" veya "Mor" olacak
    private int sepetlenenNesneSayisi = 0;

    void Start()
    {
        // Eski text atamas� silindi, sadece kap� kontrol� kald�.
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

        Debug.Log("Salon Nesnesi agza alindi: " + agizdakiNesne.name);
    }

    void NesneyiSepeteBirak()
    {
        // 1. Nesne renklerini isim kontrol� ile ay�rt ediyoruz
        bool sariNesneMi = agizdakiNesne.name.Contains("NesneS");
        bool kirmiziNesneMi = agizdakiNesne.name.Contains("NesneK");
        bool morNesneMi = agizdakiNesne.name.Contains("NesneM");

        // 2. Nesneyi a��zdan fiziksel olarak b�rakma i�lemleri
        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Fiziksel olarak kedi taraf�ndan tekrar hemen al�nmas�n� engellemek i�in collider'� kapat�yoruz
        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // 3. TEK VE NET E�LE�ME KONTROL� (UIManager Entegrasyonlu)
        if ((sariNesneMi && sepetTuru == "Sari") ||
            (kirmiziNesneMi && sepetTuru == "Kirmizi") ||
            (morNesneMi && sepetTuru == "Mor"))
        {
            // DO�RU E�LE�ME
            if (uiManager != null)
            {
                uiManager.SkorEkle(10); // UI ekran�ndaki skoru 10 art�r�r
            }
            Debug.Log("Salon - Dogru sepet! +10 Puan eklendi.");
        }
        else
        {
            // YANLI� E�LE�ME
            if (uiManager != null)
            {
                uiManager.SkorEkle(-5); // UI ekran�ndaki skoru 5 azalt�r
            }
            Debug.LogWarning("Salon - Yanlis sepet! -5 Puan dusuldu.");
        }

        // 4. Hedef nesne ve kap� kontrol�
        sepetlenenNesneSayisi++;
        Debug.Log("Salonda sepete atilan nesne: " + sepetlenenNesneSayisi + " / " + hedefNesneSayisi);

        if (uiManager != null)
        {
            uiManager.NesneSayaciniGuncelle(sepetlenenNesneSayisi, hedefNesneSayisi);
        }

        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            Debug.Log("TEBRIKLER! Salondaki tum nesneler bitti. Kapi acildi!");
            if (kapiEngeli != null)
            {
                kapiEngeli.SetActive(false);
            }
        }

        // Elimizdeki nesne referans�n� temizliyoruz
        agizdakiNesne = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hiyerar�ideki nesne adlar�na g�re (NesneS, NesneK, NesneM) kedinin yakla�mas�na izin veriyoruz
        if ((other.gameObject.name.Contains("NesneS") ||
             other.gameObject.name.Contains("NesneK") ||
             other.gameObject.name.Contains("NesneM")) && agizdakiNesne == null)
        {
            yakindakiNesne = other.gameObject;
        }

        // Salon sepetlerinin kontrol�
        if (other.gameObject.name == "Salon_S_Sepet")
        {
            sepeteYakinMi = true;
            sepetTuru = "Sari";
            Debug.Log("Sari sepetin yanindasin.");
        }
        else if (other.gameObject.name == "Salon_K_Sepet")
        {
            sepeteYakinMi = true;
            sepetTuru = "Kirmizi";
            Debug.Log("Kirmizi sepetin yanindasin.");
        }
        else if (other.gameObject.name == "Salon_M_Sepet")
        {
            sepeteYakinMi = true;
            sepetTuru = "Mor";
            Debug.Log("Mor sepetin yanindasin.");
        }

        // --- KAPIDAN GEÇİŞ VE SKOR KONTROLÜ ---
    if (other.gameObject.name == "Yatak_Odasina_Gecis_Kapisi")
    {
        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            // Sahnede aktif olan UIManager'a ulaşıyoruz
            UIManager uiManager = FindFirstObjectByType<UIManager>();

            if (uiManager != null)
            {
                // Banyo barajı olan 50 puanı kontrol ediyoruz
                if (uiManager.mevcutSkor >= 65)
                {
                    Debug.Log("Tebrikler! Yeterli skorla Salon sahnesine geciliyor...");
                    SceneManager.LoadScene("AraSahne_3");
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
                SceneManager.LoadScene("AraSahne_3");
            }
        }
    }
    }

    private void OnTriggerExit(Collider other)
    {
        // 1. Nesneden uzakla��ld���nda referans� temizle
        if (other.gameObject == yakindakiNesne)
        {
            yakindakiNesne = null;
        }

        // 2. Sepetten uzakla��ld���nda durumu s�f�rla
        if (other.gameObject.name == "Salon_S_Sepet" ||
            other.gameObject.name == "Salon_K_Sepet" ||
            other.gameObject.name == "Salon_M_Sepet")
        {
            sepeteYakinMi = false;
            sepetTuru = "";
            Debug.Log("Sepetten uzaklasildi, durum sifirlandi.");
        }
    }
}