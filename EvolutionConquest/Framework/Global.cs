using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Global
{
    public enum Anchor
    {
        TopCenter,
        BottomCenter,
        LeftCenter,
        RightCenter,
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight
    };

    public const int CLIMATE_HEIGHT_PERCENT = 20;
    public static readonly Camera Camera = new Camera();
    //https://stackoverflow.com/questions/1011732/iterating-through-the-alphabet-c-sharp-a-caz
    public static string GetNextBase26(string a)
    {
        return Base26Sequence().SkipWhile(x => x != a).Skip(1).First();
    }

    private static IEnumerable<string> Base26Sequence()
    {
        long i = 0L;
        while (true)
            yield return Base26Encode(i++);
    }

    private static char[] base26Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static string Base26Encode(Int64 value)
    {
        string returnValue = null;
        do
        {
            returnValue = base26Chars[value % 26] + returnValue;
            value /= 26;
        } while (value-- != 0);
        return returnValue;
    }
}