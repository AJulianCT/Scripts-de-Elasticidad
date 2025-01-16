using UnityEngine;

public class ObjectReconstruction : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private Vector3[] currentVertices;
    
    private float recoveryTime = 3f; // Tiempo de recuperación (3 segundos)
    private float recoveryTimer = 0f; // Temporizador que controla el tiempo de recuperación
    private bool isDeformed = false; // Para saber si el objeto se ha deformado

    void Start()
    {
        // Obtenemos el MeshFilter del objeto
        meshFilter = GetComponent<MeshFilter>();
        
        // Guardamos la malla original y los vértices originales
        originalMesh = Instantiate(meshFilter.mesh);
        originalVertices = originalMesh.vertices;
        currentVertices = meshFilter.mesh.vertices;
    }

    void Update()
    {
        // Si el objeto ha sido deformado, iniciamos el proceso de recuperación gradual
        if (isDeformed)
        {
            recoveryTimer += Time.deltaTime;
            
            // Interpolamos los vértices entre la forma deformada y la original
            float recoveryProgress = Mathf.Min(recoveryTimer / recoveryTime, 1f);
            InterpolateVertices(recoveryProgress);
            
            // Si ha pasado el tiempo de recuperación, restauramos la malla
            if (recoveryTimer >= recoveryTime)
            {
                isDeformed = false;
                recoveryTimer = 0f; // Reiniciamos el temporizador
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detectamos una colisión para marcar que el objeto se ha deformado
        if (!isDeformed)
        {
            isDeformed = true;
            recoveryTimer = 0f; // Reiniciamos el temporizador al momento del impacto
        }
    }

    void InterpolateVertices(float progress)
    {
        // Interpolamos entre los vértices actuales y los vértices originales
        for (int i = 0; i < originalVertices.Length; i++)
        {
            // Interpolamos la posición de los vértices
            currentVertices[i] = Vector3.Lerp(meshFilter.mesh.vertices[i], originalVertices[i], progress);
        }

        // Aplicamos la interpolación de los vértices de vuelta a la malla
        meshFilter.mesh.vertices = currentVertices;
        meshFilter.mesh.RecalculateNormals(); // Recalcular normales para una correcta visualización
    }
}