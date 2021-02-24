using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    public float swayAmount;
    public float swayAmountMax;
    [Range (0,1)]public float smoothing;

    public Vector3 offsetMove;
    private Vector3 initialPosition;
    Actor_Player aP => LevelManager.Instance.Player;
   
    void Start()
    {
        initialPosition = transform.localPosition;

       
    }

   
    void Update()
    {

        //float movementX = -Input.GetAxis("Mouse X") * swayAmount;
        float movementX = aP.playerInputs.actions["Look"].ReadValue<Vector2>().x *swayAmount;
        float movementY = aP.playerInputs.actions["Look"].ReadValue<Vector2>().y *swayAmount;
        //float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
        movementX = Mathf.Clamp(movementX, -swayAmountMax, swayAmountMax);
        movementY = Mathf.Clamp(movementY, -swayAmountMax, swayAmountMax);

        Vector3 finalPosition = new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, smoothing);
    }
}
