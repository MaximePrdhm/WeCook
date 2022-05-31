let peopleCount = parseInt($('#NbrPeople').text());

document.querySelector('#btn-add-people').addEventListener('click', (e) => {
    e.preventDefault();

    $('#NbrPeople').text(++peopleCount);
    requestProportions();
});

document.querySelector('#btn-rmv-people').addEventListener('click', (e) => {
    e.preventDefault();

    if (peopleCount > 1) {
        $('#NbrPeople').text(--peopleCount);
        requestProportions();
    }
});

function requestProportions() {
    $.ajax({
        headers: { RequestVerificationToken: $('#RequestCsrfToken').val() },
        dataType: 'json',
        url: "/Recipes/ProportionCalculation",
        data: { recipeId: $('#Id').val(), peopleFor: peopleCount },
        success: function (data) { replaceQuantities(data) }
    })
}

function replaceQuantities(data) {
    data.forEach(ingr => {
        $(`#IngredientQuantity${ingr.id}`).text(ingr.quantity);
    });
}