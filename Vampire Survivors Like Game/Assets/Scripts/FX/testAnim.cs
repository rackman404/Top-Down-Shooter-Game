using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testAnim : MonoBehaviour
{

    Animator test;

    // Start is called before the first frame update
    void Start()
    {
        //test =  gameObject.GetComponent<Animator>();
        //test.Play("Entry");
    }

    public void DeleteSelf(){
        Destroy(gameObject);
    }

    void Update(){
        
    }


}
