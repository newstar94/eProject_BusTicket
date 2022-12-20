$(document).ready(function () {
    $("#TypeID").change(function () {
        $.get("/Trips/Getvehicle",
            { TypeID: $("#TypeID").val() }, function (data) {
                $("#VehicleID").empty();
                $.each(data, function (index, row) {
                    $("#VehicleID").append("<option value='" + row.VehicleID + "'>" + row.Code + "</option>")
                });
            });
    })
    var options = document.querySelectorAll('option[value=""]');
    for (var i = 0; i < options.length; i++) {
        options[i].disabled = true;
    }
});

function fillSelectArr() {
    $('#DestinationID').children().remove().end();
    jQuery("#OriginID option").each(function () {
        if (jQuery(this).val() != $('#OriginID').val()) {
            if ($(this).val()!="") {
                $('#DestinationID').append('<option value=' + jQuery(this).val() + '>' + jQuery(this).text() + '</option>');
            }
        }
    });
}

function calculate() {
    var origin = $("#OriginID option:selected").text();
    var destination = $("#DestinationID option:selected").text();
    var service = new google.maps.DistanceMatrixService();
    service.getDistanceMatrix(
        {
            origins: [origin],
            destinations: [destination],
            travelMode: google.maps.TravelMode.DRIVING,
            unitSystem: google.maps.UnitSystem.METRIC,
            avoidHighways: false,
            avoidTolls: false,
        },
        callback
    );
    // get distance results
    function callback(response, status) {
        if (status == 'OK')
        {
            var distance = response.rows[0].elements[0].distance;
            console.log(distance);
            var duration = response.rows[0].elements[0].duration;
            console.log(duration);
            console.log(response.rows[0].elements[0].distance);
            var distance = (distance.value / 1000).toFixed(0);
            var duration_text = duration.text;
            document.getElementById("Distance").value = distance;
            document.getElementById("TripTime").value = duration_text;
        }
    }
}