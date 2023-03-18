using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;
     
   
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxPathLength = 40f;

        private void Awake()
        {
            health = GetComponent<Health>();
        }
        void Update()
        {
            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);//If nothing happens set cursor to none.
        }

       

        private bool InteractWithUI()
        {
             if(EventSystem.current.IsPointerOverGameObject())//check if you hover 
             {//if you hover over to UI then set Cursor type and return true.
                SetCursor(CursorType.UI);
                return true;
             }
            return false;//If you dont hover UI return false.
        }

        public bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {   //Get all the Raycastables.
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables) //Go into raycastables.
                {
                    if(raycastable.HandleRaycast(this))//Send this PlayerController to all of the HandleRaycast classes.
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }
        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
           
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;//Distances element has been assigned.
            }
            //Distances array's elements are sorted 
            Array.Sort(distances, hits);

            return hits;
        }
     
  
        private bool InteractWithMovement()
        {
      

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            
            if (hasHit)
            {
                if(Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target,1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }
        private bool RaycastNavMesh(out Vector3 target)
        { //Raycasting to terrain
            target = new Vector3();

            //origin point of Raycasting.
            RaycastHit hit; //Info about where we have clicked.
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;

            //Find nearest position by SamplePosition.
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit,maxNavMeshProjectionDistance,NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            NavMeshPath navMeshPath = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, navMeshPath);
            if(!hasPath) return false;
            if(navMeshPath.status != NavMeshPathStatus.PathComplete) return false;//In order to prevent path calculation to the ROOFTOPS.
            if (GetPathLength(navMeshPath) > maxPathLength) return false;//When the destination is way far off from our position.

            return true;
        }

        private float GetPathLength(NavMeshPath navMeshPath)
        {//Summation of these path corners in the navMeshPath.
            float total = 0;
            if (navMeshPath.corners.Length < 2) return total; //When there is just 1 line with 2 corners return 0;.
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
            {
                total += Vector3.Distance(navMeshPath.corners[i], navMeshPath.corners[i+1]);
            }
            return total;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture,mapping.hotspot,CursorMode.Auto);
        }
        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
