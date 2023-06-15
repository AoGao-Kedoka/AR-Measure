using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.Samples;
using Utils;

public class PreparationIndicatorController : MonoBehaviour
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
    /// Measure button elements for add point
    /// </summary>
    [SerializeField]
    private List<FadeController> m_MeasureButtonElements;

    private void OnEnable()
    {
        m_arPlaneManager = GetComponent<ARPlaneManager>();

        foreach(var element in m_MeasureButtonElements)
        {
            StartCoroutine(element.FadeToZeroAlpha(0));
            element.gameObject.SetActive(false);
        }

        m_arPlaneManager.planesChanged += PlaneChangedInPreparation;
    }

    private void Update()
    {
        if (m_arPlaneManager.trackables.count >= m_PlaneNumberToActivate)
        {
            if (!m_panelDetectionIndicator.disabled)
            {
                m_panelDetectionIndicator.disabled = true;
                foreach(var element in m_panelDetectionIndicator.m_lookingAroundElements)
                {
                    StartCoroutine(element.FadeToZeroAlpha(1.0f));
                }

                foreach(var element in m_MeasureButtonElements)
                {
                    element.gameObject.SetActive(true);
                    StartCoroutine(element.FadeToFullAlpha(1.0f));
                }

                GetComponent<PersistantRayCastController>().enabled = true;
                GetComponent<MeasureController>().enabled = true;
                this.enabled = false;
            }
        }
    }

    private void OnDisable()
{
        DisablePointCloud();
        EnablePlaneVisualizer();
    }

    void EnablePlaneVisualizer()
    {
        m_arPlaneManager.planesChanged -= PlaneChangedInPreparation;
        foreach(var plane in m_arPlaneManager.trackables)
        {
            plane.GetComponent<ARPlaneMeshVisualizer>().enabled = true;
            plane.GetComponent<ARFeatheredPlaneMeshVisualizer>().enabled = true;
        }
    }
    void DisablePointCloud()
    {
        ARPointCloudManager pointCloudManager = GetComponent<ARPointCloudManager>();
        foreach(var point in pointCloudManager.trackables)
        {
            point.gameObject.SetActive(false);
        }
        pointCloudManager.enabled = false;
    }

    void PlaneChangedInPreparation(ARPlanesChangedEventArgs args)
    {
        foreach(var plane in args.added)
        {
            plane.GetComponent<ARPlaneMeshVisualizer>().enabled = false;
            plane.GetComponent<ARFeatheredPlaneMeshVisualizer>().enabled = false;
        }
    }
}
