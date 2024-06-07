using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AIController", menuName = "InputController/AIController")]
public class AIController : InputController
{
    public override bool RetreiveJumpInput() => true;
    public override float RetreiveMoveInput() => 1f;
    public override bool RetreiveRollInput() => true;
    public override bool RetreiveJumpInputRelease() => true;
}
