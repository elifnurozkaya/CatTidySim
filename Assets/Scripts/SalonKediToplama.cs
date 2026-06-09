using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SalonKediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 9; // Hiyerarþide toplam 9 nesne (3 Sari, 3 Kirmizi, 3 Mor) 
    public GameObject kapiEngeli;

    [Header("Skor Sistemi")]
    // UIManager deðiþkenini sýnýfýn ÝÇÝNE aldýk ve eski deðiþkenleri tamamen temizledik.
    public UIManager uiManager;

    private GameObject agizdakiNesne = null;
    private GameObject yakindakiNesne = null;
    private bool sepeteYakinMi = false;
    private string sepetTuru = ""; // "Sari", "Kirmizi" veya "Mor" olacak
    private int sepetlenenNesneSayisi = 0;

    void Start()
    {
        // Eski text atamasý silindi, sadece kapý kontrolü kaldý.
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
        // 1. Nesne renklerini isim kontrolü ile ayýrt ediyoruz
        bool sariNesneMi = agizdakiNesne.name.Contains("NesneS");
        bool kirmiziNesneMi = agizdakiNesne.name.Contains("NesneK");
        bool morNesneMi = agizdakiNesne.name.Contains("NesneM");

        // 2. Nesneyi aðýzdan fiziksel olarak býrakma iþlemleri
        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Fiziksel olarak kedi tarafýndan tekrar hemen alýnmasýný engellemek için collider'ý kapatýyoruz
        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // 3. TEK VE NET EÞLEÞME KONTROLÜ (UIManager Entegrasyonlu)
        if ((sariNesneMi && sepetTuru == "Sari") ||
            (kirmiziNesneMi && sepetTuru == "Kirmizi") ||
            (morNesneMi && sepetTuru == "Mor"))
        {
            // DOÐRU EÞLEÞME
            if (uiManager != null)
            {
                uiManager.SkorEkle(10); // UI ekranýndaki skoru 10 artýrýr
            }
            Debug.Log("Salon - Dogru sepet! +10 Puan eklendi.");
        }
        else
        {
            // YANLIÞ EÞLEÞME
            if (uiManager != null)
            {
                uiManager.SkorEkle(-5); // UI ekranýndaki skoru 5 azaltýr
            }
            Debug.LogWarning("Salon - Yanlis sepet! -5 Puan dusuldu.");
        }

        // 4. Hedef nesne ve kapý kontrolü
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

        // Elimizdeki nesne referansýný temizliyoruz
        agizdakiNesne = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hiyerarþideki nesne adlarýna göre (NesneS, NesneK, NesneM) kedinin yaklaþmasýna izin veriyoruz
        if ((other.gameObject.name.Contains("NesneS") ||
             other.gameObject.name.Contains("NesneK") ||
             other.gameObject.name.Contains("NesneM")) && agizdakiNesne == null)
        {
            yakindakiNesne = other.gameObject;
        }

        // Salon sepetlerinin kontrolü
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

        // Kapý kontrolü
        if (other.gameObject.name == "Yatak_Odasina_Gecis_Kapisi")
        {
            if (sepetlenenNesneSayisi >= hedefNesneSayisi)
            {
                Debug.Log("Yatak odasi sahnesine geciliyor...");
                SceneManager.LoadScene("AraSahne_3");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 1. Nesneden uzaklaþýldýðýnda referansý temizle
        if (other.gameObject == yakindakiNesne)
        {
            yakindakiNesne = null;
        }

        // 2. Sepetten uzaklaþýldýðýnda durumu sýfýrla
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