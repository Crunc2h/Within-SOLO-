using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerController", menuName = "InputController/PlayerController")]
public class PlayerController : InputController
{
    public override bool RetreiveRollInput() => Input.GetKeyDown(KeyCode.LeftShift);
    public override bool RetreiveJumpInput() => Input.GetKeyDown(KeyCode.Space);
    public override float RetreiveMoveInput() => Input.GetAxisRaw("Horizontal");
    public override bool RetreiveJumpInputRelease() => Input.GetKeyUp(KeyCode.Space);

    public bool RetreiveLightAttackInput() => Input.GetKeyDown(KeyCode.Mouse0);
    public bool RetreiveHeavyAttackInput() => Input.GetKeyDown(KeyCode.Mouse1);

}
