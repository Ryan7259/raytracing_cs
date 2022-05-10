public abstract class Hittable {
    
    /*
        find out if there exists 1-2 point(s) of intersection
        for a ray pointing at a sphere

        solving for those two points tells us if there exists solutions or points of intersection
        in quadratic form, we look at determinant > 0 => 2 pts, < 0 => none, == 0 => 1 pt
        (< 0 could mean behind ray origin aswell)

        using geometry could also give us solutions, 
        but we have to stop early if angles or distances
        are not within sphere
    */
    public abstract bool hit(in Ray r, float t_min, float t_max, ref HitRecord rec);

    /*
        create a bounding box for AABB
        returns a bool for cases where AABB is not possible like infinite planes
    */
    public abstract AABB WorldBound(float time0 = 0.0f, float time1 = 0.0f);
}