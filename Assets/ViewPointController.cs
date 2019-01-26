using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPointController : MonoBehaviour
{
    public Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameObject playerObject = GameObject.FindWithTag( "Player" );
        gameObject.transform.position = playerObject.transform.position;
    }
}
