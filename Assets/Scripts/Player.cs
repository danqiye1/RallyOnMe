using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float rotationSpeed = 100.0f;
    public float rotationSmoothTime = 0.1f;
    float rotationSmoothVelocity;

    Animator anim;

    public CharacterController controller;
    public Transform cam;

    int shootHash = Animator.StringToHash("Shoot");
    int deathHash = Animator.StringToHash("Death");

    // Shooting mechanics
    public GameObject BulletPrefab;
    public Vector3 BulletSpawnOffset; // Bullet spawn offset
    public float BulletSpeed;

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
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir.normalized * moveSpeed * Time.deltaTime);
        }

        // Shooting
        if (Input.GetKey(KeyCode.Space))
        {
            anim.SetBool(shootHash, true);
            GameObject Bullet = Instantiate(BulletPrefab, transform.position + BulletSpawnOffset, Quaternion.Euler(90,0,0)); // Spawn a bullet with an offset with 0 rotation
            Rigidbody rb = Bullet.GetComponent<Rigidbody>(); //Gets the rigidbody component from the bullet and stores it in rb
            rb.AddForce(transform.forward * BulletSpeed, ForceMode.Impulse); //Move the bullet forward
            // ShootSFXAudioSource.Play(); //Plays the shoot SFX
            Destroy(Bullet, 1f); //Destroy bullet in 1 second.
        }
        else
        {
            anim.SetBool(shootHash, false);
        }

    }
}
