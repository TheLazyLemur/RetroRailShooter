using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Settings")] public bool joystick = true;
    [Space] [Header("Parameters")] public float xySpeed = 18;
    private PlayerMovement _playerMovement;
    public float lookSpeed = 340;
    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerMovement = GetComponent<PlayerMovement>();
        Player.OnDeath += () => Destroy(this);
    }

    private void Update()
    {
        var h = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
        var v = joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");

        _playerMovement.HorizontalLean(h, 80, .1f);

        _playerMovement.LocalMove(h, v, xySpeed);
        _playerMovement.RotationLook(h, v, lookSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && _player._remainingBoost > 0)
            _playerMovement.Boost(true);

        if (Input.GetKeyUp(KeyCode.Space))
            _playerMovement.Boost(false);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            _playerMovement.Break(true);

        if (Input.GetKeyUp(KeyCode.LeftShift))
            _playerMovement.Break(false);

        
        if (!Input.GetKeyDown(KeyCode.Z)) return;
        
        var rollDir = Input.GetAxis("Horizontal");
        
            
        if (rollDir < 0)
            _playerMovement.QuickSpin(-1);
        else
            _playerMovement.QuickSpin(1);
    }
}