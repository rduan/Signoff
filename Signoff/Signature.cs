using System;
using SQLite;
using Android.Graphics;

namespace Signoff
{
	public class Signature
	{
		[PrimaryKey, AutoIncrement]
		public int ID { get; set; }

		public string Name { get; set; }

		public Byte[] ImageByte { get; set; }

		public DateTime DateOfSigned { get; set; }

		public override string ToString()
		{
			return string.Format("[Signature: ID={0}, Name={1}, ImageByte={2}, DateOfSigned={3}]", ID, Name, ImageByte, DateOfSigned);
		}
	}
}

