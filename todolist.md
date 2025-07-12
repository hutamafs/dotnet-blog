Testing List

🔍 Tests to Write (Phase 1 and 2)

🧑‍💻 UserService
• GetUserDetail (valid + invalid)
• RegisterUser (validation, duplicate email)
• UpdateSelfDetail (valid + forbidden update)
• Authentication (login - valid/invalid)

📝 PostService
• CreatePost (valid request, invalid request)
• GetPostById (with/without slug, with user include)
• GetAllPosts (pagination, filtering, only published)
• UpdatePost (owned + not owned)
• Soft delete or unpublish

💬 CommentService
• AddComment (valid + invalid + unauthorized)
• GetCommentsByPostId
• DeleteComment (if owner only)

👍 LikeService
• LikePost (if not liked)
• LikePost (already liked → 400)
• UnlikePost (not liked → 400)

🏷️ Tags + Categories
• Assign tags to post
• Get posts by tag
• Create/update/delete tag/category
