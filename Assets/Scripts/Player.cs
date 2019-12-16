using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float _maxBoost = 100;
    public float _remainingBoost;
    
    public static Action OnDeath;
        
    private void OnTriggerEnter(Collider other)
    {
        OnDeath.Invoke();
        var rb = gameObject.GetComponent<Rigidbody>();
        var col = gameObject.GetComponent<BoxCollider>();
        col.isTrigger = false;
        rb.AddForce(new Vector3(50,50,50));
        rb.AddTorque(new Vector3(50, 50, 50));
        transform.parent = null;
    }
}