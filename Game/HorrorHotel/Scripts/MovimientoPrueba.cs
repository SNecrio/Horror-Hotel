using Godot;
using System;

public partial class MovimientoPrueba : CharacterBody3D
{
    [Export] public float MovementSpeed = 40.0f;
    [Export] public float RotationSpeed = 3.0f;
    [Export] public float MouseSensitivity = .005f;
    [Export] public Camera3D camara;
    [Export] public RayCast3D raycast;
    
    private int velZV;
    private int velZH;
    private Vector3 vel;
    private Vector3 rot;
    private bool spacePressed;
    
    private bool lookingAtObject;
    private GodotObject lookObj;
    private StaticBody3D lookHoldObj;
    private Vector3 lookObjScale;

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
        
        //Input de salto
        if (Input.IsKeyPressed(Key.Space) && IsOnFloor())
            vel.Y += 80f;
        else
            vel.Y -= 5f;
        
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
        
        //Raycast objetos coger
        if (raycast.IsColliding())
        {
            //Obtiene el objeto y comprueba que no sea null, porsacaso
            lookObj = raycast.GetCollider();
            if (lookObj != null)
            {
                //Si es sujetable, se pasa a ser un staticbody (por ahora, igual lo hago su propia clase) y se cambia la escala para probar
                bool holdable = (bool)lookObj.GetMeta("Hold", false);
                if (holdable)
                {
                    if (!lookingAtObject)
                    {
                        lookHoldObj = (StaticBody3D)lookObj;
                        lookObjScale = lookHoldObj.GetScale();
                        lookHoldObj.SetScale(new Vector3(1,1,1));
                        lookingAtObject = true;
                    }
                }
            }
        }
        else if(lookingAtObject)
        {
            lookHoldObj.SetScale(lookObjScale);
            lookingAtObject = false;
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