using System.Numerics;

public class Ray {
    public Vector3 Origin { get; }
    public Vector3 Dir { get; }
    public float Time { get; }
    public Vector3 InvDirs { get; }

    public Ray(Vector3 origin, Vector3 dir, float time)
    {
        Origin = origin;
        Dir = Vector3.Normalize(dir); // not normalizing immediately led to weird outline at surface
        Time = time;
        InvDirs = new Vector3((float)(1.0/dir.X), (float)(1.0/dir.Y), (float)(1.0/dir.Z));
    }

    public Vector3 at(float t)
    {
        return Origin + t*Dir;
    }
}