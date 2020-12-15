using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendly : MonoBehaviour
{
    public GameObject Officer;
    public float FollowSpeed;
    public float PanicDistance;
    public float ArcOfFire;

    Animator anim;

    int PanicHash = Animator.StringToHash("Panic");
    int SpeedHash = Animator.StringToHash("Speed");
    int deathHash = Animator.StringToHash("Death");
    int shootHash = Animator.StringToHash("Shoot");

    bool isFollowing = false;

    bool isDead = false;

    // Shooting mechanics
    public GameObject BulletPrefab;
    public float BulletSpeed;
    public Transform BulletTransform;
    public GameObject MuzzlePrefab;
    public Transform MuzzleTransform;
    public float FireRate = 1f;
    float LastFire;


    void Start()
    {
        anim = GetComponent<Animator>();
        LastFire = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Hacky way to calculate Player's speed
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        Quaternion lookDir;
        float angle;
        Quaternion aimDir;

        float PlayerDistance = Vector3.Distance(Officer.transform.position, transform.position);

        if (isDead == false){

            if (PlayerDistance > PanicDistance && isFollowing == false)
            {
                // If player is not nearby, panic!
                anim.SetBool(PanicHash, true);
            }
            else
            {
                // Start following player.
                // The animatior controller will ensure the animation will no longer become panicked.
                anim.SetBool(PanicHash, false);
                isFollowing = true;

                // Match follow speed with player
                FollowSpeed = Officer.GetComponent<Player>().moveSpeed;
                anim.SetFloat(SpeedHash, FollowSpeed * direction.magnitude);

                // Move the ally
                Vector3 TargetPosition = Officer.transform.position + new Vector3(2,0,-2);
                transform.position = Vector3.MoveTowards(transform.position, TargetPosition, FollowSpeed * Time.deltaTime);
                
                // Sense the closest enemy
                GameObject Target;
                Target = FindClosestEnemy();

                // Rotation to look at where he is moving to. Else look to the closest enemy
                if (transform.position != TargetPosition){
                    lookDir = Quaternion.LookRotation(TargetPosition - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime);
                }
                else
                {
                    lookDir = Quaternion.LookRotation(Target.transform.position - transform.position);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime);
                }

                

                // Shoot if enemy or Arc of fire and rate of fire
                aimDir = Quaternion.LookRotation(Target.transform.position - transform.position);
                angle = Quaternion.Angle(transform.rotation, aimDir);
                if(angle <= ArcOfFire && Time.time - LastFire >= FireRate)
                {
                    //Shoot
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
                    LastFire = Time.time;
                }
                else{
                    anim.SetBool(shootHash, false);
                }
                
            }
        }
    }

    GameObject FindClosestEnemy()
    {
        float minDist = Mathf.Infinity;
        GameObject closestEnemy = null;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach( GameObject currentEnemy in allEnemies) {
            float dist = (currentEnemy.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist) {
                minDist = dist;
                closestEnemy = currentEnemy;
            }
        }

        return closestEnemy;

    }

    // Death from hitting a bullet
    void OnCollisionEnter(Collision collision){

        if(collision.transform.CompareTag("Bullet"))
        {
            anim.SetInteger(deathHash, Random.Range(1, 4));
            isDead = true;
        }
    }
}
