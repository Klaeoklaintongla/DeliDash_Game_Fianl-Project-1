using UnityEngine;
using UnityEngine.InputSystem;

public class Driver : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;   
    [SerializeField] float steerSpeed = 200f;  

    void Update()
    {
        float move = 0f;
        float steer = 0f;

       
        if (Keyboard.current.wKey.isPressed)
            move = 1f;
        else if (Keyboard.current.sKey.isPressed)
            move = -1f;

       
        if (Keyboard.current.aKey.isPressed)
            steer = 1f;
        else if (Keyboard.current.dKey.isPressed)
            steer = -1f;

        
        transform.Rotate(0f, 0f, steer * steerSpeed * Time.deltaTime);
        transform.Translate(0f, move * moveSpeed * Time.deltaTime, 0f);
    }
}
