using System.Numerics;

public class SolidColor : Texture {
    public Vector3 Color { get; set; }

    public SolidColor()
    {
        Color = new Vector3();
    }

    public SolidColor(Vector3 c)
    {
        Color = c;
    }
    public SolidColor(float r, float g, float b)
    {
        Color = new Vector3(r, g, b);
    }


    public override Vector3 Value(float u, float v, Vector3 p)
    {
        return Color;
    }
}