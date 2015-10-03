
$(function () {
	$(".clickable-row").click(function () {
		document.location = $(this).data("rowUrl");
	});
});