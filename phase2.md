✅ Phase 2: Feature Enhancements & Advanced Capabilities

1. Tags & Many-to-Many Support
   • Implement tag model (Tag)
   • Many-to-many between Post and Tag via join table (e.g., PostTag)
   • Add tag filtering in GetAllPosts

2. Soft Delete
   • Add IsDeleted flag on Post, Comment, possibly User
   • Override SaveChangesAsync or apply global query filters

3. Post Metrics / Analytics
   • Track ViewCount, LikeCount, or similar
   • Consider storing IP/UserAgent/etc. for real analytics (optional)

4. Slug Uniqueness & URL Handling
   • Auto-generate slugs from title
   • Ensure uniqueness (add number suffix if duplicate)

5. Role-Based Authorization
   • Add roles (e.g., Admin, User)
   • Use [Authorize(Roles = "Admin")] where needed
   • Control access to editing/deleting posts by other users

6. Global Response Wrapper
   • Standardize all API responses with status, message, and data
   • Already started using helper like ErrorFormat.FormatErrorResponse

7. Centralized Logging
   • Integrate a logging library like Serilog
   • Log requests, responses, exceptions

8. Environment Configuration
   • Use appsettings.Development.json, appsettings.Production.json
   • Hide secrets like JWT secret in environment variables

9. Unit & Integration Testing
   • Test Services and Repositories
   • Mock using Moq or similar
   • Use xUnit or NUnit

10. External API Integration (Mini Feature)
    • Fetch metadata from a public API (e.g., news, quotes, GitHub) and associate it with a post
