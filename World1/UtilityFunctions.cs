using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

public static class UtilityFunctions
{
    public static float Distance2D(LocationDataFloat loc1, LocationDataFloat loc2) => (float)Math.Sqrt(Math.Pow((loc2.X - loc1.X), 2f) + Math.Pow((loc2.Y - loc1.Y), 2f));
    public static float Distance2D(LocationDataInt loc1, LocationDataInt loc2) => (float)Math.Sqrt(Math.Pow(((float)loc2.X - (float)loc1.X), 2f) + Math.Pow(((float)loc2.Y - (float)loc1.Y), 2f));
    public static float Distance2D(LocationDataFloat loc1, LocationDataInt loc2) => (float)Math.Sqrt(Math.Pow(((float)loc2.X - loc1.X), 2f) + Math.Pow(((float)loc2.Y - loc1.Y), 2f));
    public static float Distance2D(LocationDataInt loc1, LocationDataFloat loc2) => (float)Math.Sqrt(Math.Pow((loc2.X - (float)loc1.X), 2f) + Math.Pow((loc2.Y - (float)loc1.Y), 2f));
    public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> => (val.CompareTo(min) < 0) ? min : (val.CompareTo(max) > 0) ? max : val;

    public static bool IsEmptySpace(int excludeentityID, LocationDataFloat location)
    {
        foreach (int x in Program.slashGame.EntityManager.EntitiesWithComponent(typeof(FormComponent)))
        {
            if (x != excludeentityID) // && !EntityIsTerrainEmpty(x))
            {
                var form = Program.slashGame.EntityManager.GetComponent<FormComponent>(x);

                if ((int)form.Location.X == (int)location.X && (int)form.Location.Y == (int)location.Y && !form.IsPassable)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static int Random(int min, int max)
    {
        Random rnd = new Random();
        return rnd.Next(min, max + 1);
    }

    public static int Random(int seed, int min, int max)
    {
        Random rnd = new Random(seed);
        return rnd.Next(min, max + 1);
    }
}

public class LocationDataInt
{
    public int X { get; set; }
    public int Y { get; set; }

    public LocationDataInt Clone() => new LocationDataInt(X, Y);
    public Point GetAsPoint => new Point(X, Y);

    public LocationDataInt(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(LocationDataInt obj1, LocationDataInt obj2) => (obj1.X == obj2.X && obj1.Y == obj2.Y);
    public static bool operator !=(LocationDataInt obj1, LocationDataInt obj2) => !(obj1.X == obj2.X && obj1.Y == obj2.Y);
    public override bool Equals(object obj) => (X == ((LocationDataInt)obj).X && Y == ((LocationDataInt)obj).Y);
    public override int GetHashCode()
    {
        // I have no idea what this does
        unchecked
        {
            int hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Y.GetHashCode();
            return hashCode;
        }
    }
}

public class LocationDataFloat
{
    public float X { get; set; }
    public float Y { get; set; }

    public LocationDataFloat Clone() => new LocationDataFloat(X, Y);
    public LocationDataInt GetAsInt => new LocationDataInt((int)Math.Round(X, 0), (int)Math.Round(Y, 0));
    public Point GetAsPoint => new Point((int)Math.Round(X, 0), (int)Math.Round(Y, 0));

    public LocationDataFloat(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(LocationDataFloat obj1, LocationDataFloat obj2) => (obj1.X == obj2.X && obj1.Y == obj2.Y);
    public static bool operator !=(LocationDataFloat obj1, LocationDataFloat obj2) => !(obj1.X == obj2.X && obj1.Y == obj2.Y);
    public override bool Equals(object obj) => (X == ((LocationDataFloat)obj).X && Y == ((LocationDataFloat)obj).Y);
    public override int GetHashCode()
    {
        // I have no idea what this does
        unchecked
        {
            int hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Y.GetHashCode();
            return hashCode;
        }
    }
}
