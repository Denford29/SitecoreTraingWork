﻿$(document).ready(function() {

    $("li.dropdown").on("click",
        function() {
            var listItem = $(this);
            if (listItem.hasClass("open")) {
                var toogleLink = listItem.children("a.dropdown-toggle");
                if (toogleLink.length && toogleLink.attr("href")) {
                    location.href = toogleLink.attr("href");
                }
            }
        });

});

    function switchToLanguage(newLanguage, currentLanguage, redirectUrl) {
        var ajaxPost = function(responseFunction, sender) {
            //var token = $("#_CRSFform input[name=__RequestVerificationToken]").val();
            ajax({
                type: "POST",
                url: "/api/feature/language/changelanguage",
                cache: false,
                //headers: { "__RequestVerificationToken": token },
                contentType: "application/json; charset=utf-8",
                data: "{\"NewLanguage\":\"" + newLanguage + "\", \"CurrentLanguage\":\"" + currentLanguage + "\"}",
                success: function(data) {
                    if (responseFunction != null) {
                        responseFunction(data, true, sender);
                    }
                },
                error: function(data) {
                    if (responseFunction != null) {
                        responseFunction(data, false, sender);
                    }
                }
            });
        };

        ajaxPost(function(data, success, sender) {
            window.location = redirectUrl;
        });
        return false;
    }

