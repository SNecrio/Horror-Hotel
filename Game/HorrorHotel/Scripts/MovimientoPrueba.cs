using Godot;
using System;

public partial class MovimientoPrueba : CharacterBody3D
{
    [Export] public float MovementSpeed = 40.0f;
    [Export] public float RotationSpeed = 3.0f;
    [Export] public Camera3D camara;
    
    public int velZV = 0;
    public int velZH = 0;
    private Vector3 vel;
    private Vector3 rot;
    private Vector3 camRot;

    public override void _PhysicsProcess(double delta)
    {
        //Setup de Variables
        vel = Velocity;
        rot = Rotation;
        camRot = camara.Rotation;
        
        //Input de movimiento
        velZV = 0;
        velZH = 0;
        if (Input.IsKeyPressed(Key.W))
            velZV += 1;
        if (Input.IsKeyPressed(Key.S))
            velZV -= 1;
        if (Input.IsKeyPressed(Key.A))
            velZH += 1; 
        if (Input.IsKeyPressed(Key.D))
            velZH -= 1;
        
        //Input de rotacion
        if(Input.IsKeyPressed(Key.Right))
            rot.Y = (rot.Y - RotationSpeed * (float)delta) % 360;
        if(Input.IsKeyPressed(Key.Left))
            rot.Y = (rot.Y + RotationSpeed * (float)delta) % 360;
        if(Input.IsKeyPressed(Key.Up) && camRot.X < 1.5f)
            camRot.X += RotationSpeed * (float)delta;
        if(Input.IsKeyPressed(Key.Down) && camRot.X > -1)
            camRot.X -= RotationSpeed * (float)delta;
        
        //Calculo de movimiento dependiendo de la rotacion
        float forwardX = (float)Math.Sin(rot.Y);
        float forwardZ = (float)Math.Cos(rot.Y);
        float rightX = (float)Math.Cos(rot.Y);
        float rightZ = -(float)Math.Sin(rot.Y);
        vel.X = (forwardX * velZV + rightX * velZH) * MovementSpeed;
        vel.Z = (forwardZ * velZV + rightZ * velZH) * MovementSpeed;
        
        Velocity = vel;
        Rotation = rot;
        camara.Rotation = camRot;
        MoveAndSlide();
    }
}