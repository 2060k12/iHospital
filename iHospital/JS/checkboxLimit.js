
// checkboxLimit.js

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




