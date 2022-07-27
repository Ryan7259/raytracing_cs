using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
public class ImageTexture : Texture
{
    public int BytesPerPixel { get; }
    public int ByesPerScanline { get; }
    public byte[]? Buffer { get; }
    public int Width { get; }
    public int Height { get; }

    public ImageTexture(string filename)
    {
        /*
            to use parallelism, can't use Bitmap alone but also don't want to create new arr w/ LockBits each time, 
            so use a byte array, but only do concurrent reads

            transfer image data into a 1D byte array of BGR values (each idx is a byte for a BGR value)
            3*PixelWidth for total pixels per scanline (BitmapData.Stride() returns same val)
        */
        try 
        {
            Bitmap Bmp = (Bitmap)Bitmap.FromFile(filename);
            Width = Bmp.Width;
            Height = Bmp.Height;
            BytesPerPixel = Bitmap.GetPixelFormatSize(Bmp.PixelFormat) / 8;
            BitmapData bmpData = Bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, Bmp.PixelFormat);
            ByesPerScanline = bmpData.Stride;
            Buffer = new byte[Height*ByesPerScanline];
            Marshal.Copy(bmpData.Scan0, Buffer, 0, Buffer.Length);
            Bmp.UnlockBits(bmpData);
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine($"ERROR: Failed to load image texture file: '{filename}'");
        }
    }

    public override Vector3 Value(float u, float v, Vector3 p)
    {
        if ( Buffer == null )
        {
            return new Vector3(0f,1f,1f);
        }

        u = 1-Math.Clamp(u, 0, 1);
        v = Math.Clamp(v, 0, 1);

        int i = (int)(u * Width);
        int j = (int)(v * Height);

        if ( i >= Width ) i = Width-1;
        if ( j >= Height ) j = Height-1;
        
        int offset = BytesPerPixel*(Width*j + i);
        float colorScale = 1.0f / 255.0f;

        // copied into buffer in bgr, match proper indices to rgb
        return new Vector3(
            Buffer[offset+2]*colorScale, 
            Buffer[offset+1]*colorScale, 
            Buffer[offset]*colorScale
        );
    }
}
 