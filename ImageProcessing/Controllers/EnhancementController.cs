using ImageProcessing.DataObjects;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessing.Controllers
{
	[Route("api/enhance")]
	[ApiController]
	public class EnhancementController : Controller
	{
		public class EnhancementInput
		{
			public double Weight
			{
				get; set;
			}
		}

		// POST: api/<controller>
		[HttpPost]
		[Route("/api/enhance")]
		public IActionResult Post([FromBody]EnhancementInput input)
		{
			Image image = Image.CurrentImage;

			image.Enhance(input.Weight);

			return Json(image.GetPixelArray());
		}
	}
}