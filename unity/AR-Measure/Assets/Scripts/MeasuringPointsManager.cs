using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;

/// <summary>
/// Manager for the measuring points
/// First button click: measuringpointsmanager instance instantiated
/// Second button click: measuringpoint finished
/// </summary>
public class MeasuringPointsManager : MonoBehaviour
{
    [Tooltip("Measuring point 1")]
    [SerializeField]
    GameObject m_MeasuringPoint1;

    /// <summary>
    /// First click measuring point
    /// </summary>
    public GameObject MeasuringPoint1
    {
        get { return m_MeasuringPoint1; }
    }

    [Tooltip("Measuring point 2")]
    [SerializeField]
    GameObject m_MeasuringPoint2;

    /// <summary>
    /// Second click measuring point
    /// </summary>
    public GameObject MeasuringPoint2
    {
        get { return m_MeasuringPoint2; }
    }

    /// <summary>
    /// Prefab for displaying distance
    /// </summary>
    [SerializeField]
    DistanceDisplay m_DistanceDisplayer;

    /// <summary>
    /// Line renderer between 2 measuring point
    /// </summary>
    [SerializeField]
    private LineRenderer m_DistanceLine;

    /// <summary>
    /// Line renderer for displaying normal
    /// </summary>
    [SerializeField]
    private LineRenderer m_NormalLine;

    private float m_NormalSnapThreshold;

    /// <summary>
    /// Snaping behavior of vertical lines
    /// </summary>
    /// <returns></returns>
    public float NormalSnapThreshold
    {
        get { return m_NormalSnapThreshold;}
        set { m_NormalSnapThreshold = value; }
    }

    /// <summary>
    /// whether currently snaps to the normal line
    /// </summary>
    private bool m_Snapping;

    public bool Snapping { get { return m_Snapping; } }
    
    /// <summary>
    /// Distance between points
    /// </summary>
    /// <returns>distance between 2 measuring point</returns>
    public float Distance()
    {
        return Vector3.Distance(m_MeasuringPoint1.transform.position, m_MeasuringPoint2.transform.position);
    }

    /// <summary>
    /// Display distance
    /// </summary>
    public void DisplayDistance()
    {
        if (Distance() <= 0.001f)
        {
            m_DistanceDisplayer.gameObject.SetActive(false);
        }
        else
        {
            m_DistanceDisplayer.gameObject.SetActive(true);
            m_DistanceDisplayer.transform.position = (m_MeasuringPoint1.transform.position + m_MeasuringPoint2.transform.position) / 2 + new Vector3(0, 0.1f, 0);
            m_DistanceDisplayer.UpdateText(Distance().ToString("0.00") + "m");
        }
    }

    private void OnEnable()
    {
        m_MeasuringPoint1.transform.position = this.transform.position;
        m_MeasuringPoint1.transform.rotation = this.transform.rotation;
        m_DistanceLine = GetComponent<LineRenderer>();
        DisplayVerticalLine();
    }

    private void OnDisable()
    {
        HideVerticalLine();
    }

    private void Update()
    {
        var values = CalculateDistanceToNormalLine();
        if (values[0] < m_NormalSnapThreshold)
        {
            // snapping to the normal
            m_MeasuringPoint2.transform.position = new Vector3(values[1], values[2], values[3]);
            UpdatePointsDistance();
            if (!m_Snapping) { AndroidHapticFeedbackManager.HapticFeedback(); }
            m_Snapping = true;

            return;
        } else
        {
            if (m_Snapping) { AndroidHapticFeedbackManager.HapticFeedback(); }
            m_Snapping = false;
        }

        if (PersistantRayCastController.RaycastHits.Count != 0)
        {
            Pose pose = PersistantRayCastController.RaycastHits[0].pose;
            m_MeasuringPoint2.transform.position = pose.position;
            m_MeasuringPoint2.transform.rotation = pose.rotation;
            UpdatePointsDistance();
        }

    }

    public void StopUpdate()
    {
        this.enabled = false;
    }

    /// <summary>
    /// Calculate the shortest distance between the camera ray and the vertical line ray
    /// </summary>
    /// <returns>float array, index 0 is the distance,
    /// index 1,2,3 = clostest position of vertical line point
    /// index 4,5,6 = clostest position of the camera point
    /// </returns>
    private float[] CalculateDistanceToNormalLine()
    {
        float[] values = new float[7];
        Vector3 norLineDir = (m_NormalLine.GetPosition(1) - m_NormalLine.GetPosition(0)).normalized;
        Vector3 cameraDir = Camera.main.transform.forward;

        Vector3 cross = Vector3.Cross(norLineDir, cameraDir);
        Vector3 diff = Camera.main.transform.position - m_MeasuringPoint1.transform.position;

        // Don't know why this is not working, using temporary solution
        // Vector3 vertLinePoint = m_MeasuringPoint1.transform.position + Vector3.Dot(diff, vertLineDir) / vertLineDir.sqrMagnitude * vertLineDir;
        Vector3 cameraPoint = Camera.main.transform.position + Vector3.Dot(-diff, cameraDir) / cameraDir.sqrMagnitude * cameraDir;
        Vector3 AP = cameraPoint - m_MeasuringPoint1.transform.position;
        float d = Vector3.Dot(AP, norLineDir);
        Vector3 vertLinePoint = m_MeasuringPoint1.transform.position + norLineDir * d;
        

#if UNITY_EDITOR
        Debug.DrawLine(m_MeasuringPoint1.transform.position, m_MeasuringPoint1.transform.position + norLineDir * 20);
        Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + cameraDir * 20);
        Debug.DrawLine(vertLinePoint, cameraPoint, Color.red);
#endif

        values[0] = Mathf.Abs(Vector3.Dot(diff, cross) / cross.magnitude);
        values[1] = vertLinePoint.x;
        values[2] = vertLinePoint.y;
        values[3] = vertLinePoint.z;
        values[4] = cameraPoint.x;
        values[5] = cameraPoint.y;
        values[6] = cameraPoint.z;

        return values;
    }

    public void DisplayVerticalLine()
    {
        ARPlane plane = PersistantRayCastController.GetCurrentPlane();
        Vector3 direction = plane.normal;
        m_NormalLine.transform.position = m_MeasuringPoint1.transform.position;

        m_NormalLine.SetPosition(0, m_MeasuringPoint1.transform.position + direction * 10);
        m_NormalLine.SetPosition(1, m_MeasuringPoint1.transform.position - direction * 10);

    }

    private void UpdatePointsDistance()
    {
        m_DistanceLine.SetPosition(0, m_MeasuringPoint1.transform.position);
        m_DistanceLine.SetPosition(1, m_MeasuringPoint2.transform.position);

        DisplayDistance();
    }

    public void HideVerticalLine()
    {
        m_NormalLine.enabled = false;
    }
}
