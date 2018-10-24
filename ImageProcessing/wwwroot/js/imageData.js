/// <reference path="jquery-3.3.1.js" />

// Img class to store images in javascript

// Initialize Img
function Img(canvas: JQueryStatic) {
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
	data = [];
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

	this.pixels = data;

	return data;
}