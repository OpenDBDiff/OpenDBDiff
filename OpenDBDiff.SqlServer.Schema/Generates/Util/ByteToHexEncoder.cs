namespace OpenDBDiff.SqlServer.Schema.Generates.Util
{
    /// <summary>
    /// This class implements a fast conversion from a byte array to an hex string.
    /// </summary>
    public class ByteToHexEncoder
    {
        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                var s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        public static string ByteArrayToHex(byte[] bytes)
        {
            var result = new char[2 + bytes.Length * 2];

            result[0] = '0';
            result[1] = 'x';

            for (int i = 0; i < bytes.Length; i++)
            {
                var val = _lookup32[bytes[i]];
                result[2 * i + 2] = (char)val;
                result[2 * i + 3] = (char)(val >> 16);
            }

            return new string(result);
        }
    }
}
