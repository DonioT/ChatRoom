$('.register-btn').click(function (e) {
    var userName = $('#registerUsername').val();
    var password = $('#registerPassword').val();
    var confirmPass = $('#registerConfirmPassword').val();
    if (userName.length == 0 || userName.length > 10) {
        $.growl.error({ title: "Invalid Username", message: "Please enter a username between 1 and 10 characters" });
        return false;
    } else if (password.length < 5) {
        $.growl.error({ title: "Invalid Password", message: "Please enter a password greater than 5 characters" });
    } else if (password != confirmPass) {
        $.growl.error({ title: "Passwords do not match", message: "Check your passwords" });
    }
    else {
        $.ajax({
            type: "GET",
            data: {
                username: userName
            },
            url: "/Home/UserExists"
        }).done(function (response) {
            if (response == "True") {
                $.growl.error({ title: "Username already exists", message: "Try a different username" });
            } else {
                $.ajax({
                    type: "POST",
                    url: "/Home/RegisterUser",
                    headers: { 'username': userName, 'password': $('#registerPassword').val() },
                    data: {
                        avatar: $('#RegisterUserIcon').attr('src')
                    }
                }).done(function (data) {
                    $.growl.notice({ title: "Registration successful", message: "Logging in...." });
                  
                    window.location.replace(data);
                });
            }
        });
    }
});


$('#RegisterUserIcon').click(function () {
    $('#RegisterUserIcon').popModal({
        placement: "bottomCenter",
        html: $('#imageContent').html()
    });
});

function replaceImageSrc(element, targetElement) {
    $(targetElement).attr('src', $(element).attr('src'));
}

$(document).on('click', '.imageUserIconPreview', function () {
    replaceImageSrc(this, '#RegisterUserIcon');
    //$('#RegisterUserIcon').attr('src', $(this).attr('src'));
});




$('.login-btn').click(function (e) {
    $.ajax({
        type: "POST",
        url: "/Home/Login",
        headers: { 'username': $('#username').val(), 'password': $('#password').val() },
    }).done(function (data) {
        if (data == "/") {
            $.growl.error({ title: "Problem logging in", message: "Check your login details" });
        } else {
            $.growl.notice({ title: "Login successful", message: "Logging in...." });
        }
        window.location.replace(data);

    });
});





