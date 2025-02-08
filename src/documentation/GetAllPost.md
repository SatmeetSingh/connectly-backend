Good question! Yes, calling `.CountAsync()` separately does make two queries, which can impact performance. To optimize it, you have a few options:  

---

### ✅ **Option 1: Use `ToListAsync()` First (Single Query)**
Instead of making two separate queries, fetch all data **once**, then process in-memory:
```csharp
var allPosts = await _context.Posts
    .OrderByDescending(p => p.CreatedDate)
    .AsNoTracking()
    .ToListAsync();  // Fetch everything once

int totalCount = allPosts.Count;  // Get total count in-memory

var posts = allPosts
    .Skip((Page - 1) * limit)
    .Take(limit)
    .ToList();  // Apply pagination in-memory

return Ok(new 
{
    TotalCount = totalCount,
    Posts = posts
});
```
🔹 **Pros:** ✅ **Single Query** → Reduces DB load.  
🔹 **Cons:** ❌ If there are millions of posts, fetching everything at once may use too much memory.

---

### ✅ **Option 2: Use `IQueryable` with `Count()` and `ToListAsync()` (Efficient)**
Instead of querying all posts, **let SQL Server optimize both queries**:
```csharp
var query = _context.Posts.AsNoTracking();

var totalCountTask = query.CountAsync();  // Query 1 (Total Count)
var postsTask = query.OrderByDescending(p => p.CreatedDate)
                     .Skip((Page - 1) * limit)
                     .Take(limit)
                     .ToListAsync();  // Query 2 (Paginated Data)

await Task.WhenAll(totalCountTask, postsTask);  // Execute both queries in parallel

return Ok(new 
{
    TotalCount = totalCountTask.Result,  // Get total count
    Posts = postsTask.Result  // Get paginated posts
});
```
🔹 **Pros:** ✅ **Both queries run in parallel**, reducing overall response time.  
🔹 **Cons:** ❌ Still two queries, but more efficient than separate calls.

---

### ✅ **Option 3: Use `SQL ROW_NUMBER()` (Best for Large Data)**
If performance is critical, you can use a **raw SQL query with `ROW_NUMBER()`**:
```csharp
var result = await _context.Posts
    .FromSqlRaw(@"
        WITH CTE AS (
            SELECT *, ROW_NUMBER() OVER (ORDER BY CreatedDate DESC) AS RowNum
            FROM Posts
        )
        SELECT * FROM CTE WHERE RowNum BETWEEN {0} AND {1}", 
        (Page - 1) * limit + 1, Page * limit)
    .AsNoTracking()
    .ToListAsync();

int totalCount = await _context.Posts.CountAsync();  // Separate but lightweight query

return Ok(new { TotalCount = totalCount, Posts = result });
```
🔹 **Pros:** ✅ **Optimized SQL execution** for large datasets.  
🔹 **Cons:** ❌ Slightly complex query.

---

### 🔥 **Which One Should You Use?**
1. **If you have < 100,000 records**, use **Option 2** (`Task.WhenAll()`).  
2. **If you have millions of records**, use **Option 3** (SQL `ROW_NUMBER()`).  
3. **If you are okay with more memory usage**, use **Option 1** (fetch all and filter in-memory).  

Would you like further optimizations, like **caching** for repeated queries? 🚀