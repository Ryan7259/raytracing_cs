
using System.Numerics;
using System.Runtime.Versioning;

namespace raytracer {
    [SupportedOSPlatform("windows")]
    class Program 
    {
        static void Write_Color(Vector3 pixel_color, int samples_per_pixel)
        {
            float r = pixel_color.X;
            float g = pixel_color.Y;
            float b = pixel_color.Z;

            // we took many samples so rgb is large so average it out by samples per pixel
            float scale = 1.0f / samples_per_pixel; 

            // Photo viewers modify gamma for photos, so we have to undo this gamma factor that is applied by these apps
            // gamma default is usually power 2 which means we need to sqrt Vector3 values
            r = (float)Math.Pow(r*scale, 1/2.2);
            g = (float)Math.Pow(g*scale, 1/2.2);
            b = (float)Math.Pow(b*scale, 1/2.2);

            Console.WriteLine($"{(int)(255f*Math.Clamp(r, 0f, 1f))} {(int)(255f*Math.Clamp(g, 0f, 1f))} {(int)(255f*Math.Clamp(b, 0f, 1f))}");
        }
        static Vector3 Trace(in Ray r, in BVHNode root, int depth, in Vector3 background)
        {   
            // bounce stuck between objects infinitely? never gonna reach eye, so return black
            if ( depth < 0 ) return new Vector3();
            
            // trace ray r across these hittable objects
            // returns true if ray hit atleast 1 object along with nearest hit info so we only render objects in front
            HitRecord rec = new HitRecord();
            if ( !root.Hit(r, 0.001f, float.PositiveInfinity, ref rec) ) return background;

            Vector3 attenuation = new Vector3();
            Ray scatter_ray = new Ray(new Vector3(), new Vector3(), 0);
            Vector3 emitted = rec.Mat.Emit(rec.U, rec.V, rec.P);

            // from hit point and material type, gather scatter dir and attenuation of material
            // false result usually means its a light source
            if ( !rec.Mat.Scatter(in r, ref rec, ref attenuation, ref scatter_ray) ) return emitted;

            // attenuate/absorb color and recursively bounce scattered ray into world again 
            // (limit this single ray scattering by depth times to prevent infinite bounces)
            return attenuation * Trace(scatter_ray, root, depth-1, background);

            /* 
            // ray didn't intersect anything, so return background/sky value (sky is and blue/white blend depending on y val)
            // which could be part of a long stack of recursive bounces that will attenuate this Vector3
            float t = (r.Dir.Y + 1.0f) * 0.5f; // min-max normalize ray's y dir from [-1,1] to [0,1] just for blending Vector3s
            return (1.0f-t)*new Vector3(1.0f) + t*new Vector3(0.5f, 0.7f, 1.0f);
            */
        }

        static void Main(string[] args)
        {
            // Camera
            float aspect_ratio = 3.0f / 2.0f;
            float vfov_deg = 20.0f; // can't calc proper height if atan is btwn: (-90 <= fov/2.0 <= 90)
            Vector3 lookfrom = new Vector3(13.0f, 2.0f, 3.0f);
            Vector3 lookat = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 world_up = new Vector3(0.0f, 1.0f, 0.0f);
            float aperture = 0f;
            float focus_dist = 10.0f;

            // Image
            int image_width = 400;
            int samples_per_pixel = 20;
            int max_depth = 50;
            Vector3 background = new Vector3();

            // World
            List<Hittable> world = new List<Hittable>();
            int scene = 6;
            switch (scene) 
            {
                case 1:
                    world = RandomScene.Random_Scene();
                    background = new Vector3(0.7f, 0.8f, 1f);
                    break;
                case 2:
                    world = TwoPerlinSpheres.Two_Perlin_Spheres();
                    background = new Vector3(0.7f, 0.8f, 1f);
                    break;
                case 3:
                    world = Earth.Earth_Scene();
                    background = new Vector3(0.7f, 0.8f, 1f);
                    break;
                case 4:
                    world = SimpleLight.Simple_Light_Scene();
                    lookfrom = new Vector3(26f,3f,6f);
                    lookat = new Vector3(0f, 2f, 0f);
                    samples_per_pixel = 400;
                    break;
                case 5:
                    world = CornellBox.Cornell_Box_Scene();
                    image_width = 600;
                    samples_per_pixel = 400;
                    lookfrom = new Vector3(278f, 278f, -800f);
                    lookat = new Vector3(278f, 278f, 0f);
                    vfov_deg = 40.0f;
                    break;
                case 6:
                    world = Assorted.Assorted_Scene();
                    image_width = 800;
                    aspect_ratio = 1f;
                    samples_per_pixel = 1000;
                    lookfrom = new Vector3(478f, 278f, -600f);
                    lookat = new Vector3(278f, 278f, 0f);
                    vfov_deg = 40.0f;
                    break;
                default:
                    world = TwoSpheres.Two_Spheres();
                    background = new Vector3(0.7f, 0.8f, 1f);
                    lookfrom = new Vector3(13f,2f,3f);
                    lookat = new Vector3();
                    aperture = 0f;
                    break;
            }

            int image_height = (int) ((float)image_width / aspect_ratio);
            Camera c = new Camera(
            lookfrom, lookat, world_up, 
            vfov_deg, aspect_ratio, aperture, focus_dist,
            0.0f, 1.0f);
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // build BVH with binned surface area heuristic for world
            BVH bvh = new BVH(world, 0, world.Count-1);

            // Render
            Console.Write($"P3\n{image_width} {image_height}\n255\n");
            for ( int i = image_height-1; i >= 0; --i )
            {
                Console.Error.WriteLine($"Scanlines remaining: {i}");
                for ( int j = 0; j < image_width; ++j )
                {
                    Vec3PL final_pixel_color = new Vec3PL(new Vector3());
                    Parallel.For(0, samples_per_pixel, () => new Vector3(), (s, state, subtotal) => 
                    {
                        float r1 = Util.Rand_Float();
                        float r2 = Util.Rand_Float();
                        float u = (float) (j + r1) / (float) (image_width-1);
                        float v = (float) (i + r2) / (float) (image_height-1);

                        Ray r = c.Get_Ray(u, v); // get ray at this dir
                        // trace ray across hittable objects, scattering, building Vector3 vals
                        subtotal += Trace(r, bvh.Root, max_depth, background); 
                        return subtotal;
                    },
                        (sample_color) => {
                            Util.PlAdd(ref final_pixel_color, sample_color);
                        }
                    );
                    // average out final Vector3 value from ray hits/bounces
                    Write_Color(final_pixel_color.ToVector3(), samples_per_pixel); 
                }
            }
            watch.Stop();
            Console.Error.Write("\nDone in " + watch.ElapsedMilliseconds + "ms\n");
        }
    }
}