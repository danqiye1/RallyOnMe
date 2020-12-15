using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Friendly : MonoBehaviour
{
    public GameObject Officer;
    public float FollowSpeed;
    public float PanicDistance;

    Animator anim;

    int PanicHash = Animator.StringToHash("Panic");
    int SpeedHash = Animator.StringToHash("Speed");
    bool isFollowing = false;


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Hacky way to calculate Player's speed
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector3 direction = new Vector3(horizontal, 0, vertical);

        Quaternion lookDir;

        float PlayerDistance = Vector3.Distance(Officer.transform.position, transform.position);
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

            // Move the player
            Vector3 TargetPosition = Officer.transform.position + new Vector3(2,0,-2);
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, FollowSpeed * Time.deltaTime);
            
            // Rotation to look at where he is moving to.
            if (transform.position != TargetPosition){
                lookDir = Quaternion.LookRotation(TargetPosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, Time.deltaTime);
            }
            
        }
    }
}
