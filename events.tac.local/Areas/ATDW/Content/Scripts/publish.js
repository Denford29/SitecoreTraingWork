$(document).ready(function ()
{
    //hide the loading area 
    $("#loadingArea").hide();

    if ($("#autoPublsihForm"))
    {
        // trigger the form submit
        $("#autoPublsihForm").submit();

        // once we trigger the sibmit show the loading
        $("#loadingArea").show();
    }
});