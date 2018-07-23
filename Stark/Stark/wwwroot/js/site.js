// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(init);
function init() {
    $('#0').click(function () {
        $("#Plate").val($("#0").val());
        $("input[class^='selectButton']").removeClass('selectButton');
        $("#0").addClass("selectButton");
    });
    $('#1').click(function () {
        $("#Plate").val($("#1").val());
        $("input[class^='selectButton']").removeClass('selectButton');
        $("#1").addClass("selectButton");
    }); $('#2').click(function () {
        $("#Plate").val($("#2").val());
        $("input[class^='selectButton']").removeClass('selectButton');
        $("#2").addClass("selectButton");
    }); $('#3').click(function () {
        $("#Plate").val($("#3").val());
        $("input[class^='selectButton']").removeClass('selectButton');
        $("#3").addClass("selectButton");
    });
}
    