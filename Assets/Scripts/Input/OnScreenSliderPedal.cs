using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

// Переименовать в OnScreenAnalogButton
public sealed class OnScreenSliderPedal : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float pressSpeed = 1f;
    [SerializeField] private float releaseSpeed = 1f;
    [SerializeField] private float inertia = 1f;
    [SerializeField] private float maxSpeed = 0.1f;

    [SerializeField] private Image maskImage;

    // Реализовать
    private float rawValue;
    private float targetValue;

    private RectTransform wheel;
    private Vector2 center;
    Vector2 point;
    Vector2 bot;
    private float scale = 1f;

    private float speed;
    private float prevSpeed;

    // [SerializeField] private bool flipValue = false;

    public float OutputValue { get; private set; }

    [InputControl(layout = "Axis")]
    [SerializeField] private string controll;

    protected override string controlPathInternal { get => controll; set => controll = value; }

    private void Awake()
    {
        wheel = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        center = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, wheel.position);

        // Может быть проблема если оригинальное разрешение будет другим, тогда лучше брать скейл трансформа
        scale = Mathf.Min(Screen.width / 1920f, Screen.height / 1080f);

        GetTargetValue(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        center = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, wheel.position);

        // targetValue = Mathf.Clamp((pos - center).y, 0f, 1f);
        // point = (pos - center);
        point = pos;
        Vector2Int bottom = Vector2Int.CeilToInt(RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, wheel.position - (wheel.up * (wheel.rect.size.y / 2f) * scale)));
        Debug.LogWarning(bottom);
        // Неверно расчитывается направление слайдера
        targetValue = Mathf.Clamp((pos - bottom).y / (wheel.rect.height * scale), 0f, 1f);
        bot = bottom;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetValue = 0f;
    }

    private void GetTargetValue(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        Vector2Int bottom = Vector2Int.CeilToInt(RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, wheel.position - (wheel.up * (wheel.rect.size.y / 2f) * scale)));
        targetValue = Mathf.Clamp((pos - bottom).y / (wheel.rect.height * scale), 0f, 1f);
    }

    private void Update()
    {
        /* if (OutputValue == targetValue)
            return; */

        SimpleValue();
        SendValueToControl(OutputValue);
        // SendValueToControl(OutputValue * (flipValue ? -1f : 1f));
        // Simple();
        // UpdateValue();

        UpdateMask();
        return;
        
        // OutputValue = targetValue;

        float speedSign = Mathf.Sign(targetValue - OutputValue);
        // speed += speedSign * pressSpeed * Time.deltaTime;
        float max = targetValue - OutputValue;
        float acceleration = speedSign * pressSpeed * Time.deltaTime * Mathf.Abs(max);
        float deacceleration = Mathf.Clamp(speed, -0.01f, 0.01f) / inertia * Time.deltaTime;
        /* float acceleration = speedSign * max * pressSpeed * Time.deltaTime;
        float deacceleration = speedSign * ((Mathf.Abs(max) * -1f) + 1f) * inertia * Time.deltaTime; */
        // Заменить кламп на уменьшение скорости. думаю Уменьшение скорости стоит попробовать поделить на инерцию
        // speed = Mathf.Clamp(speed + (speedSign * pressSpeed * Time.deltaTime), max * -speedSign, max * speedSign);
        speed += acceleration - deacceleration;

        // New, 
        // speed = Mathf.Clamp(speed + acceleration - deacceleration, -maxSpeed, maxSpeed);

        // speed -= (targetValue - OutputValue) * releaseSpeed * Time.deltaTime;
        // speed = speed * 0.9f;
        // speed = speed - (1f * Time.deltaTime);
        OutputValue = Mathf.Clamp(OutputValue + speed, 0f, 1f);
        // OutputValue = Mathf.SmoothStep(OutputValue, targetValue, pressSpeed * Time.deltaTime);

        UpdateMask();
    }

    private void Simple()
    {
        float distance = targetValue - OutputValue;
        float distanceSign = Mathf.Sign(distance);
        float speedSign = Mathf.Sign(speed);
        float deltaSpeed = speed - prevSpeed;

        float acceleration = distanceSign * pressSpeed * Time.deltaTime;
        // speed += acceleration;
        // speed = 0.4f;
        // target = 0.2f
        // output = 0.6f

        // maxSpeed = output - target выбор по min, max

        speed = Mathf.Clamp(speed + acceleration, 0f, targetValue);
        
        prevSpeed = speed;

        if (speedSign > 0)
        {
        }
        else
        {

        }
    }

    private void SimpleValue()
    {
        float distance = targetValue - OutputValue;
        // return -1/1
        // float distanceSign = Mathf.Sign(distance);
        // return -1/0/1
        float distanceSign = System.Math.Sign(distance);

        // float rawValue = 2f;
        float releaseMaxSpeed = 0.1f;
        float pressMaxSpeed = 0.05f;
        // Попробовать заменить distance на distanceSign, но с нулём
        // Sign -1/1
        // float currentMaxSpeed = Mathf.Abs((Mathf.Max(-distance) * releaseMaxSpeed) + (Mathf.Max(distance) * pressMaxSpeed)) * Mathf.Max(rawValue, 1f);
        // Sign -1/0/1, constant max speed
        // Исправить! Max, Min без указания второстипенного аргумента просто не работают!
        // Хотя тут вроде как всё работает верно и за знака с нулём. Тогда лучше убрать Max
        // Эм, что то я не понял, вроде он необходим
        float currentMaxSpeed = Mathf.Abs((Mathf.Max(-distanceSign) * releaseMaxSpeed) + (Mathf.Max(distanceSign) * pressMaxSpeed)) * Mathf.Max(rawValue, 1f);
        speed = Mathf.Min(currentMaxSpeed, Mathf.Abs(distance)) * distanceSign;
        
        OutputValue = Mathf.Clamp(OutputValue + speed, 0f, 1f);
    }

    private void UpdateValue()
    {
        float distance = targetValue - OutputValue;
        float distanceSign = Mathf.Sign(distance);
        float speedSign = Mathf.Sign(speed);

        /* float acceleration = distanceSign * pressSpeed * Time.deltaTime * Mathf.Abs(distance);
        // float deacceleration = 
        // speed += acceleration;
        speed = Mathf.Min(Mathf.Abs(speed + acceleration), Mathf.Abs(distance)) * speedSign; */

        // distance = 0.6
        // speed = 0.5
        // acceleration = 0.2
        // s + a = -0.7

        /* float acceleration = distanceSign * pressSpeed * Time.deltaTime;
        // speed += Mathf.Abs(distance) - (speed + Mathf.Max(acceleration, 0f));
        // speed = Mathf.Min(speed + Mathf.Max(speed + acceleration, 0f), distance); */
        /* float newSpeed = speed + acceleration - Mathf.Max((speed + acceleration) - (Mathf.Sign(speed) * Mathf.Abs(distance)), 0f);
        speed = Mathf.Clamp(newSpeed, -maxSpeed, maxSpeed); */

        float acceleration = distanceSign * pressSpeed * Time.deltaTime * Mathf.Abs(distance);
        float deacceleration = (speedSign * ((Mathf.Abs(distance) * -1f) + 1f) * inertia) / Time.deltaTime;
        speed += acceleration - deacceleration;

        OutputValue = Mathf.Clamp(OutputValue + speed, 0f, 1f);
    }

    private void UpdateMask()
    {
        // maskRectTransform.localScale = new Vector3(maskRectTransform.localScale.x, OutputValue, 1f);
        maskImage.fillAmount = OutputValue;
    }
}
