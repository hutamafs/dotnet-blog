namespace BlogAPI.Repository;

using BlogAPI.DTOs;
using BlogAPI.Models;

public interface IPostRepository
{
  Task<IEnumerable<Post>> GetAllPosts();
  Task<Post?> GetByIdAsync(int id);
  Task<bool> IsUserIdExist(int id);
  Task<bool> IsCategoryIdExist(int id);
  Task CreatePostAsync(Post post);
  Task SaveChangesAsync();
  Task<Post> UpdatePost(int id, Post post);
  Task<bool> DeletePost(Post post);
}