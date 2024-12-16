using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Multiple : MonoBehaviour
{

    public List<Transform> Joints1;
    public List<Transform> Joints2;
    public List<Transform> JointsSB;

    public Transform target1;
    public Transform target2;
    private Vector3 sbtarget;

    public float tolerance = 1f;
    public float maxIterations = 1e5f;
    private int countIterations;

    private Vector3[] Links1;
    private Vector3[] Links2;
    private Vector3[] SBLinks;

    private Vector3 initialRootPosition;
    private Vector3 subbase1;
    private Vector3 subbase2;


    // Start is called before the first frame update
    void Start()
    {
        Links1 = getLinks(Joints1);
        Links2 = getLinks(Joints2);
        SBLinks = getLinks(JointsSB);
        initialRootPosition = JointsSB[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (countIterations < maxIterations &&
            (Vector3.Distance(target1.position, Joints1.LastOrDefault().position)> tolerance 
            || Vector3.Distance(target2.position, Joints2.LastOrDefault().position)>tolerance)) {


            firstStage(); //Forward process of all joints
            secondStage(); //Backward process of all joints

            countIterations++;

        }
    }

    Vector3[] getLinks(List<Transform> Joints)
    {

        int numberOfJoints = Joints.Count;

        Vector3[] Links = new Vector3[numberOfJoints-1];

        for (int i = 0; i < numberOfJoints-1; i++) {
            Links[i] = Joints[i+1].position - Joints[i].position;
        }
        return Links;
    }

    void Forward(List<Transform> Joints, Vector3 target, Vector3[] Links) 
    { 
        
        int numberOfJoints = Joints.Count;
        Joints[numberOfJoints-1].position = target;
        int index = numberOfJoints - 2;

        for (int i = index; i > -1; i--) {

            float distance = Vector3.Magnitude(Links[i]);
            float denomitator = Vector3.Distance(Joints[i].position, Joints[i + 1].position);
            float lambda = distance/denomitator;
            Vector3 temp = lambda * Joints[i].position + (1- lambda) * Joints[i+1].position;
            Joints[i].position = temp;
        
        } 
    }

    void Backward(List<Transform> Joints, Vector3[] Links, Vector3 rootPostion) { 

        int numberOfJoints = Joints.Count;
        Joints[0].position = rootPostion;

        for (int i = 1; i < numberOfJoints; i++) {
            float distance = Vector3.Magnitude(Links[i-1]);
            float denominator = Vector3.Distance(Joints[i].position, Joints[i - 1].position);
            float lambda = distance/denominator;

            Vector3 temp = lambda* Joints[i].position + (1 - lambda) * Joints[i-1].position;
            Joints[i].position = temp;     
        
        }
      
    }

    void centroidSubBasePosition() {
        subbase1 = Joints1[0].position;
        subbase2 = Joints2[0].position;
        sbtarget = subbase1 + (subbase2 - subbase1)/2;
    }


    void firstStage() {

        Forward(Joints1, target1.position, Links1);
        Forward(Joints2, target2.position, Links2);
        centroidSubBasePosition();
        Forward(JointsSB, sbtarget, SBLinks);
    
    }

    void secondStage() {

        Backward(JointsSB, SBLinks, initialRootPosition);
        Backward(Joints1, Links1, JointsSB.LastOrDefault().position);
        Backward(Joints2, Links2, JointsSB.LastOrDefault().position);
    
    }

}
