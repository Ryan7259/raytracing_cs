using System.Numerics;

class TwoPerlinSpheres {
    public static List<Hittable> Two_Perlin_Spheres() {
        PerlinNoise pertext = new PerlinNoise(4);

        return new List<Hittable>() {
            new Sphere(new Vector3(0, -1000f, 0), 1000f, new Lambertian(pertext)),
            new Sphere(new Vector3(0, 2f, 0), 2f, new Lambertian(pertext))
        };
    }
}