using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;

public class PersistantRayCastController : MonoBehaviour
{
    /// <summary>
    /// Reference to the raycast manager
    /// </summary>
    private ARRaycastManager m_RaycastManager;

    /// <summary>
    /// List of raycast hits
    /// </summary>
    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    public List<ARRaycastHit> RaycastHit { get { return m_Hits; } }

    /// <summary>
    /// Indicator for planes
    /// </summary>
    [SerializeField]
    private GameObject m_Indicator;

    /// <summary>
    /// whether the persistant ray casting indicator should currently enabled
    /// </summary>
    private bool m_Enabled = true;

    private void OnEnable()
    {
         m_RaycastManager = GetComponent<ARRaycastManager>();
         if (m_RaycastManager == null) { this.LogError("Raycast manager cannot be found"); }
        m_Indicator.SetActive(true);
    }


    private void Update()
    {
        if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), m_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)
            && m_Enabled)
        {
            m_Indicator.SetActive(true);
            Pose pose = m_Hits[0].pose;
            m_Indicator.transform.position = pose.position;
            m_Indicator.transform.rotation = pose.rotation;
        } else
        {
            m_Indicator.SetActive(false);
        }
    }

    public void EnablePersistantRaycastIndicator()
    {
        m_Enabled = true;
    }
    public void DisablePersistantRaycastIndicator()
    {
        m_Enabled = false;
    }
}
