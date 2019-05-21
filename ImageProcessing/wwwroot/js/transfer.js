/// <reference path="jquery-3.3.1.js" />
/// <reference path="imageData.js" />

// Front End for Dashboard.cshtml

var showLoading = function () {
	display.canvas.fadeTo("medium", .3);
};

var hideLoading = function () {
	display.canvas.fadeTo("fast", 1);
};

var display;

var readFile = function (e) {
	var reader = new FileReader();

	reader.onload = function (event) {

		display.image.onload = function () {
			// draw image in canvas
			display.draw();

			$("#calc").mousedown(showLoading).mouseup(function () {
				// Hide suggestions. No longer applicable
				$("#suggestion_container").hide();

				// set pixel array
				display.getPixels();

				paletteSize = $("input[name=kPicker]:checked").val();
				display.setPalette(paletteSize);
			});

			$("#transfer").mousedown(showLoading).mouseup(function () {
				var newPalette = display.getNewPalette();

				display.recolor(newPalette, display);
			});

			$("#suggest").mousedown(showLoading).mouseup(function () {
				display.showSuggestions();
				hideLoading();
			});

			$("#reset").mousedown(showLoading).mouseup(function () {
				display.reset(display);
			});

			$("#equalize").mousedown(showLoading).mouseup(function () {
				display.equalize($("#equal_slider").val() / 100);
			});
		};

		display.image.src = event.target.result;
		$(".btn").prop("disabled", false);
	};

	reader.readAsDataURL(e.target.files[0]);
};

$(document).ready(function () {

	display = new Img($("#display"));

	$("#file_button").change(readFile);

	$(".btn").prop("disabled", true);
	$("#upload").prop("disabled", false);

	$("#upload").click(function () {
		$("#file_button").click();
	});

	$("#equal_slider").change(function () {
		$("#equal_value").text($("#equal_slider").val() + "%");
	});
});