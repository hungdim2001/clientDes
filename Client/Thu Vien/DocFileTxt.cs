using Client.Doi_tuong;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Client.Thu_Vien
{
	class DocFileTxt
	{
		public static ChuoiNhiPhan FileReadToBinary(string filename)
		{
			//FileStream fs = new FileStream(filename, FileMode.Open);
			ChuoiNhiPhan chuoi;
			// List<int> chuoiLon = new List<int>() ;
			ChuoiNhiPhan KQ = new ChuoiNhiPhan(0);
			byte[] fileBytes = File.ReadAllBytes(filename);
			int i = 0;
			Parallel.ForEach(fileBytes, b =>
			{
				chuoi = ChuoiNhiPhan.ChuyenSoSangNhiPhan(b, 8);
				KQ = KQ.Cong(chuoi);
				Console.WriteLine(i++);
			});

			/*foreach (byte b in fileBytes)
			{
				chuoi = ChuoiNhiPhan.ChuyenSoSangNhiPhan(b, 8);
				KQ = KQ.Cong(chuoi);

				//text.Append(ChuoiNhiPhan.ChuyenSoSangStringNhiPhan(b,8));
			}*/
			//KQ = new ChuoiNhiPhan(chuoiLon.ToArray());

			return KQ;

		}

		public static void WriteBinaryToFile(string filename, ChuoiNhiPhan chuoiVao)
		{
			byte[] MangByte = new byte[chuoiVao.MangNhiPhan.Length / 8];
			Console.WriteLine(MangByte.Length);
			for (int i = 0; i < chuoiVao.MangNhiPhan.Length / 8; i++)
			{
				MangByte[i] = (byte)ChuoiNhiPhan.ChuyenMangSangByte(chuoiVao.MangNhiPhan, i * 8, i * 8 + 8);
			}
			File.WriteAllBytes(filename, MangByte);

		}
	}
}
