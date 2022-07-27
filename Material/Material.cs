using System.Numerics;

public abstract class Material {

    public virtual Vector3 Emit(float u, float v, in Vector3 p)
    {
        return new Vector3();
    }
    public abstract bool Scatter(in Ray r_in, ref HitRecord rec, ref Vector3 attenuation, ref Ray scattered);
}