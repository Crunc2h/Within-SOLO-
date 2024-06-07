using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   
    // Start is called before the first frame update
    

    public void ApplyAttack(PlayerAttackUnit attack)
    {
        var attackDamage = attack.GetAttackDamage();
        var hitBox = attack.GetHitBox();


        RaycastHit2D[] hitEnemies = Physics2D.BoxCastAll(transform.position, hitBox, 0, Vector2.right, 2f, LayerMask.GetMask("enemyLayer"));
        Debug.Log(hitEnemies.Count());
    }

    
}
