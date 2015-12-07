using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SQLite;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Signoff
{
	[Activity (Label = "Signature History")]			
	public class SignatureHistory : Activity
	{
		List<Signature> signatureItems = new List<Signature>();
		ListView listView;
		Boolean sortStatus = true;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);


			SetContentView (Resource.Layout.HomeScreen);
			listView = FindViewById<ListView> (Resource.Id.List);
			Button btnSearch = FindViewById<Button> (Resource.Id.btnSearch);
			Button btnSort = FindViewById<Button> (Resource.Id.btnSort);
			EditText searchText = FindViewById<EditText>(Resource.Id.searchName);

			//build signatureItems
			var dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var dbPath = System.IO.Path.Combine(dbFolder, "signoffdb1.db3");
			var db = new SQLiteConnection (dbPath);
			var query = db.Table<Signature>();

			foreach (var signatureItem in query)
				signatureItems.Add(signatureItem);
			
			listView.Adapter = new HistoryAdapter(this, signatureItems);


			btnSearch.Click += delegate {
				signatureItems.Clear();
				string queryLike = "SELECT * FROM Signature WHERE NAME LIKE " + "'%" + searchText.Text + "%'";
				var sortItems = db.Query<Signature>(queryLike);
				foreach (var signatureItem in sortItems)
					signatureItems.Add(signatureItem);
				listView.Adapter.Dispose();
				listView.Adapter = null;
				listView.Adapter = new HistoryAdapter(this, signatureItems);
				
			};

			btnSort.Click += delegate {
				List<Signature> sortedItems;
				if (sortStatus) {
					sortedItems = signatureItems.OrderBy(s =>s.Name).ToList();
					sortStatus = false;
				}
				else {
					sortedItems = signatureItems.OrderByDescending(s =>s.Name).ToList();
					sortStatus = true;
				}
				listView.Adapter.Dispose();
				listView.Adapter = null;
				listView.Adapter = new HistoryAdapter(this, sortedItems);
			};

		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = signatureItems[e.Position];
			Android.Widget.Toast.MakeText(this, t.Name, Android.Widget.ToastLength.Short).Show();
			//Console.WriteLine("Clicked on " + t.Heading);
		}
	}
}

