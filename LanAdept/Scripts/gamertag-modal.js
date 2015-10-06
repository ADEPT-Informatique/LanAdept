$(document).ready(function () {

    $('.tagItem').click(function (event) {
        $('#GamerTagID').val($(this).attr('data-tag-id'));
        $('.active').removeClass('active');
        $(this).addClass('active')
    })

    $('.addGamertag').click(function (event) {
        $.ajax({
            method: "POST",
            url: "@Url.Action("AddGamerTag")",
            data: { gamertag: $('#gamerTagText').val() }
    }).done(function (data) {
        console.log(data);
        $('.tagItem:last').after('<a href="#" class="tagItem list-group-item" data-tag-id="' + data.GamerTagID + '">' + data.Gamertag + '</a>')
    })
})

$('#ModalChoisirGamerTag').on('show.bs.modal', function (event) {
    var button = $(event.relatedTarget);
    var modal = $(this);
})
})