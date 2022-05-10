using System.Numerics;

public abstract class Material {
    public abstract bool scatter(in Ray r_in, ref HitRecord rec, out Vector3 attenuation, out Ray scattered);
}