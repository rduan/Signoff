using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Signoff
{
	public class HistoryAdapter : BaseAdapter<Signature> 
	{
		List<Signature> items;
		Activity context;
		public HistoryAdapter(Activity context, List<Signature> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}

		public override Signature this[int position]
		{
			get { return items[position]; }
		}

		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.HistoryDisplay, null);

			view.FindViewById<TextView>(Resource.Id.NameSigned).Text = item.Name;
			view.FindViewById<TextView>(Resource.Id.DateSigned).Text = item.DateOfSigned.ToString();
			view.FindViewById<ImageView>(Resource.Id.Image).SetImageBitmap(BitmapFactory.DecodeByteArray(item.ImageByte,0,item.ImageByte.Length));
			

			return view;
		}

		public void updateList(List<Signature> newList) {
			items = newList;
			NotifyDataSetChanged ();
		}
	}
}

