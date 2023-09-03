using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;

public struct PosRot
{
    public PosRot(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }

    public Vector3 pos;
    public Quaternion rot;
}

public class PersistantRayCastController : MonoBehaviour
{
    /// <summary>
    /// Reference to the raycast manager
    /// </summary>
    private ARRaycastManager m_RaycastManager;

    /// <summary>
    /// List of raycast hits
    /// </summary>
    private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    public static List<ARRaycastHit> RaycastHit { get { return s_Hits; } }

    /// <summary>
    /// Indicator for planes
    /// </summary>
    [SerializeField]
    private GameObject m_Indicator;

    /// <summary>
    /// whether the persistant ray casting indicator should currently enabled
    /// </summary>
    private bool m_Enabled = true;

    /// <summary>
    /// Reference to plane manager
    /// </summary>
    private static ARPlaneManager s_ARPlaneManager;

    /// <summary>
    /// Points that persistant raycasting should snap to
    /// </summary>
    private List<PosRot> m_SnapPoints = new List<PosRot>();

    /// <summary>
    /// whether currently snapped to a point
    /// </summary>
    private PosRot m_CurrentSnapPoint = new PosRot(Vector3.negativeInfinity, Quaternion.identity);

    [Tooltip("Threshold for the snapping behavior")]
    [SerializeField]
    private float m_SnapThreshold;

    /// <summary>
    /// Threshold for the snapping behavior
    /// </summary>
    public float SnapThreshold { get { return m_SnapThreshold; } }


    private void OnEnable()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        s_ARPlaneManager = GetComponent<ARPlaneManager>();
        if (m_RaycastManager == null) { this.LogError("Raycast manager cannot be found"); }
        m_Indicator.SetActive(true);
    }


    private void Update()
    {
        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), s_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon) && m_Enabled)
        {
            m_Indicator.SetActive(true);
            Pose pose = s_Hits[0].pose;
            m_Indicator.transform.position = pose.position;
            m_Indicator.transform.rotation = pose.rotation;
        } else
        {
            m_Indicator.SetActive(false);
        }
        // TODO: Handle snapping point
    }


    public static ARPlane GetCurrentPlane()
    {
        if (s_Hits.Count == 0) return null;

        return s_ARPlaneManager.GetPlane(s_Hits[0].trackableId);
    }

    /// <summary>
    /// Enable the persistant raycast
    /// </summary>
    public void EnablePersistantRaycastIndicator()
    {
        m_Enabled = true;
    }

    /// <summary>
    /// disable the persistant raycast
    /// </summary>
    public void DisablePersistantRaycastIndicator()
    {
        m_Enabled = false;
    }

    /// <summary>
    /// Add a snapping point
    /// </summary>
    /// <param name="snapPoint">Snapping oint</param>
    public void AddSnapingPoint(PosRot snapPoint)
    {
        m_SnapPoints.Add(snapPoint);
    }

    /// <summary>
    /// Return current raycast position
    /// </summary>
    /// <returns></returns>
    public PosRot GetRaycastPoint()
    {
        if (!m_CurrentSnapPoint.pos.Equals(Vector3.negativeInfinity))
            return m_CurrentSnapPoint;
        else if (s_Hits.Count > 0)
            return new PosRot(s_Hits[0].pose.position, s_Hits[0].pose.rotation);
        else
            return new PosRot(Vector3.negativeInfinity, Quaternion.identity); // invalid case
    }

    private PosRot CalculateSnappingPoint()
    {
        //TODO
        return new PosRot(Vector3.negativeInfinity, Quaternion.identity); // invalid case
    }
}
