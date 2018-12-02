using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ImageProcessing.DataObjects;

namespace ImageProcessing.Controllers
{
    [Route("api/reset")]
    [ApiController]
    public class ResetController : Controller
	{
		// POST: api/<controller>
		[HttpPost]
		[Route("/api/reset")]
		public IActionResult Post()
		{
			Image image = Image.CurrentImage;

			return Json(image.GetPixelArray(Original: true));
		}
	}
}