using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabrikConstraints : MonoBehaviour
{
    public List<Transform> Joints;
    public Transform target;
    public float tolerance = 1.0f;
    public float maxIterations = 1e5f;
    private float lambda;
    private Vector3[] Links;
    private int countIterations;
    private int numberOfJoints;
    private Vector3 initialPosition;

    public float angle = Mathf.PI / 4;

    // Start is called before the first frame update
    void Start()
    {
        numberOfJoints = Joints.Count;
        getLinks();
        initialPosition = Joints[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (countIterations < maxIterations &&
            Vector3.Distance(Joints[numberOfJoints - 1].position, target.position) > tolerance)

        {
            Forward();
            Backward();

            countIterations++;
        }
    }
    void getLinks() {
        Links = new Vector3[numberOfJoints - 1];
        for (int i = 0; i < numberOfJoints - 1; i++) {
            Links[i] = Joints[i + 1].position - Joints[i].position;
        }
    }

    void Forward()
    {
        Joints[numberOfJoints - 1].position = target.position;
        for (int i = numberOfJoints - 2; i >= 0; i--) {

            if (i <= numberOfJoints - 3)
            {
                Joints[i].position = rotationConstrains(Joints[i + 1].position, 
                                                 Joints[i + 2].position, 
                                                    Joints[i].position);
            }
            
            float distance = Vector3.Magnitude(Links[i]);
            float denominator = Vector3.Distance(Joints[i].position, Joints[i + 1].position);
            lambda = distance / denominator;
            Vector3 temp = lambda * Joints[i].position + (1 - lambda) * Joints[i + 1].position;
            Joints[i].position = temp;

        }
    }

    void Backward()
    {
        Joints[0].position = initialPosition;

        for (int i = 1; i < numberOfJoints; i++) {

            // if (i >= 2)
            // {
            //     Joints[i].position = rotationConstrains(Joints[i-1].position, Joints[i-2].position, Joints[i].position);
            // }
            
            float distance = Vector3.Magnitude(Links[i - 1]);
            float denominator = Vector3.Distance(Joints[i - 1].position, Joints[i].position);
            lambda = distance / denominator;
            Vector3 temp = lambda * Joints[i].position + (1 - lambda) * Joints[i - 1].position;
            Joints[i].position = temp;
        }
        
        
    }

    Vector3 rotationConstrains(Vector3 joint, Vector3 previousJoint, Vector3 nextJoint)
    {
        Vector3 direction = (joint - previousJoint).normalized;
        Vector3 orthogonal = Vector3.Dot(direction, nextJoint) * direction;
        Vector3 nextJoinTraslated = nextJoint - orthogonal;
        float cos = Mathf.Clamp(Vector3.Dot(Vector3.up, direction), -1.0f, 1.0f);
        float rotation = Mathf.Acos(cos);

        Vector3 axis = Vector3.Cross(orthogonal, Vector3.up).normalized;
            
        Quaternion q = Quaternion.AngleAxis(rotation, axis);
        Vector3 nextJointRotated = q * nextJoinTraslated;
        
        float S = Vector3.Magnitude(orthogonal-joint);
        float distance = S * Mathf.Tan(angle);

        if (Vector3.Magnitude(nextJointRotated) > distance)
        {
            Vector3 newPosition = new Vector3(distance, 0, 0);
            if (nextJointRotated.x < 0)
            {
                newPosition.x *= -1;
            }
            return Quaternion.AngleAxis(-rotation, axis) * newPosition;
        }
        return nextJoint;
    
    }
}
