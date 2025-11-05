namespace Neo.Bpms.Util.Expressions.FunctionImplementations;

public partial class BuiltInFunctions : IFunctionImplementations
{
    /// <summary>
    /// قدرمطلق عدد ورودی
    /// </summary>
    /// <param name="d">عدد ورودی</param>
    /// <returns></returns>
    public static double abs(double d)
    {
        return (d < 0) ? -d : d;
    }

    public static double round(double d, int type, int resolution, int resolutionType)
    {
        double scale = 1;
        if (resolutionType == 0) //دقت تعداد رقم اعشار را تعیین می کند
        {
            for (int i = 0; i < resolution; i++)
            {
                scale *= 10;
            }
        }
        else
        {
            //دقت تعداد صفر را تعیین می کند
            for (int i = 0; i < resolution; i++)
            {
                scale /= 10;
            }
        }

        return type switch
        {
            //up
            2 => Math.Ceiling(d * scale) / scale,
            //down
            3 => Math.Floor(d * scale) / scale,
            //case 1: //round
            _ => Math.Round(d * scale) / scale,
        };
    }

    public static double round2(double d, int type)
    {
        return type switch
        {
            1 => round(d, 2, 6, 1),
            2 => round(d, 2, 5, 1),
            3 => round(d, 2, 4, 1),
            4 => round(d, 2, 3, 1),
            5 => round(d, 2, 2, 1),
            6 => round(d, 2, 1, 1),
            7 => round(d, 2, 0, 1),
            8 => round(d, 2, 1, 0),
            9 => round(d, 2, 2, 0),
            10 => round(d, 2, 3, 0),
            11 => round(d, 2, 4, 0),
            12 => round(d, 2, 5, 0),
            13 => round(d, 2, 6, 0),
            21 => round(d, 3, 6, 1),
            22 => round(d, 3, 5, 1),
            23 => round(d, 3, 4, 1),
            24 => round(d, 3, 3, 1),
            25 => round(d, 3, 2, 1),
            26 => round(d, 3, 1, 1),
            27 => round(d, 3, 0, 1),
            28 => round(d, 3, 1, 0),
            29 => round(d, 3, 2, 0),
            30 => round(d, 3, 3, 0),
            31 => round(d, 3, 4, 0),
            32 => round(d, 3, 5, 0),
            33 => round(d, 3, 6, 0),
            41 => round(d, 1, 6, 1),
            42 => round(d, 1, 5, 1),
            43 => round(d, 1, 4, 1),
            44 => round(d, 1, 3, 1),
            45 => round(d, 1, 2, 1),
            46 => round(d, 1, 1, 1),
            47 => round(d, 1, 0, 1),
            48 => round(d, 1, 1, 0),
            49 => round(d, 1, 2, 0),
            50 => round(d, 1, 3, 0),
            51 => round(d, 1, 4, 0),
            52 => round(d, 1, 5, 0),
            53 => round(d, 1, 6, 0),
            //case 0: 
            _ => d,
        };
    }

    /// <summary>
    /// جذر عدد ورودی
    /// </summary>
    /// <param name="d">عدد ورودی</param>
    /// <returns></returns>
    public static double sqrt(double d)
    {
        return Math.Sqrt(d);
    }

    public static double sin(double d)
    {
        return Math.Sin(d);
    }

    public static double cos(double d)
    {
        return Math.Cos(d);
    }

    public static double tan(double d)
    {
        return Math.Tan(d);
    }

    public static double sinh(double d)
    {
        return Math.Sinh(d);
    }

    public static double cosh(double d)
    {
        return Math.Cosh(d);
    }

    public static double tanh(double d)
    {
        return Math.Tanh(d);
    }

    public static double asin(double d)
    {
        return Math.Asin(d);
    }

    public static double acos(double d)
    {
        return Math.Acos(d);
    }

    public static double atan(double d)
    {
        return Math.Atan(d);
    }

    public static double exp(double d)
    {
        return Math.Exp(d);
    }

    public static double log(double d)
    {
        return Math.Log(d);
    }

    public static double log10(double d)
    {
        return Math.Log10(d);
    }

    /// <summary>
    /// حاصل مقدار x به توان  y
    /// </summary>
    /// <param name="x">عدد x</param>
    /// <param name="y">عدد y</param>
    /// <returns></returns>
    public static double pow(double x, double y)
    {
        return Math.Pow(x, y);
    }

    public static bool has(int Option, long Options)
    {
        return ((Options & (1 << Option)) != 0);
    }

    public static bool hasbit(long Options, int Option)
    {
        return ((Options & (1 << Option)) != 0);
    }
    public static long RightShift(long inputBit, int shiftNum)
    {
        return inputBit >> shiftNum;
    }
    public static long BitWiseAnd(object inputL1, object inputL2)
    {
        return !long.TryParse(inputL1.ToString(), out long l1) ||
            !long.TryParse(inputL2.ToString(), out long l2)
            ? 0
            : l1 & l2;
    }
    public static byte SpecificByteOfNumber(object input, int byteNumber)
    {
        if (input != null && long.TryParse(input.ToString(), out long l))
        {
            byte[] bytes = BitConverter.GetBytes(l);
            return bytes[byteNumber];
        }
        return byte.MinValue;
    }
}
