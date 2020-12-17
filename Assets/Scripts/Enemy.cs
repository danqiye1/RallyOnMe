using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator anim;
    int shootHash = Animator.StringToHash("Shooting");
    int deathHash = Animator.StringToHash("Death");

    public float DetectionRange;
    // Angle at which to start shooting
    public float ArcOfFire;

    // Shooting mechanics
    public GameObject BulletPrefab;
    public float BulletSpeed;
    public Transform BulletTransform;
    public GameObject MuzzlePrefab;
    public Transform MuzzleTransform;
    public float FireRate = 1f;

    float LastFire;
    public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        LastFire = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Detect closest soldier.
        GameObject Target; 
        Target = FindClosestFriendly();

        // Aim at the closest soldier if they are within range
        float dist = (Target.transform.position - transform.position).sqrMagnitude;
        
        Quaternion aimDir;
        float angle;
        // Rotation to aim at player or friendly.
        if (dist <= DetectionRange && isDead == false){
            aimDir = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, aimDir, Time.deltaTime);

            // Shoot if player or friendly is within Arc of fire and rate of fire
            angle = Quaternion.Angle(transform.rotation, aimDir);
            if(angle <= ArcOfFire && Time.time - LastFire >= FireRate)
            {
                //Shoot
                anim.SetBool(shootHash, true);
                // Spawn a bullet with an offset
                GameObject Bullet = Instantiate(BulletPrefab, BulletTransform.position, BulletTransform.rotation * Quaternion.Euler(90, 0, 0));
                Bullet.tag = "BulletEnemy";
                // Spawn a muzzle flash
                GameObject MuzzleFlash = Instantiate(MuzzlePrefab, MuzzleTransform.position, MuzzleTransform.rotation);
                //Gets the rigidbody component from the bullet and stores it in rb
                Rigidbody rb = Bullet.GetComponent<Rigidbody>();
                //Move the bullet forward
                rb.AddForce(transform.forward * BulletSpeed, ForceMode.Impulse); 
                // ShootSFXAudioSource.Play(); //Plays the shoot SFX
                Destroy(Bullet, 1f); //Destroy bullet in 1 second. To Do: Destroy bullet if hit somthing
                Destroy(MuzzleFlash, 0.2f);
                LastFire = Time.time;
            }
        }
    }

    GameObject FindClosestFriendly()
    {
        float minDist = Mathf.Infinity;
        GameObject closestFriendly = null;
        GameObject[] allFriendlies = GameObject.FindGameObjectsWithTag("Friendly");

        foreach( GameObject currentFriendly in allFriendlies) {
            float dist = (currentFriendly.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist) {
                minDist = dist;
                closestFriendly = currentFriendly;
            }
        }

        return closestFriendly;

    }

    // Death from hitting a bullet
    void OnCollisionEnter(Collision collision){

        if(collision.transform.CompareTag("BulletFriendly"))
        {
            anim.SetInteger(deathHash, Random.Range(1, 4));
            isDead = true;
        }
    }
}
