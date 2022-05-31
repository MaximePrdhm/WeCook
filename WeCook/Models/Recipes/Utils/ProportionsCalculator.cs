namespace WeCook.Models.Recipes.Utils
{
    public class ProportionsCalculator
    {
        private IList<Ingredient> _ingredients;
        private int _initialPeopleNumber;

        private IDictionary<IngredientUnit, string> _units;

        public ProportionsCalculator(Recipe recipe)
        {
            _ingredients = recipe.Ingredients;
            _initialPeopleNumber = recipe.PeopleFor;

            _units = new Dictionary<IngredientUnit, string>()
            {
                { IngredientUnit.None, ""},
                { IngredientUnit.Unit, ""},
                { IngredientUnit.Gram, "G"},
                { IngredientUnit.Kilogram, "Kg"},
                { IngredientUnit.Milliliter, "Ml"},
                { IngredientUnit.Centiliter, "Cl"},
                { IngredientUnit.Liter, "L"},
                { IngredientUnit.Teaspoon, " Càc"},
                { IngredientUnit.Tablespoon, " Càs"},
                { IngredientUnit.Pinch, " Pincée"},
            };
        }

        public IList<Ingredient> ComputeFor(int nbPeople)
        {
            var newIngredients = new List<Ingredient>();

            foreach(var ingredient in _ingredients)
            {
                newIngredients.Add(new Ingredient { Id = ingredient.Id, Unit = ingredient.Unit, Quantity = GetNewQuantity(ingredient, nbPeople), DisplayUnit = ingredient.DisplayUnit});
            }

            return newIngredients;
        }

        private float GetNewQuantity(Ingredient ingredient, int nbPeople)
        {
            var unit = ingredient.Unit;
            if (unit == IngredientUnit.None || unit == IngredientUnit.Pinch)
                return ingredient.Quantity;

            var val = (ingredient.Quantity / _initialPeopleNumber) * nbPeople;
            if (unit == IngredientUnit.Unit || unit == IngredientUnit.Tablespoon || unit == IngredientUnit.Teaspoon)
                return (float)Math.Round(val * 2, MidpointRounding.AwayFromZero) / 2;
            return val;
        }
    }
}
