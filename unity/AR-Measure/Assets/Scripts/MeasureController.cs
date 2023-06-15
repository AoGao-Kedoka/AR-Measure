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

    [Tooltip("Measure point prefab")]
    [SerializeField]
    private GameObject m_MeasuringPointPrefab;

    [Tooltip("Distance displaying UI")]
    [SerializeField]
    private GameObject m_DistanceDisplayingUIPrefab;

    struct MeasuringPair
    {
        public Transform m_MeasuringPoint1;
        public Transform m_MeasuringPoint2;

        public GameObject m_DistanceDisplayer;

        public float Distance()
        {
            return Vector3.Distance(m_MeasuringPoint1.position, m_MeasuringPoint2.position);
        }

        public void DisplayDistance()
        {
            if (Distance() <= 0.001f)
            {
                m_DistanceDisplayer.SetActive(false);
            }
            else
            {
                m_DistanceDisplayer.SetActive(true);
                m_DistanceDisplayer.transform.position = (m_MeasuringPoint1.position + m_MeasuringPoint2.position) / 2 + new Vector3(0, 0.1f, 0);
                m_DistanceDisplayer.GetComponent<DistanceDisplay>().UpdateText(Distance().ToString("0.00") + "m");
            }
        }
    }

    /// <summary>
    /// Container for saving all the measuring points
    /// </summary>
    private List<MeasuringPair> m_MeasuringPoints = new List<MeasuringPair>();

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
            if (m_PersistantRaycastController.RaycastHit.Count != 0)
            {
                if (FirstTimeClick())
                {
                    MeasuringPair pair;
                    pair.m_MeasuringPoint1 = Instantiate(m_MeasuringPointPrefab).transform;
                    pair.m_MeasuringPoint2 = Instantiate(m_MeasuringPointPrefab).transform;
                    pair.m_DistanceDisplayer = Instantiate(m_DistanceDisplayingUIPrefab);
                    pair.m_DistanceDisplayer.SetActive(false);
                    pair.m_MeasuringPoint1.transform.position = m_PersistantRaycastController.RaycastHit[0].pose.position;
                    pair.m_MeasuringPoint1.transform.rotation = m_PersistantRaycastController.RaycastHit[0].pose.rotation;

                    m_MeasuringPoints.Add(pair);
                    m_CurrentMeasuringPointIndex++;
                    RegisterButtonClick();

                    m_PersistantRaycastController.DisablePersistantRaycastIndicator();
                } else
                {
                    RegisterButtonClick();
                    m_PersistantRaycastController.EnablePersistantRaycastIndicator();
                }
            }
        });
    }

    private void Update()
    {
        if (m_PersistantRaycastController.RaycastHit.Count != 0 && !FirstTimeClick())
        {
            Pose pose = m_PersistantRaycastController.RaycastHit[0].pose;
            var measuringPoint1 = m_MeasuringPoints[m_CurrentMeasuringPointIndex].m_MeasuringPoint1;
            var measuringPoint2 = m_MeasuringPoints[m_CurrentMeasuringPointIndex].m_MeasuringPoint2;
            measuringPoint2.position = pose.position;
            measuringPoint2.rotation = pose.rotation;

            measuringPoint1.GetComponent<LineRenderer>().SetPosition(0, measuringPoint1.position);
            measuringPoint1.GetComponent<LineRenderer>().SetPosition(1, measuringPoint2.position);

            m_MeasuringPoints[m_CurrentMeasuringPointIndex].DisplayDistance();
        }
    }
}
