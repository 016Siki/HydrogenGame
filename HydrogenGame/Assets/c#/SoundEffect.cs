using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffect : MonoBehaviour
{
    [SerializeField] Button ButtonSund;
    // Start is called before the first frame update
    void Start()
    {
        if (ButtonSund != null)
        {
            ButtonSund.onClick.AddListener(pushButton);
        }
    }

    void pushButton()
    {
        GetComponent<AudioSource>().Play();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
