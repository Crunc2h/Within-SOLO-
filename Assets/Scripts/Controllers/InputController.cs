using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract float RetreiveMoveInput();
    public abstract bool RetreiveJumpInput();
    public abstract bool RetreiveRollInput();
    public abstract bool RetreiveJumpInputRelease();
}
