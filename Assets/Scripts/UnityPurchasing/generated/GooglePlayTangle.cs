// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("g2GpO7BbjiHFgLjkfxxGQXj626/MibFPyq4GF+4rA8v2CE+hqkxgITkkf2niwTDrs4aMWfmvFZvGb5F2G6dFCnwnA7nV9ztzBk2SQ6xi9xmtVm/uU+Ky/o5PFh7qI9iuAOe0XW1kCp5S7YnaTaZntfuDU2SvWwa1Pp2SmoJc6oL3hLeAuoqZB+624uVDFzRsOME9dd38ZHXuCCJN2UWLNF9Se5uzjPZ/oV/Tv0oPTTV2dd+yLHa50y9jxkiokz94Pr3FjyVeo9VGxcvE9EbFzsZGxcXETkZfOwZX6nEwzwV1Q+U/PhZB6UEKTESoBoU99EbF5vTJws3uQoxCM8nFxcXBxMdyv15ExTYzjnWXdxtNOZWxLhcfTYOEEQcluBzYw8bHxcTF");
        private static int[] order = new int[] { 13,5,2,7,11,10,8,9,11,9,10,11,13,13,14 };
        private static int key = 196;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
