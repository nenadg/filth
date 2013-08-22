// configuration proto
var configuration = (function () {
    var delta = 0;

    /// ajax to controller/action
    /// @param {String} [controller] name of api controller
    /// @param {String} [action] controller action to invoke
    var make = function (controller, action) {
        var form = $('form'),
            enabled = ($('#Live').prop('checked') !== undefined) ? $('#Live').prop('checked') : true; // lame ...
        
        // related only to server configuration form thus stupid but hey ...
        var serialized = (enabled) ? form.serializeArray() : { ServerName: "local", Catalog: "local", Username: "local" };

        $.ajax({
            type: "POST",
            url: "/api/" + controller + "/" + action,
            data: serialized
        }).done(function () {
            // later ...

        }).fail(function (xhr) {
            
            // server returns location header which
            // replaces Response.Redirect(location)
            var loc = xhr.getResponseHeader("Location");

            if (loc !== null) {
                progress();
                setTimeout(function () { window.location.href = "http://" + loc; }, 350);
                $('body').fadeOut(340); // fancy
            } else {

                // modelStateErros - validation
                // exceptionMessage - if something goes wrong
                var modelStateErrors = xhr.responseJSON.ModelState;
                var exceptionMessage = xhr.responseJSON.ExceptionMessage;
                
                if (exceptionMessage !== undefined) {
                    delta++;
                    var errorTime = new Date().getHours() + ":" + new Date().getMinutes() + ":" + new Date().getSeconds();
                    
                    $('.comments.error').append("<p>[" + errorTime + "] " + exceptionMessage + "</p>");
                    if (delta > 5) $('.comments.error').append("<p><strong>" + messages[Math.floor(Math.random() * messages.length)] + "</strong></p>");
                }

                if (enabled)
                    validate(form, modelStateErrors);
            }
        });
    };

    /// validate form against modelstate errors
    /// @param {Object} [form] name of form to validate against
    /// @param {Object} [modelStateErrors] collection of modelstate errors returned by server
    function validate(form, modelStateErrors) {

        for (var prop in modelStateErrors) {

            var spanId = prop.split('.');

            var spanInForm = $("span[data-valmsg-for='" + spanId[1] + "']");
            var reqInput = $("#" + spanId[1]);

            if (reqInput.val() == "") {
                spanInForm.text(modelStateErrors[prop]);
                spanInForm.addClass("field-validation-error").removeClass("field-validation-valid");
                reqInput.addClass("input-validation-error");
            }
        }

        form.find('.input-validation-error').each(function (i, e) {

            var spanInForm = $("span[data-valmsg-for='" + e.id + "']");
            var reqInput = $("#" + e.id);

            if (reqInput.val() != "") {
                spanInForm.addClass("field-validation-valid").removeClass("field-validation-error");
                reqInput.removeClass("input-validation-error");
                spanInForm.text("");
            }
        });
    }

    // eggs 
    var messages = ['Come on', 'Stop it', 'Much fun having here', 'Crazy...', 'It seems that you have some problem', 'Just die.', 'Maybe you should contact a psychiatrist',
                    'You\'re so wrong','An error in your head occured', 'Bullshit bingo', 'Weeee...', 'Off with your head'];

    // display simple progress as we move on
    function progress() {
        var full = $(window).width();
         $('.progress').animate({ width: full }, 290);   
    }

    return {
        make: make,
        validate: validate
    }
})();

