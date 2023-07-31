function handleValidation(errors) {

    $(".error").remove();
    $('.is-invalid').attr('class', 'form-control');

    for (var i = 0; i < (errors.length); i++) {
        if (errors[i].includes("Firstname")) {
            $('input#field_firstName').after('<label class="error">Invalid Name Format</label>');
            $('input#field_firstName').attr('class', 'form-control is-invalid');
        }
        else if (errors[i].includes("Lastname")) {
            $('input#field_lastName').after('<label class="error">Invalid Name Format</label>');
            $('input#field_lastName').attr('class', 'form-control is-invalid');
        }
        else if (errors[i].includes("Phone")) {
            $('input#field_phone').after('<label class="error">Invalid Phone Number</label>');
            $('input#field_phone').attr('class', 'form-control is-invalid');
        }
        else if (errors[i].includes("Balance")) {
            $('input#field_balance').after('<label class="error">Invalid Amount</label>');
            $('input#field_balance').attr('class', 'form-control is-invalid');
        }
        else if (errors[i].includes("Email")) {
            if (!errors[i].includes("Empty"))
                $('input#field_email').after('<label class="error">Invalid Email</label>');
            else
                $('input#field_email').after('<label class="error">Required Field</label>');

            $('input#field_email').attr('class', 'form-control is-invalid');
        }
    }
}

$(document).ready(function () {

    $('#customerFormSubmit').submit(function (e) {
        e.preventDefault();

        var $inputs = $('#customerFormSubmit :input');
        var values = {};
        $inputs.each(function () {
            values[this.name] = $(this).val();
        });

        $.ajax({
            type: "POST",
            url: "/Customer/Push/",
            data: {
                firstName: values["firstname"],
                lastName: values["lastname"],
                email: values["email"],
                phone_number: values["phone"],
                country_code: values["country"],
                gender: values["gender"],
                balance: values["balance"]
            },
            success: function (result) {
                if (typeof result["validation_errors"] !== 'undefined') {
                    console.log("Error Submitting Form");
                    handleValidation(result["validation_errors"]);
                }
                else {
                    console.log("Form Submitted!");
                    window.location = "/CustomerList/" + "?updated";
                }
            },
            error: function (request, error) {
                alert("Couldn't Create Customer. An Error has occurred. | " + error);
            }
        });
    });

});