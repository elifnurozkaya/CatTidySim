using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class YatakOdasiKediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 4; // 2 Yesil + 2 Pembe Nesne
    public GameObject kapiEngeli; // Yatak odasindan cikis kapisi engeli

    [Header("Skor Sistemi")]
    public int puan = 0;
    public Text puanYazisiNesnesi;

    [Header("Yatak Odasi Sepet Isimleri")]
    public string sepetY_Ismi = "Yatak_Odasi_Y_Sepet";
    public string sepetP_Ismi = "Yatak_Odasi_P_Sepet";

    private GameObject agizdakiNesne = null;
    private GameObject yakindakiNesne = null;
    private bool sepeteYakinMi = false;
    private string sepetTuru = ""; // "Yesil" veya "Pembe"
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
        // --- 1. KUP MEKANIGI (SADECE 'R' TUSU) ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            // Agiz bossa ve yakinda KUP varsa 'R' ile agza al
            if (agizdakiNesne == null && yakindakiNesne != null && yakindakiNesne.CompareTag("TasinabilirKup"))
            {
                NesneyiAgzaAl();
            }
            // Agizda KUP varsa 'R' ile yere sabitle (Bir daha alinamaz)
            else if (agizdakiNesne != null && agizdakiNesne.CompareTag("TasinabilirKup"))
            {
                KupuYereSabitle();
            }
        }

        // --- 2. NORMAL NESNE MEKANIGI (SADECE 'E' TUSU) ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Agiz bossa ve yakinda NORMAL nesne varsa 'E' ile agza al
            if (agizdakiNesne == null && yakindakiNesne != null)
            {
                if (yakindakiNesne.CompareTag("Yesil_Nesne") || yakindakiNesne.CompareTag("Pembe_Nesne"))
                {
                    NesneyiAgzaAl();
                }
            }
            // Agizda normal nesne varken sepete yakin Sakin 'E' ile sepete birak
            else if (agizdakiNesne != null && !agizdakiNesne.CompareTag("TasinabilirKup") && sepeteYakinMi)
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
        // Isim kontrolunu banyo mantigindaki gibi Contains ile yapiyoruz
        bool yesilNesneMi = agizdakiNesne.name.Contains("NesneY");
        bool pembeNesneMi = agizdakiNesne.name.Contains("NesneP");

        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Nesnenin bir daha geri alinmasini engelle
        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // --- PUANLAMA VE ESLESME KONTROLU ---
        if ((yesilNesneMi && sepetTuru == "Yesil") || (pembeNesneMi && sepetTuru == "Pembe"))
        {
            puan += 10;
            Debug.Log("Dogru sepet! +10 Puan. Toplam Puan: " + puan);
        }
        else
        {
            puan -= 5;
            Debug.LogWarning("Yanlis sepet! -5 Puan. Toplam Puan: " + puan);
        }

        if (puanYazisiNesnesi != null)
        {
            puanYazisiNesnesi.text = "Puan: " + puan;
        }

        sepetlenenNesneSayisi++;
        Debug.Log("Sepete atilan toplam nesne: " + sepetlenenNesneSayisi + " / " + hedefNesneSayisi);

        // Hedefe ulasildiysa kapi aciliyor
        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            Debug.Log("TEBRIKLER! Yatak odasi nesneleri bitti. Kapi engeli kaldirildi!");
            if (kapiEngeli != null)
            {
                kapiEngeli.SetActive(false);
            }
        }

        agizdakiNesne = null;
    }

    void KupuYereSabitle()
    {
        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Fizigini dondur, havada/yerde çivi gibi sabit kalsin
        }

        agizdakiNesne.tag = "Untagged"; // Etiketini bosa cikar ki kedi bir daha alamasin

        Debug.Log("Kup yere sabitlendi ve kilitlendi: " + agizdakiNesne.name);

        agizdakiNesne = null;
        yakindakiNesne = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Alinabilir nesneleri algila
        if (agizdakiNesne == null)
        {
            if (other.CompareTag("Yesil_Nesne") || other.CompareTag("Pembe_Nesne") || other.CompareTag("TasinabilirKup"))
            {
                yakindakiNesne = other.gameObject;
            }
        }

        // Sepetleri banyo mantigina gore algila
        if (other.gameObject.name == sepetY_Ismi)
        {
            sepeteYakinMi = true;
            sepetTuru = "Yesil";
        }
        else if (other.gameObject.name == sepetP_Ismi)
        {
            sepeteYakinMi = true;
            sepetTuru = "Pembe";
        }

        // Sonraki sahneye gecis tetikleyicisi (Eger kapidan geciliyorsa ve oyun bittiyse)
        if (other.gameObject.name == "Ana_Menu_Gecis_Kapisi" || other.gameObject.name == "Sonraki_Seviye_Kapisi")
        {
            if (sepetlenenNesneSayisi >= hedefNesneSayisi)
            {
                Debug.Log("Sonraki sahneye geciliyor...");
                // Buraya gecmek istedigin sahnenin adini yazabilirsin
                // SceneManager.LoadScene("AnaMenu"); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == yakindakiNesne)
        {
            yakindakiNesne = null;
        }

        if (other.gameObject.name == sepetY_Ismi || other.gameObject.name == sepetP_Ismi)
        {
            sepeteYakinMi = false;
            sepetTuru = "";
        }
    }
}