using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private GameObject camera;

    [SerializeField] private float cameraSpeed = 1f; 
    // Start is called before the first frame update
    void Start()
    {
        camera.transform.position = new Vector3(0f,0f,-1f);
    }

    // Update is called once per frame
    void Update()
    {
        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        transform.position += move * cameraSpeed * Time.deltaTime;
    }
}
