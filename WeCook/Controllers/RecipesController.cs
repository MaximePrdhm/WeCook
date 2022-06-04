using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeCook.Models.Database;
using WeCook.Models.Recipes;
using WeCook.Models.Recipes.Utils;

namespace WeCook.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly WeCookDbContext _context;
        private readonly IWebHostEnvironment _env;

        public RecipesController(WeCookDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Recipes
        public async Task<IActionResult> Index()
        {
              return _context.Recipes != null ? 
                          View(await _context.Recipes.ToListAsync()) :
                          Problem("Entity set 'WeCookDbContext.Recipes'  is null.");
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Creator)
                .Include(r => r.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);
            var ownerRecipeCount = _context.Recipes
                .Include(r => r.Creator)
                .Where(r => r.CreatorId == recipe.CreatorId)
                .Count();
            ViewBag.OwnerRecipeCount = ownerRecipeCount;
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [ValidateAntiForgeryToken]
        public JsonResult ProportionCalculation()
        {
            var recipeId = int.Parse(HttpContext.Request.Query["recipeId"].ToString());
            var wantedAmount = int.Parse(HttpContext.Request.Query["peopleFor"].ToString());

            var recipe = _context.Recipes
                .Include(r => r.Ingredients)
                .FirstOrDefaultAsync(m => m.Id == recipeId).Result;
            if (recipe == null)
                return Json(new List<Ingredient>());

            var calculator = new ProportionsCalculator(recipe);

            return Json(calculator.ComputeFor(wantedAmount));
        }

        [ValidateAntiForgeryToken]
        public JsonResult RecipeBookUpdate()
        {
            var recipeId = int.Parse(HttpContext.Request.Query["recipeId"].ToString());
            var userId = HttpContext.Request.Query["userId"].ToString();
            var action = HttpContext.Request.Query["action"].ToString();

            var tuple = new UserRecipe() { RecipeId = recipeId, UserId = userId };

            if(action == "like")
            {
                _context.Add(tuple);
            } else if(action == "dislike")
            {
                _context.Remove(tuple);
            }
            _context.SaveChanges();

            return Json("");
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Difficulty,Type,PeopleFor,PreparationTime,CookingTime,ImageFile,Ingredients,Steps")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                var fileName = $"${Guid.NewGuid()}{Path.GetExtension(recipe.ImageFile.FileName)}";
                var filePath = Path.Combine(_env.WebRootPath, "uploads", "recipes", fileName);
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await recipe.ImageFile.CopyToAsync(fileStream);
                }
                recipe.ImageName = fileName;
                
                recipe.CreatorId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                for(int i = 1; i <= recipe.Steps.Count; i++)
                {
                    recipe.Steps[i - 1].Position = i;
                }

                _context.Add(recipe);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            if(User.FindFirst(ClaimTypes.NameIdentifier).Value != recipe.CreatorId)
            {
                return RedirectToAction("Index", "Recipes");
            }

            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Difficulty,Type,PeopleFor,PreparationTime,CookingTime,ImageFile,ImageName,Ingredients,Steps")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            var target = _context.Recipes.Include(r => r.Creator).Where(r => r.Id == id).First();
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != target.CreatorId)
            {
                return RedirectToAction("Index", "Recipes");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(recipe.ImageFile != null)
                    {
                        System.IO.File.Delete(Path.Combine(_env.WebRootPath, "uploads", "recipes", recipe.ImageName));
                        var fileName = $"${Guid.NewGuid()}{Path.GetExtension(recipe.ImageFile.FileName)}";
                        var filePath = Path.Combine(_env.WebRootPath, "uploads", "recipes", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await recipe.ImageFile.CopyToAsync(fileStream);
                        }
                        recipe.ImageName = fileName;
                    }

                    for (int i = 1; i <= recipe.Steps.Count; i++)
                    {
                        recipe.Steps[i - 1].Position = i;
                    }

                    var missingIngredients = _context.Ingredients
                        .Where(i => i.RecipeId == id)
                        .Select(i => i.Id)
                        .ToList()
                        .Except(recipe.Ingredients.Select(i => i.Id).ToList())
                        .ToList()
                        .Select(id => _context.Ingredients.Where(i => i.Id == id).First())
                        .ToList();

                    var missingSteps = _context.Steps
                        .Where(s => s.RecipeId == id)
                        .Select(s => s.Id)
                        .ToList()
                        .Except(recipe.Steps.Select(s => s.Id).ToList())
                        .ToList()
                        .Select(id => _context.Steps.Where(s => s.Id == id).First())
                        .ToList();

                    _context.Ingredients.RemoveRange(missingIngredients);
                    _context.Steps.RemoveRange(missingSteps);
                    _context.Update(recipe);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Recipes == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Include(r => r.Creator)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != recipe.CreatorId)
            {
                return RedirectToAction("Index", "Recipes");
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Recipes == null)
            {
                return Problem("Entity set 'WeCookDbContext.Recipes'  is null.");
            }
            var recipe = _context.Recipes.Include(r => r.Creator).Where(r => r.Id == id).First();
            if (User.FindFirst(ClaimTypes.NameIdentifier).Value != recipe.CreatorId)
            {
                return RedirectToAction("Index", "Recipes");
            }
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }
            await _context.SaveChangesAsync();
            
            System.IO.File.Delete(Path.Combine(_env.WebRootPath, "uploads", "recipes", recipe.ImageName));

            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
          return (_context.Recipes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
