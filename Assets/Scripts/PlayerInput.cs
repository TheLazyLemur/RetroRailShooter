using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerInput : SerializedMonoBehaviour
{
    [Space] [Header("Parameters")] public float xySpeed = 18;
    private PlayerMovement _playerMovement;
    public float lookSpeed = 340;
    private Player _player;

    [FormerlySerializedAs("boost")] public Action<bool> boostEvent;
    [FormerlySerializedAs("boost")] public Action<bool> breakEvent;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _playerMovement = GetComponent<PlayerMovement>();
        Player.onDeath += () => Destroy(this);
    }

    private void Update()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        _playerMovement.HorizontalLean(h, 80, .1f);
        _playerMovement.LocalMove(h, v, xySpeed);
        _playerMovement.RotationLook(h, v, lookSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && _player.remainingBoost > 0)
            boostEvent?.Invoke(true);

        if (Input.GetKeyUp(KeyCode.Space))
            boostEvent?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.LeftShift))
            breakEvent?.Invoke(true);

        if (Input.GetKeyUp(KeyCode.LeftShift))
            breakEvent?.Invoke(false);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var rollDir = Input.GetAxis("Horizontal");

            if (rollDir < 0)
                _playerMovement.QuickSpin(-1);
            else
                _playerMovement.QuickSpin(1);
        }
    }
}