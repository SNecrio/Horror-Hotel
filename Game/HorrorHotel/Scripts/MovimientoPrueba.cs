using Godot;
using System;

public partial class MovimientoPrueba : CharacterBody3D
{
    [Export] public float MovementSpeed = 40.0f;
    [Export] public float RotationSpeed = 3.0f;
    [Export] public float MouseSensitivity = .05f;
    [Export] public Camera3D camara;
    [Export] public RayCast3D raycast;
    
    private float velZV;
    private float velZH;
    private Vector3 vel;
    private Vector3 rot;
    private bool spacePressed;
    
    private Node3D objLooked;
    private bool lookingAtHoldObj;
    private Node3D objHold;
    private bool holdingObj;
    private bool _debugMouseHidden = true;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    
    public override void _PhysicsProcess(double delta)
    {
        //Setup de Variables
        vel = Velocity;
        rot = Rotation;
        
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
        
        //JoyStick
        velZH = -Input.GetJoyAxis(0, JoyAxis.LeftX);
        velZV = -Input.GetJoyAxis(0, JoyAxis.LeftY);
        
        //Input de salto
        if (Input.IsKeyPressed(Key.Space) && IsOnFloor())
            vel.Y += 80f;
        else
            vel.Y -= 5f;
        
        //Rotacion stick
        float velYH = Input.GetJoyAxis(0, JoyAxis.RightX);
        float velYV = Input.GetJoyAxis(0, JoyAxis.RightY);
        
        
        //Calculo de movimiento dependiendo de la rotacion
        float forwardX = (float)Math.Sin(rot.Y);
        float forwardZ = (float)Math.Cos(rot.Y);
        float rightX = (float)Math.Cos(rot.Y);
        float rightZ = -(float)Math.Sin(rot.Y);
        vel.X = (forwardX * velZV + rightX * velZH) * MovementSpeed;
        vel.Z = (forwardZ * velZV + rightZ * velZH) * MovementSpeed;
        
        Velocity = vel;
        Rotation = rot;
        MoveAndSlide();
        
        //Input de la camara
        RotateY(-velYH * MouseSensitivity);
        //Rotación vertical (pitch) de la cámara
        float rotationX = camara.Rotation.X - velYV * MouseSensitivity;
        //Limitar el angulo vertical (-80° a 80°)
        rotationX = Mathf.Clamp(rotationX, Mathf.DegToRad(-80), Mathf.DegToRad(85));
        //Aplicar
        camara.Rotation = new Vector3(rotationX, camara.Rotation.Y, camara.Rotation.Z);
        
        //Raycast objetos coger
        if (raycast.IsColliding())
        {
            objLooked = (Node3D)raycast.GetCollider();
            
            //Si es sujetable, se pasa a ser un staticbody (por ahora, igual lo hago su propia clase) y se cambia la escala para probar

            if (objLooked != null)
            {
                bool holdable = (bool)objLooked.GetMeta("Hold", false);
                if (holdable)
                {
                    lookingAtHoldObj = true;
                    GD.Print("Looking at holdable object");
                    //Coger objeto
                    if (Input.IsActionJustPressed("grab"))
                    {
                        //Borramos el objeto del mundo
                        objLooked.GetParent().RemoveChild(objLooked);
                        objHold = objLooked;
                        holdingObj = true;
                    }
                }
            }
        }
        else if(lookingAtHoldObj)
        {
            lookingAtHoldObj = false;
        }
        
        //Si no estas mirando a ningun objeto agarrable y si estas agarrando uno, lo puedes poner
        if(holdingObj && !lookingAtHoldObj)
        {
            if (Input.IsActionJustPressed("grab"))
            {
                GetTree().CurrentScene.AddChild(objHold);
                objHold.GlobalPosition = camara.GlobalPosition + camara.GlobalTransform.Basis.Z * -5f;
                holdingObj = false;
                
                objHold = null;
            }
        }
        
        
        //Debug
        if (Input.IsActionJustPressed("_debugCaptureMouse"))
        {
            if (_debugMouseHidden)
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Captured;
            }

            _debugMouseHidden = !_debugMouseHidden;
        }
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            //Rotación horizontal (yaw) del cuerpo
            RotateY(-motion.Relative.X * MouseSensitivity);

            //Rotación vertical (pitch) de la cámara
            float rotationX = camara.Rotation.X - motion.Relative.Y * MouseSensitivity;

            //Limitar el angulo vertical (-80° a 80°)
            rotationX = Mathf.Clamp(rotationX, Mathf.DegToRad(-80), Mathf.DegToRad(85));

            camara.Rotation = new Vector3(rotationX, camara.Rotation.Y, camara.Rotation.Z);
        }
    }
}