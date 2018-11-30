using ImageProcessing.DataObjects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ImageProcessing.Tools;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImageProcessing.Controllers
{
	[Route("api/palette")]
	public class PaletteController : Controller
	{
		public class PaletteInput
		{
			public List<int> PixelData
			{
				get; set;
			}
			public int PaletteSize
			{
				get; set;
			}
			public int Width
			{
				get; set;
			}
			public int Height
			{
				get; set;
			}
		}

		// POST: api/<controller>/calc
		[HttpPost]
		[Route("/api/palette/calc")]
		public IActionResult Post([FromBody]PaletteInput input)
		{
			Image image = Image.GetImageFromArray(input.PixelData, width: input.Width, height: input.Height);

			image.SetPalette(input.PaletteSize);

			Image.CurrentImage = image;

			double[][] formattedPalette = PaletteManager.FormatData(image.Palette, rgb: true);

			return Json(formattedPalette);
		}
	}
}
