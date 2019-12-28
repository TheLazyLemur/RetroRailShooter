using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject deathCam;

    private void Awake()
    {
        Player.onDeath += EnableDeathCam;
    }

    private void EnableDeathCam()
    {
        deathCam.SetActive(true);
    }
}
