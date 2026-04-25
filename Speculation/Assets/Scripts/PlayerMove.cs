using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMove : MonoBehaviour
{
    [Header("Hareket Ayarlarý")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.81f;

    [Header("Çömelme Ayarlarý")]
    public float normalHeight = 1.58f;
    public float crouchHeight = 1f;

    [Header("Referanslar")]
    public Animator anim; // Animasyonlarý buraya baðlayacaksýn

    private CharacterController controller;

    private float currentSpeed;
    private Vector3 velocity;

    void Start()
    {
        // Karakterin üzerindeki Character Controller'ý otomatik alýr
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 1. Yere Deðme ve Yerçekimi Sýfýrlama
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 2. Klavye Girdilerini Al (W, A, S, D)
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Kameranýn baktýðý yöne göre hareket vektörü oluþtur
        Vector3 move = transform.right * x + transform.forward * z;

        // 3. Durum Kontrolleri (Tuþlara basýlýyor mu?)
        bool isMoving = move.magnitude > 0.1f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);

        // 4. Hýz ve Boyut Ayarlamalarý (Çömelme/Koþma Mantýðý)
        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            controller.height = crouchHeight; // Karakterin kapsül boyunu kýsaltýr (siper almak için)
        }
        else if (isSprinting && isMoving)
        {
            currentSpeed = sprintSpeed;
            controller.height = normalHeight;
        }
        else
        {
            currentSpeed = walkSpeed;
            controller.height = normalHeight;
        }

        // 5. Fiziksel Hareketi Uygula
        controller.Move(move * currentSpeed * Time.deltaTime);

        // 6. Yerçekimini Uygula (Aþaðý düþme)
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 7. ANÝMASYON KISMI (Senin Ekleyeceðin Parametreler)
        if (anim != null)
        {
            anim.SetBool("isWalking", isMoving && !isSprinting && !isCrouching);
            anim.SetBool("isSprinting", isMoving && isSprinting && !isCrouching);
            anim.SetBool("isCrouching", isCrouching);
        }
    }
}