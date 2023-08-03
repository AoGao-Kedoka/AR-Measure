using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Utils;

public class MeasureController : MonoBehaviour
{
    [Tooltip("Button for creating measuring point")]
    [SerializeField]
    private Button m_MeasuringPointButton;

    [Tooltip("Measure points prefab")]
    [SerializeField]
    private GameObject m_MeasuringPointsPrefab;


    /// <summary>
    /// Container for saving all the measuring points
    /// </summary>
    private List<MeasuringPointsManager> m_MeasuringPoints = new List<MeasuringPointsManager>();

    /// <summary>
    /// current index
    /// </summary>
    private int m_CurrentMeasuringPointIndex = -1;

    /// <summary>
    /// Saving for double click, value only between 0 and 1
    /// </summary>
    private int m_DoubleClick = 0;

    private void RegisterButtonClick()
    {
        if (m_DoubleClick < 0 || m_DoubleClick >= 2) { this.LogError("Something wrong with button!!"); }
        m_DoubleClick = (m_DoubleClick + 1) % 2;
    }

    private bool FirstTimeClick()
    {
        if (m_DoubleClick < 0 || m_DoubleClick >= 2) { this.LogError("Something wrong with button!!"); }
        return m_DoubleClick == 0;
    }

    /// <summary>
    /// Reference to anchor manager
    /// </summary>
    private ARAnchorManager m_AnchorManager => GetComponent<ARAnchorManager>();

    /// <summary>
    /// Reference to persistant raycast controller
    /// </summary>
    PersistantRayCastController m_PersistantRaycastController;

    /// <summary>
    /// Button for creating measuring point
    /// </summary>
    public Button MeasuringPointButton { get { return m_MeasuringPointButton; } }

    private void OnEnable()
    {
        m_PersistantRaycastController = GetComponent<PersistantRayCastController>();

        m_MeasuringPointButton.onClick.AddListener(() =>
        {
            if (PersistantRayCastController.RaycastHit.Count != 0)
            {
                if (FirstTimeClick())
                {
                    var measuringPoints = Instantiate(m_MeasuringPointsPrefab);

                    m_MeasuringPoints.Add(measuringPoints.GetComponent<MeasuringPointsManager>());
                    m_CurrentMeasuringPointIndex++;
                    RegisterButtonClick();

                    m_PersistantRaycastController.DisablePersistantRaycastIndicator();
                } else
                {
                    RegisterButtonClick();
                    m_MeasuringPoints[m_CurrentMeasuringPointIndex].StopUpdate();
                    m_PersistantRaycastController.EnablePersistantRaycastIndicator();
                }
            }
        });
    }
}
