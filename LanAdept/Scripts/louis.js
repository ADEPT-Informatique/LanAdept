$(document).ready(function () {
    cheet('↑ ↑ ↓ ↓ ← → ← → b a', function () {
        if (window.innerWidth >= 1150 && window.innerHeight >= 610) {
            $("#raysLogo").css('top', window.innerHeight);
            $("#raysDemoHolder").fadeIn("slow", function () {
                $("#raysLogo").animate({
                    top: "-=500px"
                }, 2000, function () {
                    // Animation complete.
                    $("#textLogo").fadeIn("slow", function () {

                        setTimeout(function () {
                            $('.sound').get(0).play();
                            $('.sound').get(1).play();
                            $('#my-video').css('display', 'block')
                            var videoElement = $('#my-video').get(0);
                            videoElement.currentTime = 1;
                            videoElement.play();
                        }, 1000);
                    });
                });
            });
        }
    });
});