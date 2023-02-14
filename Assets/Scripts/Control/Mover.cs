using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    //You could inhterit more than 1 intrfc, whereas just 1 class
    public class Mover : MonoBehaviour,IAction 
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;


        NavMeshAgent navMeshAgent;
        Health health;

        void Start()
        {
            navMeshAgent= GetComponent<NavMeshAgent>();
            health= GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();//If player is dead then disable NMAgent.

            UpdateAnimator();
        }
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination,speedFraction);
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
        public void Cancel()//MUST BE THE SAME METHOD NAME LIKE IN INTERFACE
        {
            navMeshAgent.isStopped = true;
        }
        public void MoveTo(Vector3 destination,float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }
    }

}