using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraSwitcher : MonoBehaviour
{
    // Start is called before the first frame update
    // Lista de cámaras para cambiar entre ellas
    public Camera[] cameras;

    // Variable para almacenar la cámara activa
    private int currentCameraIndex;

    // Start es llamado al inicio
    void Start()
    {
        // Asegurarse de que la lista de cámaras esté llena
        if (cameras.Length == 0)
        {
            Debug.LogError("No hay cámaras asignadas.");
            return;
        }

        // Inicializar la primera cámara activa
        SwitchCamera(0);
    }

    // Update es llamado una vez por frame
    void Update()
    {
        // Verifica las teclas presionadas para cambiar de cámara
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

    // Método para cambiar de cámara
    void SwitchCamera(int cameraIndex)
    {
        // Asegurarse de que el índice sea válido
        if (cameraIndex >= 0 && cameraIndex < cameras.Length)
        {
            // Desactivar todas las cámaras
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }

            // Activar la cámara seleccionada
            cameras[cameraIndex].gameObject.SetActive(true);
            currentCameraIndex = cameraIndex;
        }
        else
        {
            Debug.LogWarning("Índice de cámara fuera de rango.");
        }
    }
}
