using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject ImpactEffectPrefab;
    GameObject ImpactEffect;
    
    // Destroy object and spawn effects when collide with something.
    void OnCollisionEnter(Collision obj)
    {
        if (obj.transform.CompareTag("Ground"))
        {
            ImpactEffect = Instantiate(ImpactEffectPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        }
        Destroy(transform.gameObject);
    }
}
