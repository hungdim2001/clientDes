using Client.Doi_tuong;
using Client.Thu_Vien;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace Client
{
	public partial class Form1 : Form
	{
		IPAddress ip;
		IPEndPoint end;
		Socket sock;
		String path;
		String fileName;
		public static string TenTienTrinh = "";
		public static int GiaiDoan = -1;
		private static int Dem = 0;
		int MaHoaHayGiaiMa = 1;
		bool FileHayChuoi = true;
		DES64Bit MaHoaDES64;
		Khoa Khoa;

		public Form1()
		{
			InitializeComponent();
			Connect();
		}
		public void Connect() {
			try {
				ip = IPAddress.Parse("127.0.0.1");
				end = new IPEndPoint(ip, 9999);
				sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
				sock.Connect(end);
			}
			catch(Exception ex) {
				//MessageBox.Show(ex.Message);
			}
			Thread listen  = new Thread(Receive); 
			listen.IsBackground= true;	
			listen.Start();
			
		}
	
		void Receive()
		{
			try
			{
				int size;
				while (true)
				{
					byte[] data = new byte[1024];
					size = sock.Receive(data);
					string[] s = Encoding.UTF8.GetString(data, 0, size).Split(new char[] { ',' }); // nhan ten file, duong dan, size.
					long length = long.Parse(s[1]);
					byte[] buffer = new byte[1024];
					byte[] fsize = new byte[length]; //khai bao mang byte de chua du lieu
					long n = length / buffer.Length;  // tính số frame sẽ được gửi qua
					for (int i = 0; i < n; i++)
					{
						size = sock.Receive(fsize, fsize.Length, SocketFlags.None);
						Console.WriteLine("Received frame {0}/{1}", i, n);
					}
					FileStream fs = new FileStream("C:/Users/hungdz/Desktop/client" + "/" + s[0], FileMode.Create);  // luu file s[0] vao duong dan s[1]
					fs.Write(fsize, 0, fsize.Length);
					fs.Close();
					MessageBox.Show("Receive Successfully");
				}
			}
			catch (Exception e)
			{
				sock.Close();
				//MessageBox.Show(e.Message);
			}

		}
		private void button3_Click(object sender, EventArgs e)
		{
			if (String.IsNullOrEmpty(textBox2.Text))
			{
				MessageBox.Show("key is not empty");
				return;
			}
			if (textBox2.Text.Length != 8)
			{
				MessageBox.Show("key must be 8 characterst");
				return;
			}
			MaHoaHayGiaiMa = 2;
			MaHoa();
		}

		private void button1_Click(object sender, EventArgs e)
		{

		}
		private void MaHoa()
		{

			MaHoaDES64 = new DES64Bit();

			TenTienTrinh = "";

			GiaiDoan = 0;
			Dem = 0;

			if (FileHayChuoi)
			{
				Khoa = new Khoa(textBox2.Text);
				if (MaHoaHayGiaiMa == 1)
				{

					GiaiDoan = 0;																if (GiaiDoan == 0) { Encrypt(); return; }
					ChuoiNhiPhan chuoi = DocFileTxt.FileReadToBinary(fileName + path);
					GiaiDoan = 1;
					ChuoiNhiPhan KQ = MaHoaDES64.ThucHienDES(Khoa, chuoi, 1);
					GiaiDoan = 2;
					DocFileTxt.WriteBinaryToFile(fileName + path, KQ);
					GiaiDoan = 3;

				}
				else
				{
					GiaiDoan = 0;																	 if (GiaiDoan == 0) { Decrypt(); return; }
					ChuoiNhiPhan chuoi = DocFileTxt.FileReadToBinary(fileName + path);
					GiaiDoan = 1;
					ChuoiNhiPhan KQ = MaHoaDES64.ThucHienDES(Khoa, chuoi, -1);
					if (KQ == null)
					{
						MessageBox.Show("Invalid Key");
						return;
					}
					GiaiDoan = 2;
					DocFileTxt.WriteBinaryToFile(fileName + path, KQ);
					GiaiDoan = 3;
				}
			}


		}

		private void button1_Click_1(object sender, EventArgs e)
		{
			OpenFileDialog OD = new OpenFileDialog();
			OD.Filter = "All Files|*";
			OD.FileName = "";
			if (OD.ShowDialog() == DialogResult.OK)
			{
				path = "";
				fileName = OD.FileName.Replace("\\", "/");
				while (fileName.IndexOf("/") > -1)
				{
					path += fileName.Substring(0, fileName.IndexOf("/") + 1);
					fileName = fileName.Substring(fileName.IndexOf("/") + 1);
				}

				textBox1.Text = path + fileName;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
				Connect();
			if (String.IsNullOrEmpty(textBox1.Text))
			{
				MessageBox.Show("please select file");
				return;
			}
			if (String.IsNullOrEmpty(textBox2.Text))
			{
				MessageBox.Show("key is not empty");
				return;
			}
			if (textBox2.Text.Length != 8)
			{
				MessageBox.Show("key must be 8 characterst");
				return;
			}
			try
			{
				MaHoaHayGiaiMa = 1;
				MaHoa();
				FileInfo file = new FileInfo(path + fileName);
				byte[] data = new byte[1024];
				byte[] fsize = new byte[file.Length]; // tạo mảng chứa dữ liệu
				FileStream fs = new FileStream(path + fileName, FileMode.Open); // đọc thông tin file đã nhập
				fs.Read(fsize, 0, fsize.Length);

				fs.Close();
				while (true)
				{
					sock.Send(Encoding.UTF8.GetBytes(fileName + "," + file.Length.ToString()));
					long n = file.Length / data.Length;  //tính số frame phải gửi

					for (int i = 0; i < n; i++)
					{
						Console.WriteLine("Sending frame {0}/{1}", i, n);
						sock.Send(fsize, fsize.Length, 0);
					}
					MessageBox.Show("Send File Successfully");
					break;
				}
			}
			catch
			{
				sock.Close();
			}
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}



















		public void Encrypt()
		{
			try
			{
				DES tDES = new DES(textBox2.Text);
				tDES.EncryptFile(textBox1.Text);
				GC.Collect();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return;
			}
		}
		public void Decrypt()
		{

			if (String.IsNullOrEmpty(textBox2.Text))
			{
				MessageBox.Show("key is not empty");
				return;
			}
			if (textBox2.Text.Length != 8)
			{
				MessageBox.Show("key must be 8 characterst");
				return;
			}
			try
			{
				DES tDES = new DES(textBox2.Text);
				tDES.DecryptFile(textBox1.Text);
				GC.Collect();
				MessageBox.Show("Decrypt successfully");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			
		}
	}
}
