using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
        private void Start()
        {
            health = GetComponent<Health>();
        }
        void Update()
        {
            if (health.IsDead()) return;

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {   //CHECK WHETHER WE HIT THE COMBAT TARGET OR NOT.
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
               CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                
               if(!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;

                if(Input.GetMouseButtonDown(0))
                {//If target isn't null & clicked left MB then invoke attack.
                    GetComponent<Fighter>().Attack(target.gameObject); // for this particular target.
                }
                return true;
            }
            return false;
        }


        private bool InteractWithMovement()
        {
            //origin point of Raycasting.
            RaycastHit hit; //Info about where we have clicked.
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if(Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point,1f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
