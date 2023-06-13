using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Utils;

public class PersistantRayCastController : MonoBehaviour
{
    /// <summary>
    /// Plane number to activate raycast
    /// </summary>
    [SerializeField]
    private int m_PlaneNumberToActivate;

    /// <summary>
    /// Return whether enough plane is detected and looking-around elements text is not visible
    /// </summary>
    public bool m_EnoughPlaneDetected => m_arPlaneManager.trackables.count >= m_PlaneNumberToActivate;

    [System.Serializable]
    internal class PanelDetectionIndicator
    {
        /// <summary>
        /// Elements that should be disabled when enough planes detected
        /// </summary>
        [SerializeField]
        public List<FadeController> m_lookingAroundElements;

        /// <summary>
        /// whether elements is disabled
        /// </summary>
        [System.NonSerialized]
        public bool disabled = false;
    }

    /// <summary>
    /// Reference to panel detection indicator
    /// </summary>
    [SerializeField] private PanelDetectionIndicator m_panelDetectionIndicator;

    /// <summary>
    /// Reference to plane manager
    /// </summary>
    private ARPlaneManager m_arPlaneManager;

    /// <summary>
    /// Reference to the raycast manager
    /// </summary>
    private ARRaycastManager m_RaycastManager;

    /// <summary>
    /// Indicator for planes
    /// </summary>
    [SerializeField]
    private GameObject m_Indicator;

    /// <summary>
    /// Measure button elements for add point
    /// </summary>
    [SerializeField]
    private List<FadeController> m_MeasureButtonElements;

    /// <summary>
    /// List of raycast hits
    /// </summary>
    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();


    private void OnEnable()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        m_arPlaneManager = GetComponent<ARPlaneManager>();

        foreach(var element in m_MeasureButtonElements)
        {
            StartCoroutine(element.FadeToZeroAlpha(0));
            element.gameObject.SetActive(false);
        }

        m_Indicator.SetActive(false);
    }
    private void Update()
    {
        if (m_arPlaneManager.trackables.count >= m_PlaneNumberToActivate)
        {
            if (!m_panelDetectionIndicator.disabled)
            {
                foreach(var element in m_panelDetectionIndicator.m_lookingAroundElements)
                {
                    StartCoroutine(element.FadeToZeroAlpha(1.0f));
                }
                m_panelDetectionIndicator.disabled = true;

                foreach(var element in m_MeasureButtonElements)
                {
                    element.gameObject.SetActive(true);
                    StartCoroutine(element.FadeToFullAlpha(1.0f));
                }
            }

            if (m_RaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), m_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
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
    }
}
