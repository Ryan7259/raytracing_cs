using System.Numerics;

public class DiffuseLight : Material
{
    public Texture EmitTexture { get; }

    public DiffuseLight(Vector3 color)
    {
        EmitTexture = new SolidColor(color);
    }
    public DiffuseLight(Texture emitTexture)
    {
        EmitTexture = emitTexture;
    }

    public override bool Scatter(in Ray r_in, ref HitRecord rec, ref Vector3 attenuation, ref Ray scattered)
    {
        return false;
    }

    public override Vector3 Emit(float u, float v, in Vector3 p)
    {
        return EmitTexture.Value(u, v, p);
    }
}