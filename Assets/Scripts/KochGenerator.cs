using System;
using System.Collections.Generic;
using UnityEngine;

public class KochGenerator : MonoBehaviour {

    public struct LineSegement
    {
        public Vector3 StartPosition { get; set; }
        public Vector3 EndPosition { get; set; }
        public Vector3 Direction { get; set; }
        public float Length { get; set; }
    }

    [System.Serializable]
    public struct StartGeneration
    {
        public bool outwards;
        public float scale;
    }

    public StartGeneration[] startGenerations;

    protected enum Initiator { Triangle, Square, Pentagon, Hexagon, Heptagon, Octagon };
    protected enum Axis { X, Y, Z };

    [SerializeField]
    protected Initiator initiator = new Initiator();
    [SerializeField]
    protected Axis axis = new Axis();
    [SerializeField]
    protected AnimationCurve generator;
    [SerializeField]
    protected float initiatorSize;
    public float lengthOfSides;
    [SerializeField]
    protected bool useBezierCurves;
    [SerializeField]
    [Range(8, 24)]
    protected int bezierVertexCount;
    protected Keyframe[] keys;
    protected Vector3[] position, targetPosition, bezierPosition;
    protected int initatorPointAmount, generationCount;

    private Vector3[] initiatorPoint;
    private List<LineSegement> lineSegements;
    private Vector3 rotateVector, rotateAxis;
    private float initialRotation;

    protected Vector3[] BezierCurve(Vector3[] points, int vertexCount)
    {
        var pointList = new List<Vector3>();

        for(int i = 0; i < points.Length; i += 2)
        {
            if (i + 2 <= points.Length - 1)
            {
                for(float ratio = 0f; ratio <= 1.0f; ratio += 1.0f / vertexCount)
                {
                    var tangentLineVert1 = Vector3.Lerp(points[i], points[i + 1], ratio);
                    var tangentLineVert2 = Vector3.Lerp(points[i+1], points[i + 2], ratio);
                    var bezierPoint = Vector3.Lerp(tangentLineVert1, tangentLineVert2, ratio);
                    pointList.Add(bezierPoint);
                }
            }
        }

        return pointList.ToArray();
    }

    private void OnDrawGizmos()
    {
        GetInitiatorPoints();
        initiatorPoint = new Vector3[initatorPointAmount];

        rotateVector = Quaternion.AngleAxis(initialRotation, rotateAxis) * rotateVector;

        for (int i = 0; i < initatorPointAmount; i++)
        {
            initiatorPoint[i] = rotateVector * initiatorSize;
            rotateVector = Quaternion.AngleAxis(360 / initatorPointAmount, rotateAxis) * rotateVector;
        }

        for(int i = 0; i < initatorPointAmount; i++)
        {
            Gizmos.color = Color.white;
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;
            if(i < initatorPointAmount - 1)
            {
                Gizmos.DrawLine(initiatorPoint[i], initiatorPoint[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(initiatorPoint[i], initiatorPoint[0]);
            }
        }
        lengthOfSides = Vector3.Distance(initiatorPoint[0], initiatorPoint[1]) * 0.5f;
    }

    private void Awake()
    {
        GetInitiatorPoints();

        position = new Vector3[initatorPointAmount + 1];
        targetPosition = new Vector3[initatorPointAmount + 1];
        lineSegements = new List<LineSegement>();
        keys = generator.keys;

        rotateVector = Quaternion.AngleAxis(initialRotation, rotateAxis) * rotateVector;

        for (int i = 0; i < initatorPointAmount; i++)
        {
            position[i] = rotateVector * initiatorSize;
            rotateVector = Quaternion.AngleAxis(360 / initatorPointAmount, rotateAxis) * rotateVector;
        }

        position[initatorPointAmount] = position[0];
        targetPosition = position;

        for(int i = 0; i < startGenerations.Length; i++)
        {
            GenerateKoch(targetPosition, startGenerations[i].outwards, startGenerations[i].scale);
        }
    }

    protected void GenerateKoch(Vector3[] positions, bool outwards, float generatorMultiplier)
    {
        lineSegements.Clear();

        for(int i = 0; i < positions.Length - 1; i++)
        {
            LineSegement line = new LineSegement();
            line.StartPosition = positions[i];
            if(i == positions.Length - 1)
            {
                line.EndPosition = positions[0];
            }
            else
            {
                line.EndPosition = positions[i + 1];
            }
            line.Direction = Vector3.Normalize(line.EndPosition - line.StartPosition);
            line.Length = Vector3.Distance(line.EndPosition, line.StartPosition);
            lineSegements.Add(line);
        }

        List<Vector3> newPos = new List<Vector3>();
        List<Vector3> targetPos = new List<Vector3>();

        for(int i = 0; i < lineSegements.Count; i++)
        {
            newPos.Add(lineSegements[i].StartPosition);
            targetPos.Add(lineSegements[i].StartPosition);

            for(int j = 1; j < keys.Length - 1; j++)
            {
                float moveAmount = lineSegements[i].Length * keys[j].time;
                float heightAmount = (lineSegements[i].Length * keys[j].value) * generatorMultiplier;
                Vector3 movePos = lineSegements[i].StartPosition + (lineSegements[i].Direction * moveAmount);
                Vector3 d;
                if (outwards)
                {
                    d = Quaternion.AngleAxis(-90, rotateAxis) * lineSegements[i].Direction;
                }
                else
                {
                    d = Quaternion.AngleAxis(90, rotateAxis) * lineSegements[i].Direction;
                }
                newPos.Add(movePos);
                targetPos.Add(movePos + (d * heightAmount));
            }
        }
        newPos.Add(lineSegements[0].StartPosition);
        targetPos.Add(lineSegements[0].StartPosition);
        position = new Vector3[newPos.Count];
        targetPosition = new Vector3[targetPos.Count];
        position = newPos.ToArray();
        targetPosition = targetPos.ToArray();
        bezierPosition = BezierCurve(targetPosition, bezierVertexCount);
        generationCount++;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GetInitiatorPoints()
    {
        switch (initiator)
        {
            case Initiator.Triangle:
                initatorPointAmount = 3;
                initialRotation = 0;
                break;
            case Initiator.Square:
                initatorPointAmount = 4;
                initialRotation = 45;
                break;
            case Initiator.Pentagon:
                initatorPointAmount = 5;
                initialRotation = 36;
                break;
            case Initiator.Hexagon:
                initatorPointAmount = 6;
                initialRotation = 30;
                break;
            case Initiator.Heptagon:
                initatorPointAmount = 7;
                initialRotation = 25.71428f;
                break;
            case Initiator.Octagon:
                initatorPointAmount = 8;
                initialRotation = 22.5f;
                break;
            default:
                initatorPointAmount = 3;
                initialRotation = 0;
                break;
        };

        switch (axis)
        {
            case Axis.X:
                rotateVector = new Vector3(1, 0, 0);
                rotateAxis = new Vector3(0, 0, 1);
                break;
            case Axis.Y:
                rotateVector = new Vector3(0, 1, 0);
                rotateAxis = new Vector3(1, 0, 0);
                break;
            case Axis.Z:
                rotateVector = new Vector3(0, 0, 1);
                rotateAxis = new Vector3(0, 1, 0);
                break;
            default:
                rotateVector = new Vector3(0, 1, 0);
                rotateAxis = new Vector3(1, 0, 0);
                break;
        }
    }
}
