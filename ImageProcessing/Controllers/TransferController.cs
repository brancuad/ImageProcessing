using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ImageProcessing.DataObjects;
using ImageProcessing.Tools;

namespace ImageProcessing.Controllers
{
    [Route("api/transfer]")]
    public class TransferController : Controller
    {
		public class TransferInput
		{
			public List<double[]> NewPalette
			{
				get; set;
			}
		}

		// POST: api/<controller>
		[HttpPost]
		[Route("/api/transfer")]
		public IActionResult Post([FromBody]TransferInput input)
		{
			Image image = Image.CurrentImage;

			List<Color> NewPalette = ColorTransfer.FormatPalette(input.NewPalette);

			image.TransferPalette(NewPalette);
			
			return Json(image.GetPixelArray());
		}
	}
}