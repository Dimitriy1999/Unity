using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LaunchPad))]
public class LaunchPadHandle : Editor
{
    private int handleID;
    private Transform transform;
    private LaunchPad launchPad;
    private bool cache = false;
    private bool draggingArrow = false;
    private Quaternion arrowDirection;
    private int currentLineCount;
    private float previousAngle;
    private float previousForce;
    private int linesToRender = maxNumberOfPoints;

    private const int maxNumberOfPoints = 1000;
    private const float timeIncrement = 0.05f;
    private readonly Color selectedColor = Color.red;
    private readonly Color notSelected = Color.blue;
    private readonly int leftButtonPressed = 0;
    private void OnDisable()
    {
        Tools.hidden = false;
        SceneView.RepaintAll();
    }

    protected virtual void OnSceneGUI()
    {
        if (!cache)
        {
            CacheVariables();
            cache = true;
        }

        bool leftMousePressed = LeftMousePressed();

        if (!launchPad.EditingLaunchDirection() || Event.current.keyCode == KeyCode.Escape)
        {
            launchPad.DisableEditing();
            Tools.hidden = false;
            return;
        }
        else
        {
            if (leftMousePressed && Selection.activeGameObject == transform.gameObject)
            {
                var selectedGameObject = ObjectSelected();
                if (GUIUtility.hotControl != 0 && selectedGameObject != transform.gameObject)
                {
                    Selection.activeGameObject = selectedGameObject;
                    return;
                }
            }
            Tools.hidden = true;
        }

        switch (Event.current.type)
        {
            case EventType.Repaint:
                DrawHandles(EventType.Repaint);
                break;
            case EventType.Layout:
                DrawHandles(EventType.Layout);
                break;
            case EventType.MouseDown when leftMousePressed:
                StartDragging();
                break;
            case EventType.MouseUp:
                EndDragging();
                break;
            case EventType.MouseDrag when draggingArrow:
                HandleMouseDrag();
                break;
            default:
                break;
        }

        previousAngle = UpdateIfChangePathData(previousAngle, launchPad.angle);
        previousForce = UpdateIfChangePathData(previousForce, launchPad.force);

        RenderLaunchPadPath();
    }

    private float UpdateIfChangePathData(float previous, float after)
    {
        if (previous == after) return previous;

        previous = after;
        currentLineCount = 0;
        linesToRender = maxNumberOfPoints;

        return previous;
    }

    private bool LeftMousePressed()
    {
        return Event.current.type == EventType.MouseDown && Event.current.isMouse && Event.current.button == leftButtonPressed;
    }

    private GameObject ObjectSelected()
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Physics.Raycast(mouseRay, out RaycastHit hit);

        if (hit.transform == null) return null;

        return hit.transform.gameObject;
    }

    private void CacheVariables()
    {
        launchPad = target as LaunchPad;
        transform = launchPad.transform;
        handleID = GUIUtility.GetControlID(FocusType.Passive);
        arrowDirection = launchPad.forceDirection.eulerAngles == Vector3.zero ? Quaternion.identity : launchPad.forceDirection;
    }

    private void DrawHandles(EventType eventType)
    {
        HandlesColor();
        Handles.ArrowHandleCap(0, transform.position, arrowDirection, HandleUtility.GetHandleSize(transform.position), eventType);
    }

    private void StartDragging()
    {
        handleID = GUIUtility.hotControl;
        draggingArrow = true;
    }

    private void EndDragging()
    {
        handleID = GUIUtility.GetControlID(FocusType.Passive);
        Handles.color = notSelected;
        draggingArrow = false;
        SceneView.RepaintAll();
    }

    private void HandleMouseDrag()
    {
        var rotation = Quaternion.Euler(0, arrowDirection.eulerAngles.y + Event.current.delta.x * 0.5f, 0);
        arrowDirection = rotation;
        launchPad.forceDirection = arrowDirection;
        currentLineCount = 0;
        linesToRender = maxNumberOfPoints;
        SceneView.RepaintAll();
    }

    private void HandlesColor()
    {
        Handles.color = (handleID == 0) ? selectedColor : notSelected;
    }

    private void RenderLaunchPadPath()
    {
        Handles.color = launchPad.LineColor;

        Quaternion launchRotation = Quaternion.Euler(-launchPad.angle, arrowDirection.eulerAngles.y, 0);
        Vector3 launchDirection = launchRotation * Vector3.forward;
        float launchForce = launchPad.force;

        Vector3 startingPoint = transform.position;
        Vector3 prevPoint = startingPoint;

        for (int i = 1; i <= linesToRender; i++)
        {
            float t = i * timeIncrement;
            float x = t * launchForce * launchDirection.x;
            float y = t * launchForce * launchDirection.y - 0.5f * -Physics.gravity.y * t * t;
            float z = t * launchForce * launchDirection.z;
            Vector3 nextPoint = startingPoint + new Vector3(x, y, z);
            Handles.DrawLine(prevPoint, nextPoint, launchPad.LineThickness);
            if (currentLineCount == 0 && i > 5 && Physics.Linecast(prevPoint, nextPoint))
            {
                currentLineCount = i;
                linesToRender = currentLineCount;
                break;
            }
            prevPoint = nextPoint;
        }

        if (currentLineCount == 0)
        {
            currentLineCount = maxNumberOfPoints;
        }

    }
}
