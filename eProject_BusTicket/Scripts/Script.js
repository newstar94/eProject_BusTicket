$(document).ready(function () {
    //$("#TypeID").change(function () {
    //    $.get("/Trips/Getvehicle",
    //        { TypeID: $("#TypeID").val() }, function (data) {
    //            $("#VehicleID").empty();
    //            $.each(data, function (index, row) {
    //                $("#VehicleID").append("<option value='" + row.VehicleID + "'>" + row.Code + "</option>")
    //            });
    //        });
    //})
    var options = document.querySelectorAll('option[value=""]');
    for (var i = 0; i < options.length; i++) {
        options[i].disabled = true;
    }
});