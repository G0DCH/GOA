using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMenu : MonoBehaviour
{
    public GameObject buttons;
	
	// Update is called once per frame
	void Update ()
    {
		if(Input.anyKeyDown)
        {
            buttons.SetActive(true);
            gameObject.SetActive(false);
        }
	}
}
