using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Graphics;
using SignaturePad;
using SQLite;
using System.IO;

namespace Signoff
{
	[Activity (Label = "Signoff", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		//int count = 1;
		System.Drawing.PointF [] points;
		string name;
		Bitmap signatureImage;
		static readonly List<string> signatures = new List<string>();

		/*
		string dbPath = System.IO.Path.Combine (
			Environment.GetFolderPath (Environment.SpecialFolder.Personal),
			"database.db3");
		*/

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);


			// create DB path
			var dbFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var dbPath = System.IO.Path.Combine(dbFolder, "signoffdb1.db3");


			EditText nameText = FindViewById<EditText> (Resource.Id.nameText);
			SignaturePadView signature = FindViewById<SignaturePadView> (Resource.Id.signatureView);
			Button btnSave = FindViewById<Button> (Resource.Id.btnSave);
			Button btnHistory = FindViewById<Button> (Resource.Id.btnHistory);
			Button btnClear = FindViewById<Button> (Resource.Id.btnClear);


			if (true) { // Customization activated
				View root = FindViewById<View> (Resource.Id.rootView);
				root.SetBackgroundColor (Color.White);

				// Activate this to internally use a bitmap to store the strokes
				// (good for frequent-redraw situations, bad for memory footprint)
				// signature.UseBitmapBuffer = true;

				signature.Caption.Text = "Authorization Signature";
				signature.Caption.SetTypeface (Typeface.Serif, TypefaceStyle.BoldItalic);
				signature.Caption.SetTextSize (global::Android.Util.ComplexUnitType.Sp, 16f);
				signature.SignaturePrompt.Text = ">>";
				signature.SignaturePrompt.SetTypeface (Typeface.SansSerif, TypefaceStyle.Normal);
				signature.SignaturePrompt.SetTextSize (global::Android.Util.ComplexUnitType.Sp, 32f);
				signature.BackgroundColor = Color.Rgb (0, 0, 0); // 255,255,200 a light yellow.
				signature.StrokeColor = Color.White;
				signature.StrokeWidth = 20;
				signature.BackgroundImageView.SetImageResource (Resource.Drawable.logo_galaxy_black_64);
				signature.BackgroundImageView.SetAlpha (16);
				signature.BackgroundImageView.SetAdjustViewBounds (true);
				var layout = new RelativeLayout.LayoutParams (RelativeLayout.LayoutParams.FillParent, RelativeLayout.LayoutParams.FillParent);
				layout.AddRule (LayoutRules.CenterInParent);
				layout.SetMargins (20, 20, 20, 20);
				signature.BackgroundImageView.LayoutParameters = layout;

				// You can change paddings for positioning...
				var caption = signature.Caption;
				caption.SetPadding (caption.PaddingLeft, 1, caption.PaddingRight, 25);

			}

			//
			btnSave.Click += delegate {				
				if (signature.IsBlank)
				{//Display the base line for the user to sign on.
					AlertDialog.Builder alert = new AlertDialog.Builder (this);
					alert.SetMessage ("No signature to save.");
					alert.SetNeutralButton ("Okay", delegate { });
					alert.Create ().Show ();
				}
				points = signature.Points;
				name = nameText.Text;
				signatureImage = signature.GetImage ();
				var db = new SQLiteConnection (dbPath);
				db.CreateTable<Signature>();
				var result = saveSignature(new Signature{ Name = name, DateOfSigned = DateTime.Now, ImageByte = getBitmapAsByteArray(signatureImage) }, dbPath);
				signatures.Add(name);
			};
			//btnSave.Dispose ();

			//
			btnHistory.Click += (sender, e) =>
			{
				var intent = new Intent(this, typeof(SignatureHistory));
				intent.PutStringArrayListExtra("signatures", signatures);
				StartActivity(intent);
			};

			//btnHistory.Dispose ();

			//
			btnClear.Click += delegate {
				signature.Clear();
			};
			//btnClear.Dispose ();

		}

		private string saveSignature(Signature data, string path)
		{
			try
			{
				var db = new SQLiteConnection(path);
				if (db.Insert(data) != 0)
					db.Update(data);
				return "Single data file inserted or updated";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		public static byte[] getBitmapAsByteArray(Bitmap bitmap) {
			
			MemoryStream stream = new MemoryStream();
			bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
			return stream.ToArray();

		}
	}
}


