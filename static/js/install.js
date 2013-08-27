$(document).ready(function () {
    
    var live = $("#Live");
    var formElements = $("form :text, form :password");

    live.prop('checked', true);

    formElements.each(function (i, e) {
        $(e).removeAttr('disabled');
    });

    $('#submit-server-configuration').click(function () { configuration.make("Install", "Server") });
    $('#submit-blog-configuration').click(function () { configuration.make("Install", "Blog") });
    $('#submit-user-configuration').click(function () { configuration.make("Install", "FirstUser") });
    $('#submit-user-login').click(function () { configuration.make("Auth", "Login") });

    live.on('click', function () {

        formElements.each(function (i, e) {           
            if ($(e).attr('disabled') == 'disabled') {
                $(e).removeAttr('disabled');
            }
            else {
                $(e).attr('disabled', 'disabled');
            }
        });
    });

    // DataAnnotation, za Html.CheckBoxFor kreira dodatni hidden field koji ima isti Name atribut kao i checkbox
    // pa prilikom slanja forme server dobija dodatno polje koje ne odgovara modelu, pa vraca Bad request.
    // Treba ga izbaciti iz DOM-a i ljepota...
    $("input[name=Live]:hidden").remove();
});