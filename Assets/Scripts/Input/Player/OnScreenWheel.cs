using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public sealed class OnScreenWheel : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Кажется не верно, похоже должен указываться максимальный угол, от левого края до правого
    [SerializeField] private float maxAngle = 310f;
    public float MaxAngle => maxAngle;

    private RectTransform wheel;
    private Vector2 center;

    private float targetAngle;
    public float outputAngle;
    private float prevAngle;

    private Vector2 lastPos;
    public float inputValue { get; private set; }

    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float centerSpeed = 12f;
    private float speed;

    private bool released;
    private bool dragging;

    [SerializeField] private float decreaseFriction = 0.05f;

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
        Vector2 center = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, wheel.position);
        lastPos = pos - center;
        released = false;
        dragging = true;
        targetAngle = outputAngle;
    }

    // Добавить опцию при которой будет ограничиваться последняя позиция указателя в радиус, что бы колесо вращалось в обратную сторону с момента его упора
    // Добавить обратное вращение руля
    // Наверное Добавить в контроллер авто методы вращения руля. Кнопка со сглаживанием вращения колёс, аналог без. Но кажется что в этом нету смысла ибо тогда у аналога будет преимущество с корости поворота колёс
    // Ещё исправить проблему с сильным вращением руля при вращении его прямо около центра, + его общую сглаженность вращения
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = eventData.position;
        Vector2 center = RectTransformUtility.WorldToScreenPoint(eventData.enterEventCamera, wheel.position);
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out center);

        /* float angle = Vector2.Angle(Vector2.up, pos - center); */
        // Debug.LogWarning(Vector2.SignedAngle(Vector2.up, pos - center));
        // Debug.LogWarning(angle * Mathf.Sign((pos - center).x));
        // Debug.LogWarning(new Vector3(angle, (pos - center).x, (pos - center).y));

        // Vector2 enterPos = pos - center;

        // Заменить на более простые функции, без возможных костылей
        Vector2 angleDirection = Quaternion.AngleAxis(-targetAngle, Vector3.forward) * Vector2.up;
        // float deltaAngle = -Vector2.SignedAngle(angleDirection, pos - center);
        float deltaAngle = -Vector2.SignedAngle(lastPos, pos - center);
        // ---
        lastPos = pos - center;

        targetAngle = Mathf.Clamp(targetAngle + deltaAngle, -maxAngle, maxAngle);
        inputValue = targetAngle / maxAngle;

        // Vector2 newAngle = Vector2.
        prevAngle = targetAngle;

        // Debug.LogWarning(angle);
        Debug.LogWarning(targetAngle);

        // outputAngle = targetAngle;

        UpdateSpriteRotation();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        released = true;
        dragging = false;
        targetAngle = 0f;
    }

    private void Update()
    {
        SimpleValue();
        SendValueToControl(inputValue);
        UpdateSpriteRotation();
        return;

        if (released)
        {
            // Very simple
            outputAngle = Mathf.LerpUnclamped(outputAngle, targetAngle, rotationSpeed * Time.deltaTime);

            // targetAngle = targetAngle - (Mathf.Sign(targetAngle) * centerSpeed * Time.deltaTime);
            // Добавить сопротивление, +- как с подвеской. И я думаю что можно убрать ограничитель скорости, а вот ограничитель предела стоит сделать
            // ---___
            // Неверно расчитывается направление
            speed = Mathf.Clamp(speed + (Mathf.Sign(outputAngle) * centerSpeed * Time.deltaTime), -100f, 100f);
            // А может и тут пробелма
            float deltaAngle = (outputAngle - speed);
            // Debug.LogWarning($"DeltaAngle {deltaAngle}");
            // Debug.LogWarning($"SignedSpeed {Mathf.Sign(speed) * speed}");
            float signedSpeed = Mathf.Sign(speed) * Mathf.Sign(outputAngle);
            Debug.LogWarning($"SignedSpeed {Mathf.Sign(speed) * Mathf.Sign(outputAngle)}");
            // Или тут
            // float friction = -Mathf.Min(deltaAngle, 0f) * decreaseFriction;
            float friction = -Mathf.Min(signedSpeed * deltaAngle, 0f) * decreaseFriction;
            speed = speed - friction;
            // outputAngle -= speed;

            if (targetAngle == 0f)
                released = false;
        }
        else if (dragging)
        {
            /* speed = speed + (Mathf.Sign(targetAngle) * rotationSpeed * Time.deltaTime);
            outputAngle = Mathf.Clamp(targetAngle + speed, -maxAngle, maxAngle); */

            // if (Mathf.Abs(targetAngle == maxAngle))
        }

        UpdateSpriteRotation();
    }

    private void SimpleValue()
    {
        float distance = targetAngle - outputAngle;
        // return -1/0/1
        float distanceSign = System.Math.Sign(distance);

        // float maxrawValue = maxAngle * 2f;
        float rawValue = 0f;
        // Speed in angle per second
        float releaseMaxSpeed = 5f;
        float pressMaxSpeed = 2.5f;

        float testSign = 1 - (-distanceSign * System.Math.Sign(outputAngle));
        // speed = testSign;
        // Release. Готово, даже как-то странно что оказалось так просто
        // speed = (Mathf.Max(-distanceSign, 0f) * releaseMaxSpeed);
        // This
        // Наверное можно заменить на знак из библиотеки Юньки, без нуля
        float insideSpeed = Mathf.Abs(distanceSign * -System.Math.Sign(outputAngle)) * releaseMaxSpeed;

        // Test new press
        // speed = Mathf.Max(distanceSign * (1 - Mathf.Sign(outputAngle)), 0f);
        float outsideSpeed = Mathf.Max(distanceSign * (System.Math.Sign(outputAngle) == 0 ? distanceSign : Mathf.Sign(outputAngle)), 0f) * pressMaxSpeed;
        // ---
        // Press. Готово, но нужно упростить, это слишком сложно
        // This
        // speed = ((1 - (Mathf.Max(-distanceSign * System.Math.Sign(outputAngle), 0f))) * Mathf.Abs(System.Math.Sign(outputAngle + targetAngle))) * pressMaxSpeed;
        // speed = (Mathf.Max(-distanceSign) * distanceSign * releaseMaxSpeed) + (Mathf.Max(distanceSign) * distanceSign * pressMaxSpeed);
        float currentMaxSpeed = (insideSpeed - outsideSpeed) * Mathf.Max(rawValue, 1f);
        speed = Mathf.Min(currentMaxSpeed, Mathf.Abs(distance)) * distanceSign;
        
        outputAngle = outputAngle + speed;
        inputValue = outputAngle / maxAngle;
    }

    private void UpdateSpriteRotation() => wheel.localRotation = Quaternion.Euler(0f, 0f, -outputAngle);
}
