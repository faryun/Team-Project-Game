﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Different types of movements for the platform
public enum MovementType {
    line,
    circular,
    zigzag
};

// Movement orientation for line movement type
public enum LineMovementOrientation {
    horizontal,
    vertical,
    verticalRandom
}

// Movement orientation for circular movement type
public enum CircularMovementOrientation {
    clockwise,
    counterclockwise
}

public class MovingPlatform : MonoBehaviour
{

    // General vars (affect all types of movemens)
    float randomSpeed;
    float randomDistance;

    [SerializeField]
    MovementType movementType;
    public float speed = 2f;
    Vector3 startPosition;
    public Color gizmoColor = Color.yellow;

    // Line movement vars
    public LineMovementOrientation lineMovementOrientation;
    public float lineDistance = 7f;

    // Circle movement vars
    public CircularMovementOrientation circularMovementOrientation;
    public float circleRadius = 5f;

    // Zigzag movement vars
    public int zigzagLines = 4;
    public float zigzagLineDistance = 2;
    float zigzagStep;
    bool zigzagMovingPositive = true;
    private Vector3 platformLastPos;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        randomSpeed = UnityEngine.Random.Range(1f, 1.5f);
        randomDistance = UnityEngine.Random.Range(8.5f, 12f);
        // Set start position
        startPosition = this.transform.position;
        zigzagStep = 0f;

        platformLastPos = transform.position; // 초기 위치 저장
    }

    // Update is called once per frame
    void Update()
    {

        // Manage how the platform have to move according to movementType var
        switch (movementType) {
            case MovementType.line:
                moveInAStraightLine();
            break;
            case MovementType.circular:
                MoveInCircles();
            break;
            case MovementType.zigzag:
                MoveInZigzag();
            break;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        player = collision.gameObject;
    }
}

void OnCollisionExit2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        player = null;
    }
}

void LateUpdate()
{
    if (player != null)
    {
        Vector3 deltaPosition = transform.position - platformLastPos; // 플랫폼의 이동량 계산
        player.transform.position += deltaPosition; // 캐릭터에게 이동량 적용
    }

    platformLastPos = transform.position; // 현재 위치 업데이트
}

    // Move the platform in a straight line in movementOrientation
    public void moveInAStraightLine() 
    {
        // Get current coordenates 
        float x = startPosition.x;
        float y = startPosition.y;
        float z = startPosition.z;

        // Calculating next position according to the orientation selected
        switch (lineMovementOrientation) {
            case LineMovementOrientation.horizontal:
                x = startPosition.x + Mathf.Sin(Time.time * speed) * lineDistance;
            break;
            case LineMovementOrientation.vertical:
                y = startPosition.y + Mathf.Sin(Time.time * speed) * lineDistance;
            break;
            case LineMovementOrientation.verticalRandom:
                y = startPosition.y + Mathf.Sin(-(Time.time * randomSpeed * 1.1f)) * randomDistance;
            break;
        }

        // Moving platform
        this.transform.position = new Vector3(x,y,z);
    }

    public void MoveInCircles() 
    {
        // Calculating direction (CW or CCW)
        int direction = (circularMovementOrientation == CircularMovementOrientation.counterclockwise)?1:-1;

        // Calculating coordenates 
        float x = startPosition.x + Mathf.Cos(Time.time * speed * direction) * circleRadius;
        x -= circleRadius;
        float y = startPosition.y + Mathf.Sin(Time.time * speed * direction) * circleRadius;
        float z = transform.position.z;

        // Moving Platform
        this.transform.position = new Vector3(x,y,z);

    }


    public void MoveInZigzag() 
    {   
        // Changing direction when the platform reach the limits 
        if (transform.position.x >= startPosition.x + zigzagLineDistance * zigzagLines) {
            zigzagMovingPositive = false;
        } else if (transform.position.x <= startPosition.x) {
            zigzagMovingPositive = true;
        }

        // Calculating coordenates
        float factor = (Mathf.Acos(Mathf.Cos(zigzagStep * (float)Math.PI)) / (float)Math.PI);
        float x = startPosition.x + zigzagStep * zigzagLineDistance;
        float y = startPosition.y + factor * zigzagLineDistance;
        
        if (zigzagMovingPositive) {
            zigzagStep += speed/50;
        } else {
            zigzagStep -= speed/50;
        }

        // keep platform within the limits
        zigzagStep = Mathf.Clamp(zigzagStep, 0, zigzagLines);


        // Moving platform 
        this.transform.position = new Vector3(x,y);
    }

    // Funtion to see the platform path (only for debugging)
    private void OnDrawGizmosSelected() {
        Gizmos.color = gizmoColor;
        Vector3 src = Vector3.zero;
        Vector3 dest = Vector3.zero;

        switch (movementType) {
            case MovementType.line:
                src = new Vector3 (startPosition.x - lineDistance, startPosition.y);
                dest = new Vector3 (startPosition.x + lineDistance, startPosition.y); 
                Gizmos.DrawLine(src, dest);
                src = new Vector3 (startPosition.x, startPosition.y - lineDistance);
                dest = new Vector3 (startPosition.x, startPosition.y + lineDistance); 
                Gizmos.DrawLine(src, dest);
            break;
            case MovementType.circular:
                // Cicular movement 
                src = new Vector3(startPosition.x - circleRadius, startPosition.y);
                Gizmos.DrawWireSphere(src, circleRadius);
            break;
            case MovementType.zigzag:
                float x = startPosition.x;
                float y = startPosition.y;

                for(int i = 0; i < zigzagLines; i++){
                    
                    // Current position
                    src = new Vector3(x, y);
                    
                    // Calculating next position
                    x += zigzagLineDistance;

                    // If "i" is even draw line going up, else, draw line going down
                    // the zigzag movement always start going up
                    y = (i%2 == 0)?startPosition.y + zigzagLineDistance:startPosition.y;

                    dest = new Vector3(x,y);

                    // Drawing line
                    Gizmos.DrawLine(src, dest);

                }
            break;
        }
    }

}