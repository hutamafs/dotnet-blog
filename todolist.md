🔜 Next Priority

Let’s go in this order:

1. Authentication
   • 🔒 Login (POST /api/auth/login)
   • ✅ You already hash passwords — now verify them and return a JWT.
   • Create AuthController, LoginRequestDto, JwtService.

2. Authorization
   • Restrict protected routes (e.g. POST /posts, PATCH /posts/{id}) so only logged-in users can access them.
   • Optional: only the owner of a post can edit/delete it.

3. User Profile
   • GET /api/users/me – Get profile from token
   • PATCH /api/users/me – Update own profile (bio, name, etc.)

4. Categories & Tags
   • CRUD categories (admin-only if roles exist)
   • Associate multiple tags per post (many-to-many)
   • Include tags in Post detail

5. Comments
   • Allow authenticated users to:
   • Create comment on post
   • Get comments for a post
   • Optional: update/delete own comment
