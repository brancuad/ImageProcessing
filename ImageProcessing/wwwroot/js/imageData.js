/// <reference path="jquery-3.3.1.js" />
/// <reference path="Original/jqColor.js" />

// Img class to store images in javascript

// Initialize Img
function Img(canvas) {
	this.canvas = canvas;

	this.context = canvas[0].getContext('2d');

	this.image = new Image();
}

// Draw the image onto its canvas
Img.prototype.draw = function (x = 0, y = 0) {
	var maxWidth = this.canvas.width();
	var maxHeight = this.canvas.height();

	var ratio = Math.min(maxWidth / this.image.width,
		maxHeight / this.image.height);

	this.context.drawImage(this.image, x, y,
		this.image.width * ratio, this.image.height * ratio);
};

// Get and set pixel array
Img.prototype.getPixels = function (x = 0, y = 0) {

	flatData = this.context.getImageData(x, y, this.canvas.width(), this.canvas.height());

	// Create pixel array
	data = flatData.data;

	/*
	for (var i = 0; i < flatData.data.length; i += 4) {
		pixel = [];

		// R
		pixel.push(flatData.data[i]);
		// G
		pixel.push(flatData.data[i + 1]);
		// B
		pixel.push(flatData.data[i + 2]);
		// A
		pixel.push(flatData.data[i + 3]);

		data.push(pixel);
	}
	*/

	this.pixels = Array.from(data);

	return data;
};

// Use an ajax call to set the palette of the image
Img.prototype.setPalette = function (paletteSize) {
	var self = this;
	self.paletteSize = paletteSize;

	var url = "api/palette/calc";

	var data = {
		PixelData: this.pixels,
		PaletteSize: paletteSize,
		Width: Math.round(this.canvas.width()),
		Height: Math.round(this.canvas.height())
	};

	// Set palette
	var success = function (result) {
		console.log('Data received: ');
		console.log(result);
		self.palette = result;
		self.showPalette();
	};

	// Error alert
	var error = function (data) {
		alert("There was an error. Oh no!");
	};

	this.ajax(url, data, success, error);
};

Img.prototype.showPalette = function () {
	colorPickerInit(jQuery, window);

	var palette = this.palette;

	$("input.color").remove();

	var rgbString = function (rgba) {
		return "rgb(" + parseInt(rgba[0]) + ", " + parseInt(rgba[1]) + ", " + parseInt(rgba[2]) + ")";
	};

	for (var i = 0; i < palette.length; i++) {

		var colorElement = $("<input></input>")
			.attr("id", "originColor" + (i + 1))
			.attr("type", "button")
			.addClass("color");

		$("#colorContainer").append(colorElement);

		colorElement.val(rgbString(palette[i]));
		colorElement.css("background-color", rgbString(palette[i]));
		colorElement.colorPicker();
	}
};

Img.prototype.getNewPalette = function () {
	var stringtoRgb = function (rgbString) {
		var rgb = rgbString.replace(/[^\d,]/g, '').split(',');

		for (var i in rgb) {
			rgb[i] = parseInt(rgb[i]);
		}

		return rgb;
	};

	newPalette = [];

	for (var i = 1; i <= this.paletteSize; i++) {
		var color = $("#originColor" + i).css("backgroundColor");
		var rgb = stringtoRgb(color);
		newPalette.push(rgb);
	}

	return newPalette;
};

// Use ajax call to recolor the image
Img.prototype.recolor = function (newPalette, output) {
	var self = this;

	var url = "api/transfer";

	var data = {
		NewPalette: newPalette
	};

	var success = function (result) {
		imgData = output.context.createImageData(output.canvas.width(), output.canvas.height());

		for (var i = 0; i < result.length; i++) {
			imgData.data[i] = result[i];
		}

		output.context.putImageData(imgData, x = 0, y = 0);

		hideLoading();
	};

	var error = function (data) {
		alert("Oh no!");

		hideLoading();
	};

	this.ajax(url, data, success, error);

};

// Make an ajax call to the url, with the data stringified
Img.prototype.ajax = function (url, data, success, error) {
	console.log("Submitting form...");
	$.ajax({
		type: "POST",
		url: url,
		data: JSON.stringify(data),
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		success: success,
		error: error
	}).done(function () {
		hideLoading();
	});
};