using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CarController))]
public class CarControllerEditor : Editor
{
    private int toolIndex;
    private string[] toolName = {"General", "Wizzard", "Debug"};

    private CarController car;
    private Engine engine;
    private Gearbox gearbox;

    private List<WheelCollider> wheels;
    private List<WheelCollider> driveWheels = new List<WheelCollider>();
    private float driveWheelRadius;

    private int gearsCount;
    private int neutralGear;
    private int previewGear = 0;

    private void OnEnable()
    {
        car = (CarController)target;
        engine = car.GetComponent<Engine>();
        gearbox = car.GetComponent<Gearbox>();

        wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());
        float totalDriveRadius = 0f;

        foreach (var wheel in wheels)
        {
            if (wheel.DriveAxle)
            {
                driveWheels.Add(wheel);
                // Заменить на Max/Min?
                totalDriveRadius += wheel.transform.GetChild(0).GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
                // totalDriveRadius = wheel.transform.Find("Tire").GetComponent<MeshFilter>().sharedMesh.bounds.extents.y;
            }
        }

        // Убрать
        driveWheelRadius = totalDriveRadius / driveWheels.Count;
        // driveWheelRadius = totalDriveRadius;

        // Исправить
        if (!gearbox) return;
        gearsCount = gearbox.GearRatios.Count;

        neutralGear = gearbox.GetNeutralGear();
    }

    public override void OnInspectorGUI()
    {
        #region ToolBar

        toolIndex = GUILayout.Toolbar(toolIndex, toolName);

        GUIStyle HeaderStyle = new GUIStyle();
        HeaderStyle.fontStyle = FontStyle.Bold;
        HeaderStyle.normal.textColor = Color.white;
        HeaderStyle.fontSize = 16;

        switch (toolIndex)
        {
            case 0:
                base.OnInspectorGUI();
                break;

            #region Wizzard
            case 1:
                EditorGUILayout.Separator();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.BeginVertical();
                if (GUILayout.Button("Optimal Suspension Settings"))
                {
                    Rigidbody body = car.GetComponent<Rigidbody>();
                    List<WheelCollider> wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());

                    float massPerWheel = body.mass / wheels.Count;

                    float springStiffnes = massPerWheel * 5f;
                    float damperStiffnes = massPerWheel / 10f;

                    Undo.RecordObjects(wheels.ToArray(), "Optimal suspension");
                    foreach (var wheel in wheels)
                    {
                        // float newStiffnes = body.mass / wheels.Count;
                        // wheel.Stiffnes = newStiffnes;
                        // wheel.DamperStiffnes = newStiffnes / 10f;
                        wheel.Stiffnes = springStiffnes;
                        wheel.DamperStiffnes = damperStiffnes;
                    }
                }

                if (GUILayout.Button("Optimal Center of Mass"))
                {
                    Vector3 totalCenterOfMass = Vector3.zero;
                    List<WheelCollider> wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());
                    foreach (var wheel in wheels)
                    {
                        // 1) Центр массы относительно корня подвески
                        // 2) Стоит добавить кнопки: "Oprimal Center of Mass : Y,Z"
                        // 3) Нужно добавить отрисовку центра массы указателем
                        totalCenterOfMass += wheel.transform.localPosition;
                    }

                    Undo.RecordObject(car, "Set Optimal Center of Mass");
                    car.CenterOfMass = totalCenterOfMass / wheels.Count;
                }

                if (GUILayout.Button("Optimal Supension Lenght"))
                {
                    List<WheelCollider> wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());

                    Undo.RecordObjects(wheels.ToArray(), "Optimal Supension Lenght");
                    foreach (var wheel in wheels)
                        wheel.MaxLenght = Mathf.Abs(wheel.transform.Find("Rim").localPosition.y * 1.20f);
                }

                if (GUILayout.Button("Align Wheel to Lenght"))
                {
                    List<WheelCollider> wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());
                    List<Transform> rims = new List<Transform>();

                    foreach (var wheel in wheels)
                        rims.Add(wheel.transform.Find("Rim"));

                    Undo.RecordObjects(rims.ToArray(), "Align Wheels to Lenght");
                    for (int i = 0; i < wheels.Count; i++)
                        rims[i].localPosition = -wheels[i].transform.up * (wheels[i].MaxLenght * 0.8f);
                        // rims[i].localPosition = -wheels[i].transform.up * wheels[i].le
                }
                if (GUILayout.Button("Setup wheels parameters"))
                {
                    List<WheelCollider> wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());

                    Undo.RecordObjects(wheels.ToArray(), "Setup wheels parameters");
                    foreach (var wheel in wheels)
                        wheel.SetupWheelParameters();
                }
                if (GUILayout.Button("Assign Wheel Meshes"))
                {
                    List<WheelCollider> wheels = new List<WheelCollider>(car.GetComponentsInChildren<WheelCollider>());

                    Undo.RecordObjects(wheels.ToArray(), "Assign Wheel Meshes");
                    foreach (var wheel in wheels)
                        wheel.AssignWheelMeshes();
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                break;
                #endregion


            #region Debug
            case 2:
                EditorGUILayout.Separator();
                GUILayout.Label("Preview Speed at Gear", HeaderStyle);

                bool awalible = engine && gearbox;

                if (!awalible)
                {
                    EditorGUILayout.HelpBox("No Awalible Engine, and/or Gearbox", MessageType.Warning);
                    return;
                }
                if (gearsCount == 0)
                {
                    EditorGUILayout.HelpBox("Add gears to Gearbox", MessageType.Warning);
                    return;
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("Gear");
                GUILayout.FlexibleSpace();
                previewGear = EditorGUILayout.IntSlider(previewGear, 0 - neutralGear, gearsCount - (neutralGear + 1), GUILayout.MaxWidth(200f));
                GUILayout.EndHorizontal();
                // previewGear = EditorGUILayout.IntSlider("Gear", previewGear, 0 - neutralGear, gearsCount - (neutralGear + 1));

                // float topAngularVelocity = (engine.MaxRPM * Engine.RPMToRad) / (gearbox.GearRatios[gearbox.GearRatios.Count - 1] * gearbox.MainGear);
                float topAngularVelocity = previewGear == 0 ? 0 : (engine.MaxRPM * Engine.RPMToRad) / (gearbox.GearRatios[previewGear + neutralGear] * gearbox.MainGear);
                // KM/H
                float topLinearVelocity = (driveWheelRadius * topAngularVelocity) * 3.6f;
                float roundedVelocity = Mathf.Floor(topLinearVelocity * 10f) / 10f;
                // EditorGUILayout.FloatField((driveWheelRadius * topAngularVelocity) * 3.6f);
                // EditorGUILayout.FloatField(driveWheelRadius);
                // GUILayout.FlexibleSpace();
                // EditorGUILayout.LabelField("Speed " + topLinearVelocity);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Speed");
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(roundedVelocity.ToString(), GUILayout.MaxWidth(200f));
                GUILayout.EndHorizontal();
                break;
                #endregion
        }
        #endregion
    }
}
