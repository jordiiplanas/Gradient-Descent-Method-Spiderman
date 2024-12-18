using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientIKList : MonoBehaviour
{
    public List<Transform> joints;
    public Transform target;
    
    [Header("StartTransform")]
    public List<Vector3> startTransforms;
    public float lerpSpeed = 100.0f;
    
    bool wasInRange = false;

    
    [Header("Line Renderer")]
    public Material lineMaterial; // Material para las l�neas
    private List<LineRenderer> lineRenderers; // LineRenderers para cada segmento

    public float alpha = 1f;
    public float tolerance = 1f;

    private float costFunction;
    
    private float maxDistance; // Variable para almacenar la distancia total

    // Lista de vectores D para cada segmento (D[i] = posicionJoint[i+1] - posicionJoint[i])
    private List<Vector3> D = new List<Vector3>();

    // Arreglo de ángulos. Para cada segmento tenemos 2 ángulos.
    // Por ejemplo, si hay 4 joints, hay 3 segmentos y por ende 6 ángulos.
    private float[] theta;

    [Header("Clamp")]
    public Transform leftClamp;
    public Transform rightClamp;
    public Transform upClamp;

    public float angleRotation = 110.0f; 

    private Quaternion initialRotationLeftClamp;
    private Quaternion initialRotationRightClamp;
    private Quaternion initialRotationUpClamp;

    private bool areClosed = false;

    void Start()
    {
        InitializeLineRenderers();
        SaveStartPositions();
        CalculateMaxDistance();
        // Calcular vectores D a partir de las posiciones iniciales de los joints
        for (int i = 0; i < joints.Count - 1; i++)
        {
            D.Add(joints[i+1].position - joints[i].position);
        }

        // Inicializamos theta en cero
        theta = new float[(joints.Count - 1) * 2];
        for (int i = 0; i < theta.Length; i++)
        {
            theta[i] = 0f;
        }

        // Calcular costo inicial
        costFunction = lossCostFunction(theta);


        initialRotationLeftClamp = leftClamp.rotation;
        initialRotationRightClamp = rightClamp.rotation;
        initialRotationUpClamp = upClamp.rotation;
    }

    void Update()
    {
        bool inRange = (Vector3.Distance(joints[0].position, target.position) < maxDistance);
    
        if (!inRange && wasInRange)
        {
            // Si recién se salió de rango
            // Reiniciar theta
            for (int i = 0; i < theta.Length; i++)
            {
                theta[i] = 0f;
            }
        }

        if (inRange)
        {
            if (costFunction > tolerance)
            {
                float[] gradient = GetGradient(theta);
                for (int i = 0; i < theta.Length; i++)
                {
                    theta[i] -= alpha * gradient[i];
                }

                Vector3[] newPositions = endFactorFunction(theta);
                for (int i = 1; i < joints.Count; i++)
                {
                    joints[i].position = newPositions[i]; 
                }

                RotateEndEffectorTowardsTarget(joints[joints.Count - 1].position);
            }

            
        }
        else
        {
            ResetToInitialPositions();
        }

        if (!areClosed && Vector3.Distance(joints[joints.Count - 1].position, target.position) <= 2)
        {
            areClosed = true;
            CloseClamps();
        }
        else if (areClosed && Vector3.Distance(joints[joints.Count - 1].position, target.position) > 2)
        {
            areClosed = false;
            OpenClamps();
        }

        UpdateLineRenderers();

        costFunction = lossCostFunction(theta);
        wasInRange = inRange;
    }


    private void CloseClamps()
    {
        leftClamp.Rotate(angleRotation, 0f, 0f, Space.Self);
        rightClamp.Rotate(angleRotation, 0f, 0f, Space.Self);
        upClamp.Rotate(angleRotation, 0f, 0f, Space.Self);
    }

    private void OpenClamps()
    {
        leftClamp.Rotate(-angleRotation, 0f, 0f, Space.Self);
        rightClamp.Rotate(-angleRotation, 0f, 0f, Space.Self);
        upClamp.Rotate(-angleRotation, 0f, 0f, Space.Self);
    }

    /// <summary>
    /// Función forward kinematics. Dado el vector de ángulos theta, calcula la posición
    /// de todos los joints. El último elemento del array devuelto es el end effector.
    /// </summary>
    Vector3[] endFactorFunction(float[] theta)
    {
        // Suponiendo que cada segmento tiene dos ángulos: 
        // theta[2*i] rotación sobre Vector3.up
        // theta[2*i+1] rotación sobre Vector3.forward
        // q acumulativo
        Quaternion q = Quaternion.identity;

        Vector3[] positions = new Vector3[joints.Count];
        positions[0] = joints[0].position; // La posición del primer joint se asume fija.

        for (int i = 0; i < joints.Count - 1; i++)
        {
            // Aplicar las dos rotaciones correspondientes a este segmento
            Quaternion qUp = Quaternion.AngleAxis(theta[2*i], Vector3.up);
            Quaternion qForward = Quaternion.AngleAxis(theta[2*i+1], Vector3.forward);
            
            // Multiplicamos las rotaciones acumuladas
            q = q * qUp * qForward;

            // La nueva posición del joint i+1
            positions[i+1] = positions[i] + q * D[i];
        }

        return positions;
    }

    /// <summary>
    /// Calcula la función de costo: la distancia al cuadrado entre el end effector y el target.
    /// </summary>
    float lossCostFunction(float[] theta)
    {
        Vector3[] pos = endFactorFunction(theta);
        Vector3 endPosition = pos[joints.Count - 1];
        float dist = Vector3.Distance(endPosition, target.position);
        return dist * dist;
    }

    /// <summary>
    /// Calcula el gradiente numéricamente. 
    /// Ajusta cada componente de theta en un pequeño step y evalúa el cambio en la función de costo.
    /// </summary>
    float[] GetGradient(float[] theta)
    {
        float step = 1e-2f;
        float[] gradient = new float[theta.Length];

        float originalCost = lossCostFunction(theta);

        for (int i = 0; i < theta.Length; i++)
        {
            float originalVal = theta[i];

            // Incrementar theta[i] en step
            theta[i] = originalVal + step;
            float costPlus = lossCostFunction(theta);

            // Restaurar valor original
            theta[i] = originalVal;

            gradient[i] = (costPlus - originalCost) / step;
        }

        // Normalizar el gradiente
        float norm = 0f;
        for (int i = 0; i < gradient.Length; i++)
        {
            norm += gradient[i] * gradient[i];
        }
        norm = Mathf.Sqrt(norm);

        if (norm > 1e-6f)
        {
            for (int i = 0; i < gradient.Length; i++)
            {
                gradient[i] /= norm;
            }
        }

        return gradient;
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

        // Opcional: Imprimir la distancia total para depuraci�n
        Debug.Log($"Distancia total entre los joints: {maxDistance}");
    }
    
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
    
    private void ResetToInitialPositions()
    {
        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].position = Vector3.Lerp(joints[i].position, startTransforms[i], Time.deltaTime * lerpSpeed);
        }

        // Reiniciar theta a cero para forzar el recalculo completo cuando el target regrese a rango
        for (int i = 0; i < theta.Length; i++)
        {
            theta[i] = 0f;
        }

        Debug.Log("Joints restaurados a las posiciones iniciales y theta reiniciado.");
    }

    void RotateEndEffectorTowardsTarget(Vector3 endEffectorPosition)
    {
        // Calculamos la dirección desde el end effector hacia el target
        Vector3 directionToTarget = target.position - endEffectorPosition;

        // Si la dirección no es cero (para evitar errores de rotación si están en el mismo lugar)
        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            // Calculamos la rotación necesaria para que el end effector mire hacia el target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);  // Establecemos el "up" como Vector3.up

            // Aplicamos la rotación al último joint (end effector)
            joints[joints.Count - 1].rotation = targetRotation;
        }
    }

}
