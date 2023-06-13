using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeasureController : MonoBehaviour
{
    [Tooltip("Button for creating measuring point")]
    [SerializeField]
    private Button m_MeasuringPointButton;

    /// <summary>
    /// Button for creating measuring point
    /// </summary>
    public Button MeasuringPointButton { get { return m_MeasuringPointButton; } }

    private void OnEnable()
    {
        m_MeasuringPointButton.onClick.AddListener(() =>
        {
            Debug.Log("Clicked");
        });
    }
}
