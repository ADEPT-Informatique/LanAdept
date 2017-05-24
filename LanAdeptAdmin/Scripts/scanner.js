$(document).ready(function () {
	var pressed = false;
	var chars = [];
	$(window).keypress(function (e) {
		if ((e.which >= 48 && e.which <= 57) || (e.which >= 65 && e.which <= 90) || (e.which >= 97 && e.which <= 122)) {
			chars.push(String.fromCharCode(e.which));
		}
		if (pressed == false) {
			setTimeout(function () {
				if (chars.length >= 8) {
					var barcode = chars.join("");
					window.location = "/Place/Search/?Query=" + barcode;
				}
				chars = [];
				pressed = false;
			}, 500);
		}
		pressed = true;
	});
});
