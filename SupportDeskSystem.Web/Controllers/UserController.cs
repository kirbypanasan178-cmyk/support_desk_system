using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportDeskSystem.Web.Enums;
using SupportDeskSystem.Web.Services;

namespace SupportDeskSystem.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: /User
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();

            return View(users);
        }

        // GET: /User/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            string fullName,
            string email,
            string password,
            UserRole role)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            await _userService.CreateUserAsync(
                fullName,
                email,
                password,
                role);

            return RedirectToAction(nameof(Index));
        }

        // GET: /User/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: /User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            string fullName,
            string email,
            UserRole role,
            bool isActive)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }

            var updated = await _userService.UpdateUserAsync(
                id,
                fullName,
                email,
                role);

            if (!updated)
            {
                return NotFound();
            }

            await _userService.SetActiveStatusAsync(
                id,
                isActive);

            return RedirectToAction(nameof(Index));
        }

        // POST: /User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteUserAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /User/SetActiveStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetActiveStatus(
            int id,
            bool isActive)
        {
            var updated = await _userService.SetActiveStatusAsync(
                id,
                isActive);

            if (!updated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}