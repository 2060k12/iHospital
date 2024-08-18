document.addEventListener('DOMContentLoaded', function () {
    var checkBoxList = document.getElementById('optionsCheckBoxList');
    var maxSelections = parseInt(document.getElementById('maxSelectionsHiddenField').value);

    checkBoxList.addEventListener('change', function (event) {
        var checkedCheckboxes = checkBoxList.querySelectorAll('input[type=checkbox]:checked');
        if (checkedCheckboxes.length > maxSelections) {
            event.preventDefault();
            event.stopImmediatePropagation();
            alert('You can select a maximum of ' + maxSelections + ' options.');
            // Uncheck the last checked checkbox
            event.target.checked = false;
        }
    });
});




// emailValidation.js
function validateEmail(email) {
    // Regular expression for basic email validation
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

function onEmailInputChange(event) {
    const emailInput = event.target;
    const isValid = validateEmail(emailInput.value);
    if (isValid) {
        emailInput.style.borderColor = '#28a745'; // Green for valid
        emailInput.setCustomValidity(''); // Clear any previous error message
    } else {
        emailInput.style.borderColor = '#dc3545'; // Red for invalid
        emailInput.setCustomValidity('Please enter a valid email address.'); // Set custom error message
    }
}

document.addEventListener('DOMContentLoaded', function () {
    // Use the ClientID of the TextBox to reference it
    const emailInput = document.getElementById('<%= inputTextBox.ClientID %>');
    if (emailInput) {
        emailInput.addEventListener('input', onEmailInputChange);
    }
});