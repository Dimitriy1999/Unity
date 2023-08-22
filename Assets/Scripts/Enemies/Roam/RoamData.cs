using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class RoamData
{
    public Vector3 PointToMoveTo
    {
        get { return pointToMoveTo; }
    }
    public Transform ObjectToMove
    {
        get { return objectToMove; }
    }

    public RoamState CurrentState
    {
        get { return enemyState; }
    }

    private Vector3 pointToMoveTo;
    private Transform objectToMove;
    private RoamState enemyState;

    private Timer timeUntilRoam;
    private float time = 5;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private const float OFFSET = 1f;
    public RoamData(Vector3 pointToMoveTo, Transform objectToMove, RoamState enemyState)
    {
        this.pointToMoveTo = pointToMoveTo;
        this.objectToMove = objectToMove;
        this.enemyState = enemyState;

        timeUntilRoam = new(time);
        agent = objectToMove.gameObject.GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        path = new();
    }

    public bool LookAtPoint()
    {
        if (enemyState == RoamState.Roaming && timeUntilRoam.CheckElapsedTime(time, false)) return false;

        if (agent.velocity == Vector3.zero || Vector3.Distance(objectToMove.position, pointToMoveTo) <= 0) return true;

        var newRotation = Quaternion.LookRotation(agent.velocity, Vector3.up);

        objectToMove.rotation = Quaternion.Slerp(objectToMove.rotation, newRotation, 0.25f);

        return Quaternion.Angle(newRotation, objectToMove.rotation) <= 0;
    }
    public bool MoveToPoint()
    {
        agent.SetDestination(pointToMoveTo);

        return Vector3.Distance(objectToMove.position, pointToMoveTo) <= agent.stoppingDistance + OFFSET;
    }

    public bool ValidPath()
    {
        bool foundPath = agent.CalculatePath(pointToMoveTo, path);

        Roam.Instance.PrintPathStatus(foundPath, path.status.ToString());

        if (foundPath && path.status == NavMeshPathStatus.PathPartial) return false;

        if (foundPath && path.status == NavMeshPathStatus.PathInvalid) return false;

        return foundPath;
    }

    public void ValidatePoint()
    {
        while (!ValidPath())
        {
            pointToMoveTo = Roam.Instance.SelectRandomPoint();
        }
    }

    public void SelectNewPoint(Vector3 point)
    {
        pointToMoveTo = point;
        ValidatePoint();
        timeUntilRoam.Reset();
        time = Random.Range(1, 5);
    }

    public void SetEnemyRoamState(RoamState state)
    {
        enemyState = state;
    }
    public bool IsEnemyRoaming()
    {
        return enemyState == RoamState.Roaming;
    }

}
