if (window.location.href !== "https://localhost:44312/Admin/TripSchedules") {
    var selects = document.getElementsByTagName("select");
        for (var j = 0; j < selects.length; j++) {
            selects[j].setAttribute("required", "");
        }
}

$(document).ready(function () {
    $("#Origin").on('change', function () {
        $('#Destination').children().remove().end();
        jQuery("#Origin option").each(function () {
            if (jQuery(this).val() != $('#Origin').val()) {
                if ($(this).val() != "") {
                    $('#Destination').append('<option value=' + jQuery(this).val() + '>' + jQuery(this).text() + '</option>');
                }
            }
        });
    });

    var options = document.querySelectorAll('option[value=""]');
    for (var i = 0; i < options.length; i++) {
        options[i].disabled = true;
    }
});