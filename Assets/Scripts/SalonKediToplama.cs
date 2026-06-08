using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SalonKediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 9; // Hiyarside toplam 9 nesne (3 Sari, 3 Kirmizi, 3 Mor) gordugum icin 9 yaptim
    public GameObject kapiEngeli;

    [Header("Skor Sistemi")]
    public int puan = 0;
    public Text puanYazisiNesnesi;

    private GameObject agizdakiNesne = null;
    private GameObject yakindakiNesne = null;
    private bool sepeteYakinMi = false;
    private string sepetTuru = ""; // "Sari", "Kirmizi" veya "Mor" olacak
    private int sepetlenenNesneSayisi = 0;

    void Start()
    {
        if (puanYazisiNesnesi != null)
        {
            puanYazisiNesnesi.text = "Puan: " + puan;
        }

        if (kapiEngeli != null)
        {
            kapiEngeli.SetActive(true);
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
        // Nesne isimlerine gore hangi renk oldugunu tam kelimeyle ayirt ediyoruz
        bool sariNesneMi = agizdakiNesne.name.Contains("NesneS");
        bool kirmiziNesneMi = agizdakiNesne.name.Contains("NesneK");
        bool morNesneMi = agizdakiNesne.name.Contains("NesneM"); // Mor nesnelerin hiyarsideki adi NesneM oldugu icin boyle biraktik

        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Fiziksel olarak geri alinmasini engelleme
        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // 3'lu yeni sepet eslesme kontrolu
        if ((sariNesneMi && sepetTuru == "Sari") ||
            (kirmiziNesneMi && sepetTuru == "Kirmizi") ||
            (morNesneMi && sepetTuru == "Mor"))
        {
            puan += 10;
            Debug.Log("Salon - Dogru sepet! +10 Puan. Toplam Puan: " + puan);
        }
        else
        {
            puan -= 5;
            Debug.LogWarning("Salon - Yanlis sepet! -5 Puan. Toplam Puan: " + puan);
        }

        if (puanYazisiNesnesi != null)
        {
            puanYazisiNesnesi.text = "Puan: " + puan;
        }

        sepetlenenNesneSayisi++;
        Debug.Log("Salonda sepete atilan nesne: " + sepetlenenNesneSayisi + " / " + hedefNesneSayisi);

        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            Debug.Log("TEBRIKLER! Salondaki tum nesneler bitti. Kapi acildi!");
            if (kapiEngeli != null)
            {
                kapiEngeli.SetActive(false);
            }
        }

        agizdakiNesne = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hiyarsideki nesne adlarina gore (NesneS, NesneK, NesneM) kedinin yaklasmasina izin veriyoruz
        if ((other.gameObject.name.Contains("NesneS") ||
             other.gameObject.name.Contains("NesneK") ||
             other.gameObject.name.Contains("NesneM")) && agizdakiNesne == null)
        {
            yakindakiNesne = other.gameObject;
        }

        // Yeni ekledigin 3 farkli salon sepetinin kontrolu
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
            sepetTuru = "Mor"; // Kod icinde karismamasi icin sepet turunu direkt Mor olarak isaretledik
            Debug.Log("Mor sepetin yanindasin.");
        }

        if (other.gameObject.name == "Yatak_Odasina_Gecis_Kapisi")
        {
            if (sepetlenenNesneSayisi >= hedefNesneSayisi)
            {
                Debug.Log("Yatak odasi sahnesine geciliyor...");
                SceneManager.LoadScene("Yatak_Odasi_Sahnesi");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == yakindakiNesne)
        {
            yakindakiNesne = null;
        }

        if (other.gameObject.name == "Salon_S_Sepet" ||
            other.gameObject.name == "Salon_K_Sepet" ||
            other.gameObject.name == "Salon_M_Sepet")
        {
            sepeteYakinMi = false;
            sepetTuru = "";
            Debug.Log("Sepetten uzaklastin.");
        }
    }
}