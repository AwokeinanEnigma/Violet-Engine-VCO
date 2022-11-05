﻿using System;
using Violet;
using Violet.Input;
using VCO.Overworld;
using SFML.Graphics;
using Violet.Graphics;
using System.IO;
using SFML.System;
using Violet.Utility;

namespace VCO.Scenes
{
	internal class Base64DebugTest : StandardScene
	{
		public Base64DebugTest()
		{

			byte[] bytes = Convert.FromBase64String("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");


			Graphic graphic = SCHIZOCode.SCHIZO_GraphicFromBase64(bytes, 200);
			pipeline.Add(graphic);
			//using (Image image = Image.from(new MemoryStream(bytes)))
			//{
			//	image.Save("output.jpg", ImageFormat.Jpeg);  // Or Png
			//}
			//var ind = new IndexedTexture()
		}

		private void ButtonPressed(InputManager sender, Button b)
		{
		}

		public override void Focus()
		{
			base.Focus();
			InputManager.Instance.ButtonPressed += this.ButtonPressed;
			Engine.ClearColor = Color.Magenta;
		}

		public override void Update()
		{
			base.Update();
		}

		public override void Unfocus()
		{
			base.Unfocus();
			InputManager.Instance.ButtonPressed -= this.ButtonPressed;
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed && disposing)
			{
			}
			base.Dispose(disposing);
		}
	}
}
