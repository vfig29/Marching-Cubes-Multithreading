using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterController : MonoBehaviour
{
    public int xAxis { get; set; }
    public int zAxis { get; set; }

    protected KeyCode up = KeyCode.W, down = KeyCode.S, left = KeyCode.A, right = KeyCode.D;
    protected void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        VerticalDirectional();
        HorizontalDirectional();
    }

    protected void FixedUpdate()
    {
        
    }

    public abstract void VerticalDirectional();



    public abstract void HorizontalDirectional();
    
}
