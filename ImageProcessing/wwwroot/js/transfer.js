/// <reference path="jquery-3.3.1.js" />
/// <reference path="imageData.js" />

// Front End for Transfer.cshtml

var showLoading = function () {
	$("#loading").show();
};

var hideLoading = function () {
	$("#loading").hide();
};

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

				paletteSize = $("input[name=kPicker]:checked").val();
				origin.setPalette(paletteSize);
			});

			$("#transfer").mousedown(showLoading).mouseup(function () {
				var newPalette = origin.getNewPalette();

				origin.recolor(newPalette, output);
			});
		};

		origin.image.src = $("#imgSelect").val();

		$("#imgSelect").change(function () {
			origin.image.src = $(this).val();
		});
	}
});