
using System.Numerics;

namespace raytracer {
    class Program 
    {
        static void Write_Color(Vector3 pixel_color, int samples_per_pixel)
        {
            float r = pixel_color.X;
            float g = pixel_color.Y;
            float b = pixel_color.Z;

            float scale = 1.0f / samples_per_pixel; // we took many samples so rgb is large so average it out by samples per pixel

            // Photo viewers modify gamma for photos, so we have to undo this gamma factor that is applied by these apps
            // gamma default is usually power 2 which means we need to sqrt Vector3 values
            r = (float)Math.Sqrt(r*scale);
            g = (float)Math.Sqrt(g*scale);
            b = (float)Math.Sqrt(b*scale);

            Console.WriteLine($"{(int)256*Math.Clamp(r, 0f, 1f)} {(int)256*Math.Clamp(g, 0f, 1f)} {(int)256*Math.Clamp(b, 0f, 1f)}");
        }
        static Vector3 Trace(in Ray r, in BVHNode root, int depth) //in HittableList world, int depth)
        {
            if ( depth < 0 ) 
            {
                return new Vector3(0.0f); // bounce stuck between objects infinitely? never gonna reach eye, so return black
            }

            HitRecord rec = new HitRecord();

            // trace ray r across these hittable objects
            // returns true if ray hit atleast 1 object along with nearest hit info so we only render objects in front
            if ( root.hit(r, 0.001f, float.PositiveInfinity, ref rec) )
            {
                Vector3 attenuation; // used from hit object's material to attenuate Vector3 values
                Ray scattered;

                // from hit point and material type, gather scatter dir and attenuation of material
                // scattering calculation could give bad angles that aren't valid, so don't scatter them
                if ( rec.mat != null && rec.mat.scatter(in r, ref rec, out attenuation, out scattered) )
                {
                    // general case for metals, lambertian mats
                    // attenuate Vector3 and recursively bounce scattered ray into world again (limit this single ray scattering by depth times to prevent infinite bounces)
                    return attenuation * Trace(scattered, root, depth-1);
                }

                // if scatter isn't valid, return black
                return new Vector3(0.0f);
            }
            // ray didn't intersect anything, so return background/sky value (sky is and blue/white blend depending on y val)
            // which could be part of a long stack of recursive bounces that will attenuate this Vector3
            float t = (r.Dir.Y + 1.0f) * 0.5f; // min-max normalize ray's y dir from [-1,1] to [0,1] just for blending Vector3s
            return (1.0f-t)*new Vector3(1.0f) + t*new Vector3(0.5f, 0.7f, 1.0f);
        }

        static void Main(string[] args)
        {
            TextWriter errorWriter = Console.Error;

            // Camera
            float aspect_ratio = 3.0f / 2.0f;
            float vfov_deg = 20.0f; // can't calc proper height if atan is btwn: (-90 <= fov/2.0 <= 90)
            Vector3 lookfrom = new Vector3(13.0f, 2.0f, 3.0f);
            Vector3 lookat = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 world_up = new Vector3(0.0f, 1.0f, 0.0f);
            float aperture = 0.1f;
            float focus_dist = 10.0f;
            Camera c = new Camera(
                lookfrom, lookat, world_up, 
                vfov_deg, aspect_ratio, aperture, focus_dist,
                0.0f, 1.0f);

            // Image
            int image_width = 400;
            int image_height = (int) ((float)image_width / c.AspectRatio);
            int samples_per_pixel = 50;
            int max_depth = 50;

            // World
            List<Hittable> world = RandomScene.Random_Scene();
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            // build BVH with binned surface area heuristic for world
            BVH bvh = new BVH(world, 0, world.Count-1);

            // Render
            Console.Write($"P3\n{image_width} {image_height}\n255\n");
            for ( int i = image_height-1; i >= 0; --i )
            {
                errorWriter.WriteLine($"Scanlines remaining: {i}");
                for ( int j = 0; j < image_width; ++j )
                {
                    Vec3PL final_pixel_color = new Vec3PL(new Vector3(0.0f));
                    Parallel.For(0, samples_per_pixel, () => new Vector3(0.0f), (s, state, subtotal) => 
                    {
                        float r1 = Util.Rand_Float();
                        float r2 = Util.Rand_Float();
                        float u = (float) (j + r1) / (float) (image_width-1);
                        float v = (float) (i + r2) / (float) (image_height-1);

                        Ray r = c.Get_Ray(u, v); // get ray at this dir
                        // trace ray across hittable objects, scattering, building Vector3 vals
                        subtotal += Trace(r, bvh.Root, max_depth); 
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
            var elapsedMs = watch.ElapsedMilliseconds;
            errorWriter.Write("\nDone in " + elapsedMs + "ms\n");
            
        }
    }
}