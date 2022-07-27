using System.Numerics;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
class Earth {
     public static List<Hittable> Earth_Scene() {
        ImageTexture earth_texture = new ImageTexture("earthmap.jpg");
        Lambertian earth_surface = new Lambertian(earth_texture);
        return new List<Hittable>() {
            new Sphere(new Vector3(), 2f, earth_surface)
        };
    }
}

