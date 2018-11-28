/// <reference path="jquery-3.3.1.js" />

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

	var url = "api/palette/calc";

	var data = {
		PixelData: this.pixels,
		PaletteSize: paletteSize,
		Width: this.canvas.width(),
		Height: this.canvas.height()
	};

	// Set palette
	var success = function (result) {
		console.log('Data received: ');
		console.log(result);
	};

	// Error alert
	var error = function (data) {
		alert("There was an error. Oh no!");
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