using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace ImageProcessing.Pages
{
	public class TransferModel : PageModel
	{
		public void OnGet()
		{
		}


		// GET: Transfer/CalcPalette
		[HttpGet("{pixels, paletteSize}", Name = "CalcPalette")]
		public string CalcPalette(string pixels, string paletteSize)
		{
			return String.Empty;
		}
	}
}