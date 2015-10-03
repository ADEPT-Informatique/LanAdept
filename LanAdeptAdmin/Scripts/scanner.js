$(document).ready(function () {
	var pressed = false;
	var chars = [];
	$(window).keypress(function (e) {
		if (e.which >= 48 && e.which <= 57) {
			chars.push(String.fromCharCode(e.which));
		}
		//console.log(e.which + ":" + chars.join("|"));
		if (pressed == false) {
			setTimeout(function () {
				if (chars.length >= 8) {
					var barcode = chars.join("");
					console.log("Barcode Scanned: " + barcode);

					var form = $('<form></form>');
					form.attr("method", "post");
					form.attr("action", "/lanadeptadmin/place/search/");

					var field = $('<input></input>');
					field.attr("type", "hidden");
					field.attr("name", "Query");
					field.attr("value", barcode);

					form.append(field);
					$(document.body).append(form);

					form.submit();
				}
				chars = [];
				pressed = false;
			}, 500);
		}
		pressed = true;
	});
});
