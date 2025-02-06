

## 🔹 **Enhancements & Changes to Existing APIs**
Here are some refinements and changes you should consider:

### **1️⃣ Comments API Changes**
- **Current Issue**: The `DELETE /api/Comments/{userId}/delete/{postId}` does not specify the comment itself. If a post has multiple comments from the same user, how do you determine which to delete?
- **Fix**: Modify the endpoint to include the **comment ID**.
  - ✅ **New Endpoint**:  
    ```
    DELETE /api/Comments/{userId}/delete/{commentId}
    ```
  - **Alternative:** Only allow a user to delete **their own comment** and an admin/moderator to delete **any comment**.

---

### **2️⃣ Likes API Enhancements**
- **Current Issue**: `GET /api/Likes/{postId}/likes/{userId}` seems redundant.
- **Fix**: Make it more intuitive.
  - ✅ **New Endpoint**:
    ```
    GET /api/Likes/{postId}/user/{userId}
    ```
  - ✅ **New Feature**: Allow **users to see who liked a post**:
    ```
    GET /api/Likes/{postId}/users
    ```
    - This will return a list of users who liked the post.

- ✅ **New Feature**: Allow **users to see posts they liked**:
  ```
  GET /api/Likes/user/{userId}
  ```
  - Returns a list of posts that the user has liked.

---

### **3️⃣ Posts API Enhancements**
- ✅ **Add Pagination for Post Listing**:
  ```
  GET /api/Posts?page={pageNumber}&limit={limit}
  ```
  - Returns paginated results instead of loading all posts at once.

- ✅ **Add Post Visibility (Public, Private, Friends-only)**:
  - Modify the `POST /api/Posts` endpoint to accept a `visibility` field:
    ```json
    {
      "content": "This is a new post",
      "visibility": "public"   // Options: "public", "private", "friends"
    }
    ```
  - Update `GET /api/Posts` to **only show posts the user has permission to see**.

---

### **4️⃣ Users API Enhancements**
- ✅ **Add "Get User Profile" API**
  ```
  GET /api/Users/{id}/profile
  ```
  - Returns detailed user profile with:
    - Name, bio, profile picture, post count, follower count, etc.

- ✅ **Add Profile Picture & Cover Photo Upload**
  ```
  PATCH /api/Users/{id}/uploadProfilePicture
  PATCH /api/Users/{id}/uploadCoverPhoto
  ```
  - Users can update their profile/cover picture.

- ✅ **Add "Change Password" API**
  ```
  PATCH /api/Users/{id}/changePassword
  ```
  - Users can change their password securely.

- ✅ **Add "Delete Account" API (Soft Delete)**
  ```
  PATCH /api/Users/{id}/deactivate
  ```
  - Instead of **permanently deleting**, deactivate user account.

---

## 🔹 **New Features & APIs**
Here are some new APIs that can enhance the social media experience:

### **5️⃣ Friendships (Follow/Unfollow)**
Users need a way to **follow/unfollow** others.

✅ **Endpoints**:
```
POST   /api/Users/{userId}/follow/{targetUserId}
DELETE /api/Users/{userId}/unfollow/{targetUserId}
GET    /api/Users/{userId}/followers
GET    /api/Users/{userId}/following
```
- Allows users to **follow** or **unfollow** other users.
- Get lists of **followers** and **following users**.

---

### **6️⃣ Direct Messaging (Chat)**
A social media app should have **user-to-user chat**.

✅ **Endpoints**:
```
POST   /api/Messages/{senderId}/send/{receiverId}
GET    /api/Messages/{userId}/chats
GET    /api/Messages/{userId}/chat/{receiverId}
DELETE /api/Messages/{messageId}/delete
```
- Users can send **direct messages** to each other.
- View **chat history**.
- Delete messages.

---

### **7️⃣ Notifications System**
Users should receive notifications for events like:
- **New Followers**
- **New Likes/Comments on their Post**
- **Mentions in Comments/Posts**
- **Friend Requests**

✅ **Endpoints**:
```
GET    /api/Notifications/{userId}
POST   /api/Notifications/{userId}/markAsRead/{notificationId}
DELETE /api/Notifications/{notificationId}/delete
```
- Fetch **all unread notifications**.
- Mark notifications as **read**.
- Delete old notifications.

---

### **8️⃣ Search Feature**
Allow users to **search** for:
- Users
- Posts (by hashtag or text)
- Comments

✅ **Endpoints**:
```
GET /api/Search/users?query={searchTerm}
GET /api/Search/posts?query={searchTerm}
GET /api/Search/hashtags?query={hashtag}
```
- Search users by **name** or **username**.
- Search posts using **keywords**.
- Search posts by **hashtags**.

---

### **9️⃣ Hashtags & Trending Posts**
Enable **hashtags** for posts.

✅ **Endpoints**:
```
GET /api/Hashtags/trending
GET /api/Hashtags/{hashtag}/posts
```
- Show **trending hashtags**.
- Fetch **all posts** under a specific hashtag.

---

### **🔟 Reporting & Moderation**
To keep the platform safe, users should be able to **report** inappropriate content.

✅ **Endpoints**:
```
POST   /api/Reports/posts/{postId}/report
POST   /api/Reports/comments/{commentId}/report
POST   /api/Reports/users/{userId}/report
GET    /api/Reports
DELETE /api/Reports/{reportId}/resolve
```
- Allows users to report **posts**, **comments**, or **users**.
- Admins can **review** and **resolve reports**.

---

## 🔹 **Security Enhancements**
Since this is a **social media platform**, security is critical.

### **1️⃣ Authentication (JWT Tokens)**
✅ **Use JSON Web Tokens (JWT)** for authentication.
- Return a **JWT token** when users **login/signup**.
- Require this token in **all API requests**.

### **2️⃣ Rate Limiting (Prevent Spam)**
✅ **Limit API requests per minute** to prevent **spam**.

- Example:  
  ```
  Max 10 requests per minute per user
  ```

### **3️⃣ User Roles (Admin & Normal Users)**
✅ **Admin APIs** to manage the platform:
```
GET    /api/Admin/users
PATCH  /api/Admin/users/{id}/ban
DELETE /api/Admin/users/{id}/delete
```
- Admins can **ban** or **delete** users.

---

## 🔹 **Final Summary**
Here’s what you should add to make the app **complete**:

| Feature             | API Needed? |
|---------------------|------------|
| **Follow/Unfollow System** | ✅ |
| **Direct Messaging (Chat)** | ✅ |
| **Notifications System** | ✅ |
| **User Search (Users, Posts, Hashtags)** | ✅ |
| **Hashtags & Trending Posts** | ✅ |
| **Post Visibility Settings** | ✅ |
| **User Profile Picture & Cover Photo Upload** | ✅ |
| **Soft Delete (Deactivate Account)** | ✅ |
| **Reporting System (Moderation)** | ✅ |
| **Admin Controls (Ban/Delete Users, Handle Reports)** | ✅ |
| **Security (JWT, Rate Limiting, Roles)** | ✅ |

---
### **🔹 Next Steps**
✅ **Implement these new APIs**  
✅ **Use JWT Authentication**  
✅ **Secure your APIs with role-based access control (RBAC)**  
✅ **Optimize API responses with pagination**  

Would you like me to help with the database schema or C# implementation for any of these features? 🚀 🚀