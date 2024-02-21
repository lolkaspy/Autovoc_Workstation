using System;
using System.Security.Cryptography;
using System.Text;
namespace AutoVauxLauncher.HelpClasses
{
    public class HashSetter
    {
        public HashSetter() { }
        public string SHA512M(string pwd, string salt)
        {
            //получаем байткод пароля и соли
            var data = Encoding.UTF8.GetBytes(pwd + salt);
            //создаём реализацию класса SHA512
            var hash = SHA512.Create();
            //вычисляем хеш
            var result = hash.ComputeHash(data);
            //создаём построитель строки ёмкостью 128 символов, так как шифрование 512 бит, в байте 8 бит, 2 символа для байта
        var hashedInputStringBuilder = new System.Text.StringBuilder(128);
            foreach (var b in result)
                //форматируем части байткода в два 16-ричных символа в нижнем регистре
                hashedInputStringBuilder.Append(b.ToString("x2"));
            return hashedInputStringBuilder.ToString();
        }
        public string Saltate()
        {
            Random rand = new Random();
            string set = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890";
            string result = "";
            for (int i = 1; i < 16; i++)
            {
                result = result + set[rand.Next(0, set.Length - 1)];
            }
            return result;
        }
    }
}
