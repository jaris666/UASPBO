using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera Setup")]
    public Camera fpsCamera;
    public Camera tpsCamera;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float crouchSpeed = 3f;
    public float jumpPower = 7f;
    public float gravity = 20f;

    [Header("Look Settings")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Heights")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    // Private Variables
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private Animator animator;

    private bool isFpsMode = true;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Kunci Cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Setup Awal Kamera
        isFpsMode = true;
        if (fpsCamera != null) fpsCamera.gameObject.SetActive(true);
        if (tpsCamera != null) tpsCamera.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. SWITCH KAMERA (Tombol M)
        if (Input.GetKeyDown(KeyCode.M))
        {
            isFpsMode = !isFpsMode;
            if (fpsCamera != null) fpsCamera.gameObject.SetActive(isFpsMode);
            if (tpsCamera != null) tpsCamera.gameObject.SetActive(!isFpsMode);
        }

        // 2. DETEKSI INPUT ARAH
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Cek apakah tombol Shift ditekan?
        bool isRunningInput = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunningInput ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunningInput ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;

        // Crouch Logic (Tombol R)
        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            curSpeedX = crouchSpeed * Input.GetAxis("Vertical");
            curSpeedY = crouchSpeed * Input.GetAxis("Horizontal");
        }
        else
        {
            characterController.height = defaultHeight;
        }

        // --- SIMPAN GRAVITASI SEBELUM DI-OVERWRITE ---
        float movementDirectionY = moveDirection.y;

        // Hitung gerak maju/samping
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // BALIKAN LAGI KECEPATAN JATUHNYA
        moveDirection.y = movementDirectionY;

        // 3. ANIMASI (SUDAH DIUPDATE UNTUK LARI)
        bool isMoving = (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f);

        if (animator != null)
        {
            // Kirim sinyal Jalan
            animator.SetBool("isWalking", isMoving);

            // Kirim sinyal Lari
            // Syarat Lari: Harus sedang bergerak (isMoving) DAN tombol Shift ditekan
            animator.SetBool("isRunning", isMoving && isRunningInput);
        }

        // 4. LOMPAT & GRAVITASI
        if (characterController.isGrounded)
        {
            moveDirection.y = -1f; // Tekan ke tanah

            if (Input.GetButton("Jump") && canMove)
            {
                moveDirection.y = jumpPower;
                if (animator != null) animator.SetTrigger("Jump");
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Eksekusi Gerakan
        characterController.Move(moveDirection * Time.deltaTime);

        // 5. ROTASI KAMERA
        if (canMove)
        {
            transform.Rotate(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            if (isFpsMode && fpsCamera != null)
            {
                fpsCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            }
            else if (!isFpsMode && tpsCamera != null)
            {
                tpsCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            }
        }
    }
}