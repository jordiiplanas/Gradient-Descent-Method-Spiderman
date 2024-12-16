using System.Collections.Generic;
using UnityEngine;

public class CCDFromTop : MonoBehaviour
{
    [Header("Joints")]
    public List<Transform> joints; // Lista de joints del tentáculo
    public List<Vector3> links;    // Longitudes de los links

    [Header("Target")]
    public Transform target;       // Posición objetivo

    [Header("CCD Parameters")]
    public float tolerance = 0.1f; // Tolerancia para detener el algoritmo
    public int maxIterations = 100; // Número máximo de iteraciones

    [Header("Line Renderer")]
    public Material lineMaterial; // Material para las líneas
    private List<LineRenderer> lineRenderers; // LineRenderers para cada segmento

    [Header("StartTransform")]
    public List<Vector3> startTransforms;
    public float lerpSpeed = 5.0f;

    private float maxDistance; // Variable para almacenar la distancia total

    private void Start()
    {
        CalculateMaxDistance();
        SaveStartPositions();
        SetLinks(); // Configurar los links iniciales
        InitializeLineRenderers(); // Configurar los LineRenderers
    }

    private void Update()
    {

        if (Vector3.Distance(joints[0].position, target.position) < maxDistance)
        {
            PerformCCD(); // Ejecutar el CCD en cada frame
        }
        else
        {
            ResetToInitialPositions();
        }

        UpdateLineRenderers(); // Actualizar las líneas
    }

    /// <summary>
    /// Calcula los links entre los joints iniciales.
    /// </summary>
    private void SetLinks()
    {
        links.Clear();
        for (int i = 1; i < joints.Count; i++)
        {
            links.Add(joints[i].position - joints[i - 1].position);
        }
    }

    /// <summary>
    /// Ejecuta el algoritmo de CCD para mover el tentáculo hacia el objetivo.
    /// </summary>
    private void PerformCCD()
    {
        int iterations = 0;

        while (Vector3.Distance(joints[^1].position, target.position) > tolerance && iterations < maxIterations)
        {
            // Recorre los joints desde el último hacia el primero
            for (int i = joints.Count - 2; i >= 0; i--)
            {
                Vector3 toEndEffector = joints[^1].position - joints[i].position;
                Vector3 toTarget = target.position - joints[i].position;

                // Calcular el ángulo y el eje de rotación
                float angle = Vector3.Angle(toEndEffector, toTarget);
                Vector3 axis = Vector3.Cross(toEndEffector, toTarget).normalized;

                // Aplicar la rotación al joint actual
                joints[i].rotation = Quaternion.AngleAxis(angle, axis) * joints[i].rotation;

                // Actualizar posiciones de los children
                UpdateChildJoints(i);
            }

            iterations++;
        }
    }

    /// <summary>
    /// Actualiza las posiciones de los joints hijos en base al padre rotado.
    /// </summary>
    /// <param name="startIndex">Índice del joint desde el cual actualizar.</param>
    private void UpdateChildJoints(int startIndex)
    {
        for (int i = startIndex + 1; i < joints.Count; i++)
        {
            joints[i].position = joints[i - 1].position + joints[i - 1].TransformDirection(links[i - 1]);
        }
    }

    /// <summary>
    /// Inicializa los LineRenderers para conectar cada joint con el siguiente.
    /// </summary>
    private void InitializeLineRenderers()
    {
        lineRenderers = new List<LineRenderer>();

        for (int i = 0; i < joints.Count - 1; i++)
        {
            // Crear un objeto para el LineRenderer
            GameObject lineObj = new GameObject($"LineRenderer_{i}");
            lineObj.transform.parent = this.transform;

            // Agregar el componente LineRenderer
            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = lineMaterial;
            lineRenderer.startWidth = 0.3f;
            lineRenderer.endWidth = 0.3f;
            lineRenderer.positionCount = 2; // Dos puntos: inicio y fin

            lineRenderers.Add(lineRenderer);
        }
    }

    /// <summary>
    /// Actualiza las posiciones de los LineRenderers para reflejar las posiciones actuales de los joints.
    /// </summary>
    private void UpdateLineRenderers()
    {
        for (int i = 0; i < joints.Count - 1; i++)
        {
            if (lineRenderers[i] != null)
            {
                // Conectar cada joint con el siguiente
                lineRenderers[i].SetPosition(0, joints[i].position);
                lineRenderers[i].SetPosition(1, joints[i + 1].position);
            }
        }
    }

    private void SaveStartPositions()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            startTransforms.Add(joints[i].position);
        }
    }

    private void CalculateMaxDistance()
    {
        maxDistance = 0f; // Inicializar la distancia a cero

        for (int i = 1; i < joints.Count; i++)
        {
            // Calcular la distancia entre el joint actual y el anterior
            float distance = Vector3.Distance(joints[i].position, joints[i - 1].position);

            // Sumar al total
            maxDistance += distance;
        }

        // Opcional: Imprimir la distancia total para depuración
        Debug.Log($"Distancia total entre los joints: {maxDistance}");
    }

    private void ResetToInitialPositions()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].position = Vector3.Lerp(joints[i].position, startTransforms[i], Time.deltaTime * lerpSpeed);
        }

        Debug.Log("Joints restaurados a las posiciones iniciales.");
    }
}
