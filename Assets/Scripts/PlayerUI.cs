using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text _text;
    private float max;
    private float current;

    public Image img;
    
    private void Update()
    {
        //Scriptable object architecture here
        var player = FindObjectOfType<Player>();
        max = player.maxBoost;
        current = player.remainingBoost;
        var percent = current / max * 100;
        var res = percent / 100;
        _text.text = res.ToString();

        img.transform.localScale = new Vector3(img.gameObject.transform.localScale.x, res, img.gameObject.transform.localScale.z);
    }
}
