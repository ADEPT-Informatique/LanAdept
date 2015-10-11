$(document).ready(function () {

    var jourSemaine = ["Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi", "Dimanche"];
    var mois = ['janvier', 'février', 'mars', 'avril', 'mai', 'juin', 'juillet', 'août', 'septembre', 'octobre', 'novembre', 'décembre'];
    var timer;

    function setTimer() {
        var dateNow = new Date();
        var timeDiff = new Date(dateLan - dateNow);

        if (timeDiff < 0) {
            $("#date").html("Le site sera en ligne dans quelques instants...<br /> Merci de patienter.");
            $(".countdown").hide();
            clearTimeout(timer);
            return;
        }
            

        var seconds = Math.floor((dateLan.getTime() - (new Date().getTime())) / 1000);
        var minutes = Math.floor(seconds / 60);
        var hours = Math.floor(minutes / 60);
        var days = Math.floor(hours / 24);

        hours = hours - (days * 24);
        minutes = minutes - (days * 24 * 60) - (hours * 60);
        seconds = seconds - (days * 24 * 60 * 60) - (hours * 60 * 60) - (minutes * 60);

        //$("#timer").text("Prochain LAN dans " + days + " jours " + addLeadingZero(hours) + ":" + addLeadingZero(minutes) + ":" + addLeadingZero(seconds));
        $("#cpt_days").text(days);
        $("#cpt_hours").text(addLeadingZero(hours));
        $("#cpt_minutes").text(addLeadingZero(minutes));
        $("#cpt_seconds").text(addLeadingZero(seconds));

    }

    function dateToString(date) {
        var strDate = "";

        strDate += jourSemaine[date.getDay() - 1];
        strDate += " " + date.getDate();
        strDate += " " + mois[date.getMonth()];
        strDate += " " + date.getFullYear();
        strDate += " à " + addLeadingZero(date.getHours());
        strDate += ":" + addLeadingZero(date.getMinutes());

        return strDate;
    }

    function addLeadingZero(number) {
        return ("0" + number).slice(-2);
    }

    $(".countdown").show();

    var dateLan = new Date(2015, 9, 7, 12, 0, 0, 0);

    $("#date").text(dateToString(dateLan));

    setTimer();
    timer = setInterval(setTimer, 1000);
});
