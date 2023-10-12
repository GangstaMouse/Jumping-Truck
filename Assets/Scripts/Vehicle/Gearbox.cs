using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gearbox : MonoBehaviour
{
    [Header("Gearbox")]
    public float MainGear;
    public float GearChangeDelay = 0.3f;
    public List<float> GearRatios;
    public bool Automatic;
    public float ShiftUpRPM;
    public float ShiftDownRPM;






    private List<WheelCollider> driveWheels;
    
    private float physicsDeltaTime;

    // Думаю лучше переименовать
    public float totalGearRatio { get; private set; }

    private float engineVelocity;
    public float clutch;

    public int neutralGear { get; private set; }
    public int targetGear { get; private set; }
    public int gear { get; private set; }
    private float time;
    private float AutoShiftGearTime = 1f;
    private Engine engine;
    private float prevEngineRPM;
    private Vector3 relativeSpeed;
    private CarController car;

    private const float RPMToRad = (Mathf.PI * 2f) / 60f;
    private const float RadToRPM = 1f / RPMToRad;

    private float radius;
    // Возможно позже нужно будет это удалить
    public float ShiftGearTollerance = 200f;

    public event Action<int> OnShiftGear;

    public float outputTorque  { get; private set; }

    public void Initialize(CarController carController)
    {
        car = carController;
        driveWheels = car.driveWheels;
        engine = car.engine;

        // Переделать (0 gear всегда == neutral gear)
        // Возможно лучше перенести в Start
        /* for (int i = 0; i < GearRatios.Count - 1; i++)
        {
            if (GearRatios[i] == 0f)
            {
                neutralGear = i;
                // Исправить/Пределатьs
                targetGear = neutralGear;
                gear = neutralGear;
                // Обновить передачу? (Интерфейс)
                break;
            }
        } */
    }

    private void OnEnable()
    {
        neutralGear = GetNeutralGear();
        StartCoroutine(ShiftGear(neutralGear));
    }

    public int GetNeutralGear()
    {
        // for (int i = 0; i < GearRatios.Count - 1; i++)
        for (int i = 0; i < GearRatios.Count; i++)
            if (GearRatios[i] == 0f)
                return i;

        return 0;
    }

    public void Test()
    {
        Debug.Log("Ina, Kiara, Calli, Amelia, Gura");
    }

    public float GetOutputTorque(float inputTorque) => inputTorque * totalGearRatio;

    public float GetInputShaftVelocity(float outputShaftVelocity) => outputShaftVelocity * totalGearRatio;
    /* public float GetInputShaftVelocity(float outputShaftVelocity)
    {
        if (totalGearRatio == 0f)
            return 0;

        return outputShaftVelocity / totalGearRatio;
    } */

    private void Process(CarController carController)
    {

    }

    // Warning!!!! Проблема с тем что при переключении передач обороты движка не падают/повышаются, связанна с слишком большой мощности самого движка (много крутящего момента)
    public void AutomaticProcessor(float gasValue, float brakeValue)
    {
        if (!Automatic)
            return;
        // bool isReverse = targetGear < neutralGear;

        // float carDirection = gasValue + -brakeValue;
        // float carDirection = (isReverse ? -gasValue + brakeValue : gasValue + -brakeValue);
        float inputDirection = car.RawGasInput + -car.RawBrakeInput;
        // Debug.Log(carDirection);

        // Speed in M/s
        if (Mathf.Abs(car.relativeAirSpeed.z) <= 2f)
        {
            if (inputDirection != 0)
            {
                if (Mathf.Clamp(targetGear - neutralGear, -1, 1) != Mathf.Sign(inputDirection))
                {
                    StartCoroutine(ShiftGear(neutralGear + ((int)Mathf.Sign(inputDirection))));
                }
            }
        }

        float deltaTime = Time.fixedDeltaTime;
        float totalDriveWheelsSpeed = 0;

        foreach (var wheel in car.driveWheels)
            totalDriveWheelsSpeed += wheel.angularVelocity;

        // totalDriveWheelsSpeed /= car.driveWheels.Count;
        float meanDriveWheelsSpeed = totalDriveWheelsSpeed / car.driveWheels.Count;
        float clutchRPM = (engine.RPM + (totalDriveWheelsSpeed * RadToRPM)) / 2f;
        // Debug.Log(clutchRPM);
        // Debug.Log(totalDriveWheelsSpeed * RadToRPM);
        // Debug.Log(meanDriveWheelsSpeed * RadToRPM * totalGearRatio);

        // if (targetGear > neutralGear + 1 && targetGear < GearRatios.Count)

        // float differenceWheelFromEngine = (engine.RPM - (meanDriveWheelsSpeed * RadToRPM));
        // Debug.Log(new Vector2(engine.engineAngularVelocity, meanDriveWheelsSpeed * totalGearRatio));
        float differenceWheelFromEngine = (engine.engineAngularVelocity - (meanDriveWheelsSpeed * totalGearRatio));
        // Debug.Log(differenceWheelFromEngine);

        if (time < AutoShiftGearTime)
        {
            time += deltaTime;
            return;
        }

        if (targetGear <= neutralGear)
            return;

        if (engine.RPM >= ShiftUpRPM)
            StartCoroutine(ShiftGear(targetGear + 1));

        if (engine.RPM <= ShiftDownRPM  && targetGear > neutralGear + 1)
            StartCoroutine(ShiftGear(targetGear - 1));

        /* bool automaticCanShiftGear = time <= 0f;
        // float shiftGearDir = engine */

        /* if (targetGear > neutralGear && targetGear < GearRatios.Count)
        {
            if (time <= 0f)
            {
                // time -= AutoShiftGearTime;
                if (engine.RPM >= ShiftUpRPM - ShiftGearTollerance && gear == targetGear)
                {
                    StartCoroutine(ShiftGear(targetGear + 1));
                }

                if (engine.RPM <= ShiftDownRPM + ShiftGearTollerance && gear == targetGear && targetGear > neutralGear + 1)
                {
                    StartCoroutine(ShiftGear(targetGear - 1));
                }
            }
        } */
    }

    [System.Obsolete]
    public void AutomaticUpdate()
    {
        if (Automatic)
        {
            relativeSpeed = car.relativeAirSpeed;
            float dt = Time.deltaTime;
            time += dt;

            float engineAcceleration = engine.engineAngularVelocity - prevEngineRPM;

            // Debug.Log(engineAcceleration);
            float r = (MainGear * GearRatios[targetGear]);
            float topSpeedAtGear = ((engine.MaxRPM * RPMToRad) / r) * radius;
            float topSpeed = ((engine.MaxRPM * RPMToRad) / (MainGear * GearRatios[GearRatios.Count - 1])) * radius;

            float shiftUpSpeed = ((ShiftUpRPM * RPMToRad) / r) * radius;
            float shiftDownSpeed = ((ShiftDownRPM * RPMToRad) / r) * radius;
            // Debug.Log(new Vector4(relativeSpeed.z, shiftUpSpeed, topSpeedAtGear, topSpeed));

            // Debug.Log(((relativeSpeed.z / radius) * r) * RadToRPM);

            // Shift gear
            if (time >= AutoShiftGearTime)
            {
                // возможно не стоит Добавить погрешность в RPM. speed - погрешность
                time -= 1f;
                // if (engineAcceleration > 0f && engine.engineRPM >= ShiftUpRPM)
                if (car.GasInput > 0f && targetGear == neutralGear)
                {
                    StartCoroutine(ShiftGear(gear + 1));
                }

                if (relativeSpeed.z >= shiftUpSpeed && engine.RPM >= ShiftUpRPM)
                {
                    //  || targetGear == neutralGear && engineAcceleration > 0f
                    StartCoroutine(ShiftGear(gear + 1));
                }
                // if (engineAcceleration < 0f && engine.engineRPM <= ShiftDownRPM && targetGear > neutralGear + 1)
                if (relativeSpeed.z <= shiftDownSpeed && engine.RPM <= ShiftDownRPM && targetGear > neutralGear + 1)
                {
                    StartCoroutine(ShiftGear(gear - 1));
                }
                // StartCoroutine(ShiftGear(gear + 1));
            }

            prevEngineRPM = engine.engineAngularVelocity;
        }
    }

    [System.Obsolete]
    public void NewAutomaticUpdate()
    {
        if (Automatic)
        {
            if (targetGear > neutralGear)
            {
                if (engine.RPM >= ShiftUpRPM)
                {
                    StartCoroutine(ShiftGear(gear + 1));
                }
                if (engine.RPM <= ShiftDownRPM && gear > neutralGear + 1)
                {
                    StartCoroutine(ShiftGear(gear - 1));
                }
            }
        }
    }

    public void ClutchUpdate(float deltaTime, float inputTorque)
    {
        physicsDeltaTime = deltaTime;

        // Можно удалить
        engineVelocity = inputTorque;

        float acceleration = Mathf.Abs((inputTorque * totalGearRatio) * physicsDeltaTime) / driveWheels.Count;

        float totalAngularVelocity = 0;
        foreach (var wheel in driveWheels)
            totalAngularVelocity += wheel.angularVelocity;

        // totalAngularVelocity /= driveWheels.Count;
        float middleAngulartVelocity = totalAngularVelocity / driveWheels.Count;

        float maxSpeed = Mathf.Max(Mathf.Abs(engineVelocity / totalGearRatio) - Mathf.Abs(middleAngulartVelocity), 0f);
        float clutchSpeed = Mathf.Min(acceleration, maxSpeed) * Mathf.Sign(totalGearRatio);
        outputTorque = ((totalGearRatio != 0) ? clutchSpeed * clutch : 0f);

        /* foreach (var wheel in driveWheels)
        {
            wheel.inputTorque = outputTorque;
        } */
    }

    public void ClutchUpdatet(WheelCollider wheel, float newDeltaTime, float inputTorque, float engineVeloc)
    {
        physicsDeltaTime = newDeltaTime;

        // engineVelocity = inputTorque;
        engineVelocity = engineVeloc;

        // float acceleration = ((inputTorque * Mathf.Abs(totalGearRatio)) * deltaTime) / driveWheels.Count;
        float acceleration = Mathf.Abs((inputTorque * totalGearRatio) * physicsDeltaTime) / driveWheels.Count;

        // Debug.Log(totalGearRatio);

        foreach (var wheelD in driveWheels)
        {
            // Это либо заблокированный дефиренциал, толи открытый, объединить? (wheelD.DriveAxle) + (totalGearRatio != 0f)
            // Если totalGearRatio == 0f, то: wheel.inputTorque = 0f
            if (wheelD.DriveAxle)
            {
                if (totalGearRatio != 0f)
                {
                    // Открытый дифферинциал, а может быть и закрытый (В общем скорости колес усредняются)
                    // float maxSpeed = (engineVelocity / Mathf.Abs(totalGearRatio)) - Mathf.Abs(wheelD.angularVelocity);
                    // float maxSpeed = Mathf.Max((engineVelocity / Mathf.Abs(totalGearRatio)) - Mathf.Abs(wheelD.angularVelocity), 0f);
                    float maxSpeed = Mathf.Max(Mathf.Abs(engineVelocity / totalGearRatio) - Mathf.Abs(wheelD.angularVelocity), 0f);

                    float clutchSpeed = Mathf.Min(acceleration, maxSpeed) * Mathf.Sign(totalGearRatio);
                    // Debug.Log(clutchSpeed);
                    wheelD.inputTorque = clutchSpeed * clutch;
                    // wheelD.inputTorque = clutchSpeed;
                }
                else
                {
                    wheelD.inputTorque = 0f;
                }
            }

            //wheelD.UpdatePhysics(deltaTime);
        }




        // if (wheel.DriveAxle)
        // {
        //     if (totalGearRatio != 0f)
        //     {
        //         float acceleration = (inputTorque * totalGearRatio) * deltaTime;
        //         float maxSpeed;
        //     }
        //     else
        //     {
        //         wheel.inputTorque = 0f;
        //     }
        // }
    }

    public IEnumerator ShiftGear(int newGear)
    {
        if (newGear >= 0 && newGear < GearRatios.Count)
        {
            totalGearRatio = 0f;
            targetGear = newGear;
            time -= AutoShiftGearTime;

            OnShiftGear?.Invoke(targetGear);
            yield return new WaitForSeconds(GearChangeDelay);

            gear = targetGear;
            totalGearRatio = MainGear * GearRatios[gear];
        }
    }
}
