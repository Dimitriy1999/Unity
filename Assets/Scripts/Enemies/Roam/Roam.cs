using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roam : MonoBehaviour
{
    [SerializeField] private Transform startingPoint;
    [SerializeField] private float radius;
    [SerializeField] private int segments;
    [SerializeField] private LayerMask enemyLayer;
    [Header("Debug Options")]
    [SerializeField] private bool debug;
    [SerializeField] private bool printPathStatus;

    private BoolTimer roaming;
    private List<Vector3> circlePoints;
    private List<RoamData> roamData;
    private List<GameObject> roamPoints;
    private float angle;
    private Vector3 suspiciousPoint;
    private Timer suspiciousTimer;
    private Timer foundTimer;
    public static Roam Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]


    private static void Init()
    {
        Instance = null;
    }


    private void Awake()
    {
        Instance = this;
        roaming.Set(5.0f);
        circlePoints = new List<Vector3>(segments + 1);
        angle = 360.0f / segments * Mathf.Deg2Rad;
        roamData = new();
        roamPoints = new();
        suspiciousTimer = new();
        foundTimer = new();
        if (roaming)
        {
            roaming.Reset();
            var sphereCast = Physics.SphereCastAll(startingPoint.position, radius, Vector3.down, 0, enemyLayer);

            for (int i = 0; i < sphereCast.Length; i++)
            {
                var enemy = sphereCast[i].transform;

                roamData.Add(new RoamData(SelectRandomPoint(), enemy, RoamState.Roaming));
                roamData[i].ValidatePoint();
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < roamData.Count; i++)
        {
            RoamData enemy = roamData[i];

            var lookForPlayerComponent = enemy.ObjectToMove.GetComponent<LookForPlayer>();

            HandleFoundEnemyState(enemy, lookForPlayerComponent);

            if (!enemy.LookAtPoint()) continue;

            if (!enemy.MoveToPoint()) continue;

            HandleRoamState(enemy);
        }
        ReCheckPoints();
        HandleVisuals();
    }

    private void HandleRoamState(RoamData roamingEnemy)
    {
        if (roamingEnemy.CurrentState == RoamState.Roaming)
        {
            Vector3 newPoint = SelectRandomPoint();

            while (Vector3.Distance(roamingEnemy.ObjectToMove.position, newPoint) <= 2)
            {
                newPoint = SelectRandomPoint();
            }

            roamingEnemy.SelectNewPoint(newPoint);
        }
    }

    private void HandleFoundEnemyState(RoamData roamingEnemy, LookForPlayer lookForPlayerComponent)
    {
        bool spotted = lookForPlayerComponent.SpottedPlayer(roamingEnemy.ObjectToMove);
        if (spotted)
        {
            if (roamingEnemy.CurrentState == RoamState.Suspicious || roamingEnemy.CurrentState == RoamState.EnemyFound)
            {
                ResetTimers();
                roamingEnemy.SetEnemyRoamState(RoamState.EnemyFound);
                roamingEnemy.SelectNewPoint(lookForPlayerComponent.GetLastPlayerPosition());
            }
            else
            {
                EnterSuspiciousState(roamingEnemy, lookForPlayerComponent);
            }
        }
        else
        {
            if (roamingEnemy.CurrentState == RoamState.EnemyFound)
            {
                ResetSuspiciousPoint();
                roamingEnemy.SetEnemyRoamState(RoamState.Suspicious);
                EnterSuspiciousState(roamingEnemy, lookForPlayerComponent);
            }
            else if (roamingEnemy.CurrentState != RoamState.Roaming)
            {
                if (IsTimedOut())
                {
                    ResetTimers();
                    roamingEnemy.SetEnemyRoamState(RoamState.Roaming);
                }
            }
        }
    }

    private void EnterSuspiciousState(RoamData roamingEnemy, LookForPlayer lookForPlayerComponent)
    {
        roamingEnemy.SetEnemyRoamState(RoamState.Suspicious);
        suspiciousTimer.Reset();

        if (suspiciousPoint == Vector3.zero)
        {
            if (!lookForPlayerComponent.PointInView(roamingEnemy.ObjectToMove)) return;

            suspiciousPoint = lookForPlayerComponent.GetLastPlayerPosition();
            roamingEnemy.SelectNewPoint(suspiciousPoint);
        }
    }

    private void ResetTimers()
    {
        foundTimer.Reset();
        suspiciousTimer.Reset();
    }

    private void ResetSuspiciousPoint()
    {
        suspiciousPoint = Vector3.zero;
    }

    private bool IsTimedOut()
    {
        return foundTimer.Time >= 5f || suspiciousTimer.Time >= 5f;
    }

    private void ReCheckPoints()
    {
        for (int i = 0; i < roamData.Count; i++)
        {
            var currentData = roamData[i];
            for (int j = i + 1; j < roamData.Count; j++)
            {
                var nextData = roamData[j];

                while (Vector3.Distance(currentData.PointToMoveTo, nextData.PointToMoveTo) <= 2)
                {
                    nextData.SelectNewPoint(SelectRandomPoint());
                }

            }
        }
    }

    private void HandleVisuals()
    {
        if (debug)
        {
            AddCirclePointsToList();
            VisualizeRoaming();
        }
        else
        {
            if (roamPoints.Count > 0)
            {
                foreach (var item in roamPoints)
                {
                    Destroy(item);
                }
                roamPoints.Clear();
            }
        }
    }

    private void AddCirclePointsToList()
    {
        bool positionChanged = HasPositionChanged(out Vector3 firstPoint);

        if (!positionChanged) return;

        circlePoints.Clear();
        circlePoints.Add(firstPoint);

        for (int i = 1; i <= segments; i++)
        {
            var x = startingPoint.position.x + radius * Mathf.Cos(angle * i);
            var y = startingPoint.position.z + radius * Mathf.Sin(angle * i);
            Vector3 currentPoint = new Vector3(x, startingPoint.position.y, y);
            circlePoints.Add(currentPoint);
        }
    }

    private bool HasPositionChanged(out Vector3 firstPoint)
    {
        var curX = startingPoint.position.x + radius * Mathf.Cos(angle * 0);
        var curY = startingPoint.position.z + radius * Mathf.Sin(angle * 0);
        firstPoint = new Vector3(curX, startingPoint.position.y, curY);

        if (circlePoints.Count <= 0) return true;

        bool positionChanged = (firstPoint - circlePoints[0]).magnitude > 0f;
        return positionChanged;
    }

    public Vector3 SelectRandomPoint()
    {
        var randomRadius = Random.Range(0.5f, radius);
        var randomAngle = angle * Random.Range(0, segments);

        var randomXPoint = startingPoint.position.x + randomRadius * Mathf.Cos(randomAngle);
        var randomYPoint = startingPoint.position.z + randomRadius * Mathf.Sin(randomAngle);
        Vector3 finalPosition = new Vector3(randomXPoint, startingPoint.position.y, randomYPoint);
        return finalPosition;
    }

    private void VisualizeRoaming()
    {
        DrawCircle();

        VisualizePoints();

        for (int i = 0; i < roamData.Count; i++)
        {
            var data = roamData[i];

            var direction = (data.PointToMoveTo - data.ObjectToMove.position).normalized;
            var distance = Vector3.Distance(data.PointToMoveTo, data.ObjectToMove.position);
            Debug.DrawRay(data.ObjectToMove.position, direction * distance, Color.blue);
        }

    }

    private void VisualizePoints()
    {

        for (int i = 0; i < roamData.Count; i++)
        {
            RoamData item = roamData[i];
            if (roamPoints.Count < roamData.Count)
            {
                var test = Instantiate(startingPoint.gameObject);
                test.transform.position = item.PointToMoveTo;
                roamPoints.Add(test);
            }
            else
            {
                roamPoints[i].transform.position = roamData[i].PointToMoveTo;
            }
        }
    }

    private void DrawCircle()
    {
        for (int i = 0; i < circlePoints.Count - 1; i++)
        {
            Debug.DrawLine(circlePoints[i], circlePoints[i + 1], Color.blue);
        }

        Vector3 start = circlePoints[circlePoints.Count - 1];
        Debug.DrawLine(start, start);
    }

    public void PrintPathStatus(bool pathFound, string pathStatus)
    {
        if (!printPathStatus) return;

        print($"Path Found : {pathFound} | Path Status {pathStatus}");
    }

}
