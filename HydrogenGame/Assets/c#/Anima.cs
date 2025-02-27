using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anima : MonoBehaviour
{
    Animator animator;
    public GameObject TITLEButton;
    public GameObject AnimaObject;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        AnimaObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void DropElement()
    {
        AnimaObject.SetActive(true);
        animator.SetTrigger("rakka"); 
    }
}
