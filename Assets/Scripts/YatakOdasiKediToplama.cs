using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class YatakOdasiKediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 4; // 2 Yesil + 2 Pembe Nesne
    public GameObject kapiEngeli; // Eðer geriye dönüþü engellemek istersen diye kalabilir

    [Header("Skor Sistemi")]
    public UIManager uiManager;

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
        // Baþlangýçta geriye dönük kapýyý kapatmak istiyorsan aktif kalýr
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
        // --- 1. KUP MEKANIGI (SADECE 'R' TUSU) ---
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (agizdakiNesne == null && yakindakiNesne != null && yakindakiNesne.CompareTag("TasinabilirKup"))
            {
                NesneyiAgzaAl();
            }
            else if (agizdakiNesne != null && agizdakiNesne.CompareTag("TasinabilirKup"))
            {
                KupuYereSabitle();
            }
        }

        // --- 2. NORMAL NESNE MEKANIGI (SADECE 'E' TUSU) ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (agizdakiNesne == null && yakindakiNesne != null)
            {
                if (yakindakiNesne.CompareTag("Yesil_Nesne") || yakindakiNesne.CompareTag("Pembe_Nesne"))
                {
                    NesneyiAgzaAl();
                }
            }
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
        bool yesilNesneMi = agizdakiNesne.name.Contains("NesneY");
        bool pembeNesneMi = agizdakiNesne.name.Contains("NesneP");

        agizdakiNesne.transform.SetParent(null);

        Rigidbody rb = agizdakiNesne.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // --- PUANLAMA KONTROLU ---
        if ((yesilNesneMi && sepetTuru == "Yesil") || (pembeNesneMi && sepetTuru == "Pembe"))
        {
            if (uiManager != null) uiManager.SkorEkle(10);
            Debug.Log("Yatak Odasi - Dogru sepet! +10 Puan eklendi.");
        }
        else
        {
            if (uiManager != null) uiManager.SkorEkle(-5);
            Debug.LogWarning("Yatak Odasi - Yanlis sepet! -5 Puan dusuldu.");
        }

        sepetlenenNesneSayisi++;
        Debug.Log("Sepete atilan toplam nesne: " + sepetlenenNesneSayisi + " / " + hedefNesneSayisi);

        if (uiManager != null)
        {
            uiManager.NesneSayaciniGuncelle(sepetlenenNesneSayisi, hedefNesneSayisi);
        }

        // --- DEÐÝÞEN ANA KISIM (KAPI YERÝNE DÝREKT KAZANMA) ---
        if (sepetlenenNesneSayisi >= hedefNesneSayisi)
        {
            Debug.Log("TEBRÝKLER! Tüm nesneler toplandý, oyun kazanýldý!");

            if (uiManager != null)
            {
                uiManager.OyunuKazan(); // Son nesne sepete girdiði saniye Win Paneli açýlýr
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
            rb.isKinematic = true;
        }

        agizdakiNesne.tag = "Untagged";
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

        // Sepetleri algila
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

        // Kapý algýlama kodlarý, artýk kapýya ihtiyacýmýz olmadýðý için silindi!
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