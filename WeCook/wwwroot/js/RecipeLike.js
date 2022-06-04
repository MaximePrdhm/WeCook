let userId = parseInt($('#LogonUserId').val());
let recipeId = parseInt($('#Id').val());

let notLikedRecipeContent = `<button class="mb-4 w-100 btn btn-info" id="like-recipe-btn"><i class="fa-solid fa-heart me-2"></i>Ajouter à mon carnet</button>`;
let likedRecipeContent = `<div class="w-100 mb-2 text-center">Vous aimez déjà cette recette.</div><button class="mb-4 w-100 btn btn-info" id="dislike-recipe-btn"><i class="fa-solid fa-xmark me-2"></i>Retirer de mon carnet</button>`;

let container = document.getElementById('like-btn-container');

let button = document.querySelector('#like-btn-container button');
if (button) {
    if (button.id === 'like-recipe-btn') {
        setupLikeButton();
    } else if (button.id === 'dislike-recipe-btn') {
        setupDislikeButton();
    }
}

function setupLikeButton() {
    document.querySelector('#like-recipe-btn').addEventListener('click', (e) => {
        e.preventDefault();

        $.ajax({
            headers: { RequestVerificationToken: $('#RequestCsrfToken').val() },
            dataType: 'json',
            url: "/Recipes/RecipeBookUpdate",
            data: { recipeId: $('#Id').val(), userId: $('#LogonUserId').val(), action: "like" },
            success: function (data) {
                container.innerHTML = likedRecipeContent;
                setupDislikeButton();
            }
        });
    });
}

function setupDislikeButton() {
    document.querySelector('#dislike-recipe-btn').addEventListener('click', (e) => {
        e.preventDefault();

        $.ajax({
            headers: { RequestVerificationToken: $('#RequestCsrfToken').val() },
            dataType: 'json',
            url: "/Recipes/RecipeBookUpdate",
            data: { recipeId: $('#Id').val(), userId: $('#LogonUserId').val(), action: "dislike" },
            success: function (data) {
                container.innerHTML = notLikedRecipeContent;
                setupLikeButton();
            }
        });
    });
} 

