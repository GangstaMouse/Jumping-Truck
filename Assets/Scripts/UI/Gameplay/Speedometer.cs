using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    private TMP_Text m_Text;
    private Vehicle m_Vehicle;
    private bool m_IsVisible;

    private void Start()
    {
        m_Text = GetComponent<TMP_Text>();
        m_Vehicle = GetComponentInParent<Vehicle>();
    }

    /* private void OnEnable() => m_Vehicle.OnControllerChanged += ChangeVisibility;
    private void OnDisable() => m_Vehicle.OnControllerChanged -= ChangeVisibility;

    private void ChangeVisibility(Controller controller) => SetVisibility(controller == Controller.Player);

    public void SetVisibility(bool newVisibility)
    {
        m_IsVisible = newVisibility;
        gameObject.SetActive(m_IsVisible);
    } */

    private void LateUpdate()
    {
        float speed = Mathf.FloorToInt(Mathf.Abs(m_Vehicle.LocalAirSpeed.z) * 3.6f);
        m_Text.SetText($"{speed} km/h");
    }
}
