namespace BlogAPI.Repository;

using BlogAPI.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository(AppDbContext context) : IUserRepository
{

  private readonly AppDbContext _context = context;
  async Task<bool> IUserRepository.IsEmailTakenAsync(string email)
  {
    return await _context.Users.AnyAsync(u => u.Email == email);
  }

  async Task<bool> IUserRepository.IsUsernameTakenAsync(string username)
  {
    return await _context.Users.AnyAsync(u => u.Username == username);
  }

  public async Task<User?> GetByEmailAsync(string email)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
  }

  async Task<User?> IUserRepository.GetByIdAsync(int id)
  {
    return await _context.Users.Include(u => u.Posts).FirstOrDefaultAsync(u => u.Id == id);
  }

  async Task IUserRepository.AddAsync(User user)
  {
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
  }

  async Task IUserRepository.SaveChangesAsync()
  {
    await _context.SaveChangesAsync();
  }
}