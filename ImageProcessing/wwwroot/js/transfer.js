/// <reference path="jquery-3.3.1.js" />
/// <reference path="imageData.js" />

// Front End for Transfer.cshtml

$(document).ready(function () {

	var origin = new Img($("#origin"));
	var output = new Img($("#output"));

	make_base();

	function make_base() {

		origin.image.onload = function () {
			// draw image in canvas
			origin.draw();

			$("#calc").mousedown(showLoading).mouseup(function () {

				// set pixel array
				origin.getPixels();

				origin.paletteSize = $("input[name=kPicker]:checked").val();
				origin.palette = origin.getPalette();

				origin.weights = origin.getWeights();

				origin.showPalette();

				hideLoading();
			});

			$("#transfer").mousedown(showLoading).mouseup(function () {

				var recolorPixels = origin.recolor(origin.getNewPalette());

				// Use original pixels for now
				// var recolorPixels = origin.pixels;
				var flatPixels = origin.flattenPixels(recolorPixels);

				var imgData = output.getTransferData(flatPixels);

				output.putImageData(imgData);


				hideLoading();
			});
		};

		origin.image.src = $("#imgSelect").val();

		$("#imgSelect").change(function () {
			origin.image.src = $(this).val();
		});
	}
});