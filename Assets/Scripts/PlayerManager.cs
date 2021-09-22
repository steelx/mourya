using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler inputHandler;
    CharacterLocomotion locomotion;

    // Start is called before the first frame update
    void Start()
    {
        inputHandler = GetComponent<InputHandler>();
        locomotion = GetComponent<CharacterLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {   
        inputHandler.TickInput(Time.deltaTime);
        locomotion.CharacterMovement(Time.deltaTime);
        locomotion.HandleRollingAndSprint(Time.deltaTime);
        //locomotion.HandleFallingAndLanding(Time.deltaTime);
        //locomotion.CharacterFallingAndLanding(Time.deltaTime);
    }

    void LateUpdate()
    {
        inputHandler.rollFlag = false;
        inputHandler.sprintFlag = false;
        locomotion.UpdateInAirTimer(Time.deltaTime);
    }
}
