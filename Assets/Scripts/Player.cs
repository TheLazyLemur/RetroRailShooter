using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [FormerlySerializedAs("_maxBoost")] public float maxBoost = 100;
    [FormerlySerializedAs("_remainingBoost")] public float remainingBoost;
    public static Action onDeath;
        
    public float forwardSpeed = 6;
    
    private void OnTriggerEnter(Collider other)
    {
        onDeath.Invoke();
        var rb = gameObject.GetComponent<Rigidbody>();
        var col = gameObject.GetComponent<BoxCollider>();
        col.isTrigger = false;
        rb.AddForce(new Vector3(50,50,50));
        rb.AddTorque(new Vector3(50, 50, 50));
        transform.parent = null;
    }
}