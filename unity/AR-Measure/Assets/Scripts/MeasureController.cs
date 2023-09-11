using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Utils;

public class MeasureController : MonoBehaviour
{
    [Tooltip("Button for creating measuring point")]
    [SerializeField]
    private Button m_MeasuringPointButton;

    /// <summary>
    /// Button for creating measuring point
    /// </summary>
    public Button MeasuringPointButton
    {
        get { return m_MeasuringPointButton; }
    }

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


    private void OnEnable()
    {
        m_PersistantRaycastController = GetComponent<PersistantRayCastController>();

        m_MeasuringPointButton.onClick.AddListener(() =>
        {
            if (FirstTimeClick())
            {
                HandleFirstClick();
            } else if (m_MeasuringPoints[m_CurrentMeasuringPointIndex].Snapping == true || 
                        PersistantRayCastController.RaycastHits.Count != 0)
            {
                HandleSecondClick();
            }
        });
    }

    /// <summary>
    /// Logics to handle the first button click
    /// </summary>
    private void HandleFirstClick()
    {
        var posRot = m_PersistantRaycastController.GetRaycastPoint();
        if (posRot.pos.Equals(Vector3.negativeInfinity)) return;

        var measuringPoints = Instantiate(m_MeasuringPointsPrefab, posRot.pos, posRot.rot);
        measuringPoints.GetComponent<MeasuringPointsManager>().NormalSnapThreshold = m_PersistantRaycastController.SnapThreshold;
        m_PersistantRaycastController.AddSnapingPoint(posRot);

        m_MeasuringPoints.Add(measuringPoints.GetComponent<MeasuringPointsManager>());
        m_CurrentMeasuringPointIndex++;
        RegisterButtonClick();

        m_PersistantRaycastController.DisablePersistantRaycastIndicator();

    }

    /// <summary>
    /// Logics to handle the second button click
    /// </summary>
    private void HandleSecondClick()
    {
        RegisterButtonClick();
        m_PersistantRaycastController.EnablePersistantRaycastIndicator();

        var currentMeasuringPoints = m_MeasuringPoints[m_CurrentMeasuringPointIndex];
        currentMeasuringPoints.StopUpdate();
        m_PersistantRaycastController.AddSnapingPoint(new PosRot(
            currentMeasuringPoints.MeasuringPoint2.transform.position,
            currentMeasuringPoints.MeasuringPoint2.transform.rotation));

    }
}
