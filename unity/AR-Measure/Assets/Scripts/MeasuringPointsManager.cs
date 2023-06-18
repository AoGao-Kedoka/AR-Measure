using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Manager for the measuring points
/// First button click: measuringpointsmanager instance instantiated
/// Second button click: measuringpoint finished
/// </summary>
public class MeasuringPointsManager: MonoBehaviour
{
    /// <summary>
    /// Measuring point 1
    /// </summary>
    [SerializeField]
    GameObject m_MeasuringPoint1;

    /// <summary>
    /// Measuring point 2
    /// </summary>
    [SerializeField]
    GameObject m_MeasuringPoint2;

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
        if (PersistantRayCastController.RaycastHit.Count == 0) { return; }
        else
        {
            m_MeasuringPoint1.transform.position = PersistantRayCastController.RaycastHit[0].pose.position;
            m_MeasuringPoint1.transform.rotation= PersistantRayCastController.RaycastHit[0].pose.rotation;
        }
        m_DistanceLine = GetComponent<LineRenderer>();
        DisplayVerticalLine();
    }

    private void OnDisable()
    {
        HideVerticalLine();
    }

    private void Update()
    {
        if (PersistantRayCastController.RaycastHit.Count != 0)
        {
            Pose pose = PersistantRayCastController.RaycastHit[0].pose;
            m_MeasuringPoint2.transform.position = pose.position;
            m_MeasuringPoint2.transform.rotation = pose.rotation;


            m_DistanceLine.SetPosition(0, m_MeasuringPoint1.transform.position);
            m_DistanceLine.SetPosition(1, m_MeasuringPoint2.transform.position);

            DisplayDistance();
        }
    }

    public void StopUpdate()
    {
        this.enabled = false;
    }

    public void DisplayVerticalLine()
    {
        ARPlane plane = PersistantRayCastController.GetCurrentPlane();
        Vector3 direction = plane.normal;
        m_NormalLine.SetPosition(0, m_MeasuringPoint1.transform.position + direction * 10);
        m_NormalLine.SetPosition(1, m_MeasuringPoint1.transform.position - direction * 10);
    }

    public void HideVerticalLine()
    {
        m_NormalLine.enabled = false;
    }
}
