using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KediToplama : MonoBehaviour
{
    [Header("Ayarlar")]
    public Transform agizNoktasi;
    public int hedefNesneSayisi = 8;
    public GameObject kapiEngeli;

    [Header("Skor Sistemi")]
    public int puan = 0;
    public Text puanYazisiNesnesi;

    private GameObject agizdakiNesne = null;
    private GameObject yakindakiNesne = null;
    private bool sepeteYakinMi = false;
    private string sepetTuru = "";
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

        // --- NESNENIN GERI ALINMASINI ENGELLEME ---
        Collider nesneCollider = agizdakiNesne.GetComponent<Collider>();
        if (nesneCollider != null)
        {
            nesneCollider.enabled = false;
        }

        yakindakiNesne = null;

        // --- PUANLAMA VE MESAJ KONTROLU ---
        if ((kirmiziNesneMi && sepetTuru == "Kirmizi") || (maviNesneMi && sepetTuru == "Mavi"))
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

        if (other.gameObject.name == "Salona_Gecis_Kapisi")
        {
            if (sepetlenenNesneSayisi >= hedefNesneSayisi)
            {
                Debug.Log("Salon sahnesine geciliyor...");
                SceneManager.LoadScene("Salon_Sahnesi");
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