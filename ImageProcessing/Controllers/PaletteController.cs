using ImageProcessing.DataObjects;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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

			return Json(image.Palette);
		}
	}
}
