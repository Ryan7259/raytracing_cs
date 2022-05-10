using System.Numerics;

public class HitRecord {
    public Vector3 p { get; set; }
    public Vector3 normal { get; set; }
    public float t { get; set; }
    public bool front_face { get; set; } // if ray hit object from outside
    public Material? mat { get; set; }

    public HitRecord()
    {
        p = new Vector3(0.0f);
        normal = new Vector3(0.0f);
        t = 0.0f;
        front_face = false;
        mat = null;
    }

    // always have normal against the ray if it intersects, so we need to track if 
    // normal is pointing inside or outside surface
    public void set_face_normal(in Ray r, in Vector3 outward_normal)
    {
        // ray is inside object since its intersection point normal is going with it
        // or the dot is positive
        
        // ray is outside since its intersection point is against the normal
        // or the dot is negative
        front_face = Vector3.Dot(outward_normal, r.Dir) < 0.0f;
        normal = front_face ? outward_normal : -outward_normal;
    }
}