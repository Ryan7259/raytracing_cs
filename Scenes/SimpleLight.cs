using System.Numerics;

class SimpleLight {
    public static List<Hittable> Simple_Light_Scene()
    {
        PerlinNoise pertext = new PerlinNoise(4f);
        DiffuseLight dlight = new DiffuseLight(new Vector3(4f));

        return new List<Hittable>() {
            new XYRect(3f, 5f, 1f, 3f, -2f, dlight),
            new Sphere(new Vector3(0, 7f, 0), 2f, dlight),
            new Sphere(new Vector3(0, -1000f, 0), 1000f, new Lambertian(pertext)),
            new Sphere(new Vector3(0, 2f, 0), 2f, new Lambertian(pertext))
        };
    }
}