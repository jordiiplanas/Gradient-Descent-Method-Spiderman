using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    // Lista de c�maras para cambiar entre ellas
    public Camera[] cameras;

    // Variable para almacenar la c�mara activa
    private int currentCameraIndex;

    // Start es llamado al inicio
    void Start()
    {
        // Asegurarse de que la lista de c�maras est� llena
        if (cameras.Length == 0)
        {
            Debug.LogError("No hay c�maras asignadas.");
            return;
        }

        // Inicializar la primera c�mara activa
        SwitchCamera(0);
    }

    // Update es llamado una vez por frame
    void Update()
    {
        // Verifica las teclas presionadas para cambiar de c�mara
        if (Input.GetKeyDown(KeyCode.Alpha1))  // Tecla 1
        {
            SwitchCamera(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))  // Tecla 2
        {
            SwitchCamera(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))  // Tecla 3
        {
            SwitchCamera(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))  // Tecla 4
        {
            SwitchCamera(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))  // Tecla 4
        {
            SwitchCamera(4);
        }
    }

    // M�todo para cambiar de c�mara
    void SwitchCamera(int cameraIndex)
    {
        // Asegurarse de que el �ndice sea v�lido
        if (cameraIndex >= 0 && cameraIndex < cameras.Length)
        {
            // Desactivar todas las c�maras
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }

            // Activar la c�mara seleccionada
            cameras[cameraIndex].gameObject.SetActive(true);
            currentCameraIndex = cameraIndex;
        }
        else
        {
            Debug.LogWarning("�ndice de c�mara fuera de rango.");
        }
    }
}
