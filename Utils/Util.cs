using System.Numerics;

public class Util 
{    
    public static float Rand_Float()
    {
        Random r = new Random();
        return (float)r.NextDouble();
    }
    public static float Rand_Float(float min, float max)
    {
        return (float)(min + (max-min)*Rand_Float());
    }

    public static int Rand_Int()
    {
        Random r = new Random();
        return r.Next();
    }

    public static int Rand_Int(int min, int max)
    {
        Random r = new Random();
        return r.Next(min, max);
    }

    public static float Deg_To_Rad(float deg)
    {
        return (float)(deg * (Math.PI/180.0));
    }

    // have to use register level operations for bits of a float/float
    // since adding parallelly is only built in for ints
    public static Vec3PL PlAdd(ref Vec3PL location1, Vector3 val)
    {
        Vec3PL curLoc = location1;

        while (true)
        {
            Vec3PL oldLoc = curLoc;
            Vec3PL newVal = oldLoc + val;
            curLoc = Interlocked.CompareExchange(ref location1, newVal, oldLoc);
            // if original loc1 val stays the same, then CAS occurred, and newVal is in loc
            if ( curLoc == oldLoc )
            {
                return newVal;
            }
        }
    }

    public static BVHPrimitiveInfo NthElement(ref List<BVHPrimitiveInfo> list, int start, int end, int n, Comparer<BVHPrimitiveInfo> comparer)
    {
        if ( start == end ) return list[start];

        // select end as pivot
        // partition all elems so that they are sorted like so [elems <= pivot < elems]
        int pivot = PartitionC(ref list, start, end, comparer);
        Console.Error.WriteLine("pivot: " + pivot);

        // pivot is now sorted, check if its the nth element
        if ( n == pivot )
        {
            return list[n];
        }
        // pivot > nth element, partition lower half
        else if ( n < pivot )
        {
            return NthElement(ref list, start, pivot-1, n, comparer);
        }
        // pivot < nth element, partition upper half
        else
        {
            return NthElement(ref list, pivot+1, end, n, comparer);
        }
    }

    /*
        used to partition a range of a primitiveInfo list into two havles
        and returns first idx where comparer evals to false
        
        ordering: [ evals true < returned idx <= evals false]
    */
    public static int PartitionC(ref List<BVHPrimitiveInfo> list, int start, int end, Comparer<BVHPrimitiveInfo> c)
    {   
        // select end as pivot, so stop testing right before pivot
        // build up < list on left, starting with i as end of new list, j to test elems
        // add to end of new list when j <= pivot
        int i = start;

        for ( int j = start; j < end; ++j )
        {   
            // swap when l elem < pivot elem
            if ( c.Compare(list[j], list[end]) <= 0 )
            {
                (list[i], list[j]) = (list[j], list[i]);
                ++i;
            }
        }
        // i should now be 1 off end of list, swap with pivot, so i is now sorted
        (list[i], list[end]) = (list[end], list[i]);
        return i;
    }

    public static int Partition(ref List<BVHPrimitiveInfo> list, int start, int end, Predicate<BVHPrimitiveInfo> p)
    {   
        // select end as pivot, so stop testing right before pivot
        // build up < list on left, starting with i as end of new list, j to test elems
        // add to end of new list when j <= pivot
        int i = start;

        /*  
            if using comparer instead of predicate
            we compare against a closure splitBucket val outside of func, not the end idx or pivot
            the end idx or pivot also needs to be tested against (we use end only for param reqs of comparer)
        */
        for ( int j = start; j <= end; ++j )
        {   
            // swap when l elem <= pivot elem
            if ( p(list[j]) )
            {
                (list[i], list[j]) = (list[j], list[i]);
                ++i;
            }
        }
        // i should now be 1 off end of list, swap with pivot, so i is now sorted
        (list[i], list[end]) = (list[end], list[i]);
        return i;
    }
}

// Interlocked.CompareExchange complains that Vector3 class must be a ref...
// use this helper class and convert back to Vector3
public class Vec3PL {
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vec3PL(Vector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }
    public Vec3PL(float a, float b, float c)
    {
        X = a;
        Y = b;
        Z = c;
    }

    public static Vec3PL operator+(Vec3PL a, Vector3 b)
    {
        return new Vec3PL(a.X+b.X, a.Y+b.Y, a.Z+b.Z);
    }
    public Vector3 ToVector3()
    {
        return new Vector3(X, Y, Z);
    }
}