using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TireTrack : MonoBehaviour
{
    // Varibles
    public float Width = 0.5f;
    public int MaxMarksCount = 100;
    public float ThreasholdDistance = 0.1f;
    public Material Material;

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
    private List<Color> newColors = new List<Color>();

    private GameObject markGO;
    private Mesh mesh;

    private Vector3 prevPosition;
    private Quaternion prevRotation;
    private float totalUVLeght;
    private float prevAlpha;

    private ParticleSystem particle;



    private void Start()
    {
        markGO = new GameObject("TireMark", typeof(MeshFilter), typeof(MeshRenderer));

        mesh = new Mesh();

        markGO.GetComponent<MeshFilter>().mesh = mesh;
        markGO.GetComponent<MeshRenderer>().material = Material;

        // newVertices.Add(new Vector3(0f, 0f, 0f));
        // newVertices.Add(new Vector3(Width, 0f, 0f));
        newVertices.Add(transform.position - transform.right * (Width / 2f));
        newVertices.Add(transform.position + transform.right * (Width / 2f));

        mesh.SetVertices(newVertices);

        newUV.Add(new Vector2(0f, 0f));
        newUV.Add(new Vector2(1f, 0f));

        mesh.SetUVs(0, newUV);

        prevPosition = transform.position;

        newColors.Add(new Color(1f, 1f, 1f, 0f));
        newColors.Add(new Color(1f, 1f, 1f, 0f));

        mesh.SetColors(newColors);

        particle = GetComponent<ParticleSystem>();
        // Временно
        particle.Play();
    }

    // Временно
    private void Update()
    {
        // CreateNewSegment(1f);
    }

    public void ContinueSegment()
    {
        
    }

    // Возможно стоит создать Transform
    public void CreateNewSegment(float newAlpha)
    {
        ParticleSystem.MainModule mainModule = particle.main;
        // mainModule.startColor = new Color(1f, 1f, 1f, newAlpha);
        mainModule.startLifetime = newAlpha * 4f;

        // if (newAlpha != 0f)
        // {
        //     if (!particle.isPlaying)
        //     {
        //         particle.Play();
        //     }
        // }
        // else
        // {
        //     if (particle.isPlaying)
        //     {
        //         particle.Pause();
        //     }
        // }

        // Исправить, создавать новые следы исходя из условия (так как сейчас они быстро пропадают)

        if ((prevPosition - transform.position).magnitude >= ThreasholdDistance)
        {
            int newVertexIndex = newVertices.Count - 2;

            // newVertices.Add(new Vector3(0f, height, 0f));
            // newVertices.Add(new Vector3(Width, height, 0f));

            Vector3 vectorWidth = transform.right * (Width / 2);

            newVertices.Add(transform.position - vectorWidth);
            newVertices.Add(transform.position + vectorWidth);

            mesh.SetVertices(newVertices);

            newTriangles.Add(newVertexIndex + 3);
            newTriangles.Add(newVertexIndex + 1);
            newTriangles.Add(newVertexIndex);

            newTriangles.Add(newVertexIndex);
            newTriangles.Add(newVertexIndex + 2);
            newTriangles.Add(newVertexIndex + 3);

            mesh.SetTriangles(newTriangles, 0);

            // Исправить, Длина UV должна равняться длине нового участка (текущая позиция - предыдущая позиция)
            int lenghtUV = newVertices.Count / 2;
            float distance = (prevPosition - transform.position).magnitude;
            totalUVLeght += distance;

            // newUV.Add(new Vector2(0f, 1f));
            // newUV.Add(new Vector2(1f, 1f));
            // newUV.Add(new Vector2(0f, lenghtUV));
            // newUV.Add(new Vector2(1f, lenghtUV));
            newUV.Add(new Vector2(0f, totalUVLeght));
            newUV.Add(new Vector2(1f, totalUVLeght));

            mesh.SetUVs(0, newUV);

            // Debug.Log(newVertices.Count / 4);

            newColors.Add(new Color(1f, 1f, 1f, newAlpha));
            newColors.Add(new Color(1f, 1f, 1f, newAlpha));

            mesh.SetColors(newColors);

            if (Mathf.Ceil(newVertices.Count / 4) > MaxMarksCount)
            {
                int last = newTriangles.Count - 6;
                newTriangles.RemoveRange(last, 6);
                mesh.SetTriangles(newTriangles, 0);

                newVertices.RemoveRange(0, 2);
                mesh.SetVertices(newVertices);

                // newUV.RemoveRange((lenghtUV * 2) - 2, 2);
                newUV.RemoveRange(0, 2);
                mesh.SetUVs(0, newUV);

                newColors.RemoveRange(0, 2);
                mesh.SetColors(newColors);
            }

            mesh.RecalculateNormals();

            prevPosition = transform.position;

            if (totalUVLeght >= 1000f)
            {
                // Имеется небольшой графический артефакт, его легко можно скрыть - просто создав ещё один сегмент, сразу же после обнуления длины UV
                totalUVLeght -= 1000f;
            }
        }
    }

    private void OnDestroy() => Destroy(markGO);
}
