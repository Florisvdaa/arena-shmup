using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker_PA : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private Walker_PA otherFoot;
    [SerializeField] private float stepDistance;
    [SerializeField] private float stepHeight;
    [SerializeField] private float stepLength;
    [SerializeField] private float footSpacing;
    [SerializeField] private float speed;
    [SerializeField] private Transform body;
    [SerializeField] private Vector3 footOffset;

    // privates
    private Vector3 oldPos;
    private Vector3 newPos;
    private Vector3 currentPos;

    private Vector3 oldNormal;
    private Vector3 newNormal;
    private Vector3 currentNormal;
    private float lerp;

    private void Start()
    {
        // setting inital values
        footSpacing = transform.localPosition.x;
        oldPos = newPos = currentPos = transform.position;
        oldNormal = currentNormal = newNormal = transform.up;
        lerp = 1;
    }

    private void Update()
    {
        // Updating position and normal
        transform.position = currentPos;
        transform.up = currentNormal;

        Ray ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 10, terrainLayer.value))
        {
            if (Vector3.Distance(newPos, hit.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPos).z ? 1 : -1;
                newPos = hit.point + (body.forward * stepLength * direction) + footOffset;
                newNormal = hit.normal;
            }
        }
        if(lerp < 1)
        {
            Vector3 tempPos = Vector3.Lerp(oldPos, newPos, lerp);
            tempPos.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;
            currentPos = tempPos;
            currentNormal = Vector3.Lerp(oldNormal, newPos, lerp);
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPos = newPos;
            oldNormal = newNormal;
        }
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }
}
