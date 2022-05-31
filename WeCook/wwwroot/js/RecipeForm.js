let ingredientsCount = parseInt($('#IngredientsCount').val());
let stepsCount = parseInt($('#StepsCount').val());

document.querySelector('#btn-add-ingredient').addEventListener('click', (e) => {
    e.preventDefault();

    let newIngredient = generateNewIngredient();
    $('#IngredientsContainer').append(newIngredient);

    let targetSelect = document.getElementsByClassName('IngredientUnitSelect' + ingredientsCount)[0];
    let referrerSelect = document.getElementsByClassName('IngredientUnitReferrer')[0];
    targetSelect.innerHTML += referrerSelect.innerHTML;

    ingredientsCount++;
});

document.querySelector('#btn-rmv-ingredient').addEventListener('click', (e) => {
    e.preventDefault();

    if (ingredientsCount > 1) {
        let coll = document.getElementsByClassName('Ingredient');
        coll[coll.length - 1].remove();
        ingredientsCount--;
    }
});

document.querySelector('#btn-add-step').addEventListener('click', (e) => {
    e.preventDefault();

    let newStep = generateNewStep();
    $('#StepsContainer').append(newStep);

    stepsCount++;
});

document.querySelector('#btn-rmv-step').addEventListener('click', (e) => {
    e.preventDefault();

    if (stepsCount > 1) {
        let coll = document.getElementsByClassName('Step');
        coll[coll.length - 1].remove();
        stepsCount--;
    }
});

let generateNewIngredient = () => {
    return `<div class="Ingredient col-12 d-flex flex-row justify-content-between">
                <div class="form-group col-8 me-1">
                    <label class="control-label" for="Ingredients_${ingredientsCount}__Name">Nom</label>
                    <input class="form-control" type="text" data-val="true" data-val-maxlength="The field Nom must be a string or array type with a maximum length of &#x27;100&#x27;." data-val-maxlength-max="100" data-val-required="Champ requis" id="Ingredients_${ingredientsCount}__Name" maxlength="100" name="Ingredients[${ingredientsCount}].Name" value="" />
                    <span class="text-danger field-validation-valid" data-valmsg-for="Ingredients[${ingredientsCount}].Name" data-valmsg-replace="true"></span>
                </div>
                <div class="flex-grow-1 d-flex flex-row justify-content-end">
                    <div class="form-group flex-grow-1">
                        <label class="control-label" for="Ingredients_${ingredientsCount}__Quantity">Quantit&#xE9;</label>
                        <input class="form-control" type="text" data-val="true" data-val-number="The field Quantit&#xE9; must be a number." data-val-range="Nombre positif requis" data-val-range-max="3.4028234663852886E&#x2B;38" data-val-range-min="0.1" data-val-required="Champ requis" id="Ingredients_${ingredientsCount}__Quantity" name="Ingredients[${ingredientsCount}].Quantity" value=1 />
                        <span class="text-danger field-validation-valid" data-valmsg-for="Ingredients[${ingredientsCount}].Quantity" data-valmsg-replace="true"></span>
                    </div>
                    <div class="form-group col-6 ms-1">
                        <label class="control-label" for="Ingredients_${ingredientsCount}__Unit">Unit&#xE9;</label>
                        <select class="form-control ps-2 IngredientUnitSelect${ingredientsCount}" data-val="true" data-val-required="The Unit&#xE9; field is required." id="Ingredients_${ingredientsCount}__Unit" name="Ingredients[${ingredientsCount}].Unit">
                        
                        </select>
                        <span class="text-danger field-validation-valid" data-valmsg-for="Ingredients[0].Unit" data-valmsg-replace="true"></span>
                    </div>
                </div>
            </div>`;
}

let generateNewStep = () => {
    return `<div class="Step col-12">
                <div class="col-12">Étape ${stepsCount + 1}</div>
                <div class="form-group">
                    <textarea class="form-control" rows="1" data-val="true" data-val-required="Champ requis" name="Steps[${stepsCount}].Description" id="Steps_${stepsCount}__Description"></textarea>
                    <span class="text-danger field-validation-valid" data-valmsg-for="Steps[${stepsCount}].Description" data-valmsg-replace="true"></span>
                </div>
            </div>`;
}