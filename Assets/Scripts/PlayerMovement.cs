using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    private Camera mainCamera;

    #region Server
    // Server handles the movement
    [Command]
    private void CmdMove(Vector3 position)
    {
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(hit.position);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        //base.OnStartAuthority();

        mainCamera = Camera.main;
    }

    [ClientCallback]
    private void Update()
    {
        // Make sure it belongs to us
        if (!hasAuthority)
        {
            return;
        }

        // Make sure we pressed the RMB
        if(!Input.GetMouseButtonDown(1))
        {
            return;
        }

        // Check in the scene if we hit
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            return;
        }

        CmdMove(hit.point);
    }
    #endregion
}
