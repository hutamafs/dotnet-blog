namespace BlogAPI.Repository;

using BlogAPI.Models;

public interface IUserRepository
{
  Task<bool> IsEmailTakenAsync(string email);
  Task<bool> IsUsernameTakenAsync(string username);
  Task<User?> GetByIdAsync(int id);
  Task<User?> GetByEmailAsync(string email);
  Task AddAsync(User user);
  Task SaveChangesAsync();
}