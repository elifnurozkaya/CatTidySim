using UnityEngine;

public class KediHareket : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 moveDirection;
    private float horizontal;
    private float vertical;

    private Animator animator;

    [Header("Hareket Ayarlari")]
    public float moveSpeed = 2f;
    public float mouseSensitivity = 1.5f;
    public float jumpForce = 6f;

    [Header("Kamera Duvar Ayarlari")]
    public LayerMask duvarKatmani; // Inspector'dan duvarlarin katmanini secmek icin
    private float maxKameraMesafesi = 1.0f; // Kameranin kediye olan normal uzakligi
    private float kameraYüksekligi = 0.6f; // Kameranin yuksekligi

    private float velocityY = 0f;
    private float gravity = -18f;

    private float xRotation = 0f;
    private Transform cameraTransform;

    private bool isGroundedLaser = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        cameraTransform = Camera.main.transform;
        cameraTransform.parent = this.transform;

        // Ilk acilista kamerayi normal yerine koyuyoruz
        cameraTransform.localPosition = new Vector3(0, kameraYüksekligi, -maxKameraMesafesi);
        cameraTransform.localRotation = Quaternion.identity;

        Cursor.lockState = CursorLockMode.Locked;

        // Eger Inspector'dan katman secilmediyse, varsayilan olarak her seyle (Default) carpisabilir yapalim
        if (duvarKatmani == 0)
        {
            duvarKatmani = ~0; // Tum katmanlari secer
        }
    }

    void Update()
    {
        // 1. MOUSE LOOK
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -45f, 60f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 2. WASD INPUTS
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputDirection = right * horizontal + forward * vertical;
        moveDirection = inputDirection * moveSpeed;

        // 3. LAZER KONTROLU
        Vector3 rayStartPoint = transform.position + controller.center + (Vector3.down * (controller.height / 2f));
        float rayLength = 0.1f;
        isGroundedLaser = Physics.Raycast(rayStartPoint, Vector3.down, rayLength);

        // 4. GRAVITY & JUMP 
        if (isGroundedLaser)
        {
            if (velocityY < 0)
            {
                velocityY = -2f;
            }

            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
                velocityY = jumpForce;
            }
        }
        else
        {
            velocityY += gravity * Time.deltaTime;
        }

        moveDirection.y = velocityY;
        controller.Move(moveDirection * Time.deltaTime);

        // 5. ANIMATION CONTROL
        if (animator != null)
        {
            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                animator.SetBool("Walk", true);
            }
            else
            {
                animator.SetBool("Walk", false);
            }
        }

        // 6. KAMERA DUVARA CARPMA KONTROLU (YENI)
        KameraDuvarKontrolu();
    }

    void KameraDuvarKontrolu()
    {
        // Kameranin durmasi gereken ideal dunya pozisyonunu hesapla
        Vector3 idealKameraPozisyonu = transform.TransformPoint(new Vector3(0, kameraYüksekligi, -maxKameraMesafesi));

        // Kedinin boyun hizasindan baslayan bir nokta (Iyilestirilmis donus noktasi)
        Vector3 kediBaslangicNoktasi = transform.position + Vector3.up * kameraYüksekligi;

        RaycastHit hit;
        float gecerliMesafe = maxKameraMesafesi;

        // Kediden kameraya dogru bir gorunmez cizgi (Ray) gonderiyoruz
        Vector3 yon = idealKameraPozisyonu - kediBaslangicNoktasi;
        if (Physics.Raycast(kediBaslangicNoktasi, yon.normalized, out hit, maxKameraMesafesi, duvarKatmani))
        {
            // Eger arada bir duvar varsa, kamerayi duvarin carptigi noktaya yaklastir (0.2f kadar biraz bosluk birakarak)
            gecerliMesafe = hit.distance - 0.2f;
            if (gecerliMesafe < 0.3f) gecerliMesafe = 0.3f; // Kameranin kedinin cok icine girmesini engelle
        }

        // Kameranin yeni pozisyonunu yumusakca veya direkt uygula
        cameraTransform.localPosition = new Vector3(0, kameraYüksekligi, -gecerliMesafe);
    }
}