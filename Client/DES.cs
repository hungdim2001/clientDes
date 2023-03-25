using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Client
{
	public class DES
	{
		private TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
		public DES(string key)
		{
			des.Key = UTF8Encoding.UTF8.GetBytes(key + "12345678");
			des.Mode = CipherMode.ECB; //Electronic Code Book
			des.Padding = PaddingMode.PKCS7;
		}

		public void EncryptFile(string filepath)
		{
			byte[] Bytes = File.ReadAllBytes(filepath);
			byte[] eBytes = des.CreateEncryptor().TransformFinalBlock(Bytes, 0, Bytes.Length);
			File.WriteAllBytes(filepath, eBytes);
		}

		public void DecryptFile(string filepath)
		{
			byte[] Bytes = File.ReadAllBytes(filepath);
			byte[] dBytes = des.CreateDecryptor().TransformFinalBlock(Bytes, 0, Bytes.Length);
			File.WriteAllBytes(filepath, dBytes);
		}
	}
}
