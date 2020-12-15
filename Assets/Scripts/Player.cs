using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 100.0f;
    public float rotationSmoothTime = 0.1f;
    float rotationSmoothVelocity;
    Vector3 moveDir;

    Animator anim;

    public CharacterController controller;
    public Transform cam;

    int shootHash = Animator.StringToHash("Shoot");
    int deathHash = Animator.StringToHash("Death");

    // Shooting mechanics
    public GameObject BulletPrefab;
    public Vector3 BulletSpawnOffset; // Bullet spawn offset
    public float BulletSpeed;
    public Transform BulletTransform;
    public GameObject MuzzlePrefab;
    public Transform MuzzleTransform;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        // Walking
        anim.SetFloat("Speed", direction.magnitude);

        // Sprinting
        if (Input.GetKey("left shift") && direction.magnitude >= 0.01f)
        {
            anim.SetBool("Sprint", true);
            moveSpeed = 5.0f;
        }
        else
        {
            anim.SetBool("Sprint", false);
            moveSpeed = 2.0f;
        }

        // Death
        if (Input.GetKey("mouse 1"))
        {
            anim.SetInteger(deathHash, Random.Range(1, 4));
        }

        if (direction.magnitude >= 0.1f)
        {
            // Player Rotation.
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            // Player movement
            moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // Shooting on left click
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool(shootHash, true);
            // Spawn a bullet with an offset
            GameObject Bullet = Instantiate(BulletPrefab, BulletTransform.position, BulletTransform.rotation * Quaternion.Euler(90, 0, 0));
            // Spawn a muzzle flash
            GameObject MuzzleFlash = Instantiate(MuzzlePrefab, MuzzleTransform.position, MuzzleTransform.rotation);
            //Gets the rigidbody component from the bullet and stores it in rb
            Rigidbody rb = Bullet.GetComponent<Rigidbody>();
            //Move the bullet forward
            rb.AddForce(transform.forward * BulletSpeed, ForceMode.Impulse); 
            // ShootSFXAudioSource.Play(); //Plays the shoot SFX
            Destroy(Bullet, 1f); //Destroy bullet in 1 second. To Do: Destroy bullet if hit somthing
            Destroy(MuzzleFlash, 0.2f);
        }
        else
        {
            anim.SetBool(shootHash, false);
        }

    }
}
