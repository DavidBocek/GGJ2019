using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Gate>().GateClose();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
