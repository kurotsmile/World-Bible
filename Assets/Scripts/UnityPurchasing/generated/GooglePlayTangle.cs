// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("jqxEAqzK5gVth3/tWUW+alJUgxxaf/M7FWw1aEyMDm3SmAMYJDQbqZ8H2pNUsgKm/4Y5MVQ3PF2D+VIfkmFRy2F6AMNEqPHVnyyLautr4icTLfa9rsiLeEPX0or6zHYgZ4+lJJ0eEB8vnR4VHZ0eHh+JT+HsOMkPJP9cfIYK3E9AFYkWrDeWrXXnTDA5euIT1ngyTSZws+RLIW3R8Eb2Qi+dHj0vEhkWNZlXmegSHh4eGh8c63bqtj9KXwlS0LA2v/dAz9HykpHby48KXFcfNj/UlUjQG+yMc5EDEEzXiHmnGd9A60CqnZrw1CeYzbpslkINogaMXPOFrgTi34fd6MAP8X7CFCGSfOiJDQx6uu9Ks/LUAFViZw1nvIbfs7uG5h0cHh8e");
        private static int[] order = new int[] { 4,7,3,8,5,7,6,13,13,10,10,11,12,13,14 };
        private static int key = 31;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
