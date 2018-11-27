using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ImageProcessing.DataObjects;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImageProcessing.Controllers
{
	[Route("api/palette")]
	public class PaletteController : Controller
	{
		// POST: api/<controller>
		[HttpPost]
		public string Get()
		{
			return String.Empty;
		}

		// POST api/<controller>
		[HttpPost]
		public void Post([FromBody]string value)
		{
		}
	}
}
