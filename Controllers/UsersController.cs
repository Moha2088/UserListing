using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserListing.Data;
using UserListing.Models;

namespace UserListing.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserListingContext _context;

        public UsersController(UserListingContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
           if(!_context.User.Any()) await GetUserData();
            return View(await _context.User.ToListAsync());
        }

        async Task GetUserData()
        {
            using var client = new HttpClient();
            var firstEndpoint = new Uri("https://reqres.in/api/users?page");
            var secondEndpoint = new Uri("https://reqres.in/api/users?page=2");

            var firstResponse = await client.GetAsync(firstEndpoint);
            var secondResponse = await client.GetAsync(secondEndpoint);

            firstResponse.EnsureSuccessStatusCode();
            secondResponse.EnsureSuccessStatusCode();
            
            var firstJsonResponse = await firstResponse.Content.ReadAsStringAsync();
            var secondJsonResponse = await secondResponse.Content.ReadAsStringAsync();
            
            var firstDeserializedObject = JsonSerializer.Deserialize<Rootobject>(firstJsonResponse);
            var secondDeserializedObject = JsonSerializer.Deserialize<Rootobject>(secondJsonResponse);

            if (firstDeserializedObject != null && secondDeserializedObject != null)
            {
                List<User> userList = firstDeserializedObject.data.ToList();
                userList.AddRange(secondDeserializedObject.data);
                userList.ForEach(user => user.Id = 0);
                await _context.User.AddRangeAsync(userList);
                await _context.SaveChangesAsync();
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,email,first_name,last_name,avatar")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,email,first_name,last_name,avatar")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
