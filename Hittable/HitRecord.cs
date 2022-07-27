using System.Numerics;

public class HitRecord {
    public Vector3 P { get; set; }
    public Vector3 Normal { get; set; }
    public float T { get; set; }
    public bool FrontFace { get; set; } // if ray hit object from outside
    public Material Mat { get; set; }
    public float U { get; set; }
    public float V { get; set; }

    public HitRecord()
    {
        Mat = new Lambertian(new Vector3());
    }

    // always have normal against the ray if it intersects, so we need to track if 
    // normal is pointing inside or outside surface
    public void SetFaceNormal(in Ray r, in Vector3 outwardNormal)
    {
        // ray is inside object since its intersection point normal is going with it
        // or the dot is positive
        
        // ray is outside since its intersection point is against the normal
        // or the dot is negative
        FrontFace = Vector3.Dot(outwardNormal, r.Dir) < 0.0f;
        Normal = FrontFace ? outwardNormal : -outwardNormal;
    }
}