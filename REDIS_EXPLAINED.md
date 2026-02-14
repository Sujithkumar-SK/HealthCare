# Redis Complete Guide - Technical & Business Perspective

## 📚 Table of Contents
1. What is Redis?
2. How Redis Works (Technical)
3. Business Use Cases
4. Redis vs PostgreSQL
5. Installation & Setup
6. Code Examples
7. Best Practices

---

## 1️⃣ What is Redis?

### Simple Definition
**Redis = Remote Dictionary Server**

Think of Redis as a **super-fast notepad in RAM** where you can store and retrieve data instantly.

### Key Characteristics
- **In-Memory Database** - Stores data in RAM (not disk like PostgreSQL)
- **Key-Value Store** - Like a dictionary: `{ "user:123": "John Doe" }`
- **Extremely Fast** - Microsecond response time (1000x faster than disk)
- **Temporary Storage** - Data can expire automatically
- **Optional Persistence** - Can save to disk if needed

---

## 2️⃣ How Redis Works (Technical)

### Architecture

```
┌─────────────────────────────────────┐
│         Your Application            │
│      (.NET API / Angular)           │
└──────────────┬──────────────────────┘
               │
               ↓
┌──────────────────────────────────────┐
│           Redis Server               │
│         (Port: 6379)                 │
│  ┌────────────────────────────┐     │
│  │      RAM (Memory)          │     │
│  │  Key: "patient:123"        │     │
│  │  Value: { name: "John" }   │     │
│  └────────────────────────────┘     │
└──────────────────────────────────────┘
               │ (Optional)
               ↓
┌──────────────────────────────────────┐
│         Disk (Persistence)           │
│      dump.rdb / appendonly.aof       │
└──────────────────────────────────────┘
```

### Data Storage Model

```
Redis stores data as KEY → VALUE pairs:

Key                    Value
─────────────────────  ──────────────────────────────
"patient:123"       →  "{ name: 'John', age: 30 }"
"survey:456"        →  "{ answers: [...] }"
"session:abc"       →  "{ userId: 123, token: '...' }"
```

### Data Types Redis Supports

1. **String** - Simple text or JSON
2. **Hash** - Object with fields
3. **List** - Array of values
4. **Set** - Unique values
5. **Sorted Set** - Ordered values
6. **JSON** - Native JSON support (with RedisJSON module)

---

## 3️⃣ Business Use Cases

### Why Use Redis in Your Health Intake App?

#### **Use Case 1: Caching Patient Data**
**Problem**: Database queries are slow (50-100ms)  
**Solution**: Cache in Redis (1-2ms)

**Business Impact**:
- ✅ Faster page loads → Better user experience
- ✅ Reduced database load → Lower costs
- ✅ Handle more users → Scalability

**Example Flow**:
```
User requests patient data
    ↓
Check Redis first (FAST)
    ↓
If found → Return immediately (1ms)
    ↓
If not found → Query PostgreSQL (50ms)
    ↓
Store in Redis for next time
    ↓
Return to user
```

#### **Use Case 2: Session Management**
**Problem**: Track user's form progress across pages  
**Solution**: Store session in Redis with auto-expiry

**Business Impact**:
- ✅ Users can resume incomplete forms
- ✅ Automatic cleanup of old sessions
- ✅ No database clutter

#### **Use Case 3: Rate Limiting**
**Problem**: Prevent spam/abuse of API  
**Solution**: Track API calls per user in Redis

**Business Impact**:
- ✅ Protect against DDoS attacks
- ✅ Fair usage enforcement
- ✅ Cost control

#### **Use Case 4: Real-time Analytics**
**Problem**: Count active users, form submissions  
**Solution**: Increment counters in Redis

**Business Impact**:
- ✅ Live dashboard metrics
- ✅ No database overhead
- ✅ Instant insights

---

## 4️⃣ Redis vs PostgreSQL

| Feature | PostgreSQL | Redis |
|---------|-----------|-------|
| **Storage** | Disk (SSD/HDD) | RAM (Memory) |
| **Speed** | 50-100ms | 1-2ms |
| **Data Size** | Terabytes | Gigabytes |
| **Persistence** | Permanent | Temporary (optional save) |
| **Use Case** | Long-term storage | Temporary/Cache |
| **Cost** | Cheaper per GB | Expensive per GB |
| **Queries** | Complex SQL | Simple key lookup |

### When to Use What?

```
PostgreSQL (Permanent Storage):
✅ Patient records
✅ Survey responses
✅ Audit logs
✅ Historical data

Redis (Temporary/Cache):
✅ Session data
✅ Cached queries
✅ Real-time counters
✅ Temporary tokens
```

---

## 5️⃣ Installation & Setup

### Windows Installation

#### **Option 1: Download Redis for Windows**
1. Download: https://github.com/microsoftarchive/redis/releases
2. Download `Redis-x64-3.0.504.msi`
3. Install (default port: 6379)
4. Redis runs as Windows Service

#### **Option 2: Docker (Recommended)**
```bash
docker run -d -p 6379:6379 --name redis redis:latest
```

#### **Option 3: WSL (Windows Subsystem for Linux)**
```bash
wsl
sudo apt update
sudo apt install redis-server
redis-server
```

### Verify Installation
```bash
redis-cli ping
# Should return: PONG
```

### Redis GUI Tools (Optional)
- **RedisInsight** - https://redis.com/redis-enterprise/redis-insight/
- **Another Redis Desktop Manager** - Free GUI client

---

## 6️⃣ Code Examples

### Example 1: Basic Redis Operations (C#)

```csharp
using StackExchange.Redis;

// Connect to Redis
var redis = ConnectionMultiplexer.Connect("localhost:6379");
var db = redis.GetDatabase();

// SET - Store data
db.StringSet("patient:123", "John Doe");

// GET - Retrieve data
string name = db.StringGet("patient:123");
Console.WriteLine(name); // Output: John Doe

// SET with expiry (auto-delete after 1 hour)
db.StringSet("session:abc", "user-data", TimeSpan.FromHours(1));

// DELETE
db.KeyDelete("patient:123");

// CHECK if key exists
bool exists = db.KeyExists("patient:123");
```

### Example 2: Storing JSON Objects

```csharp
using System.Text.Json;

// Store patient object
var patient = new { 
    Id = 123, 
    Name = "John Doe", 
    Email = "john@example.com" 
};

string json = JsonSerializer.Serialize(patient);
db.StringSet("patient:123", json);

// Retrieve patient object
string storedJson = db.StringGet("patient:123");
var retrievedPatient = JsonSerializer.Deserialize<dynamic>(storedJson);
```

### Example 3: Caching Pattern (Your Use Case)

```csharp
public async Task<Patient> GetPatientAsync(Guid patientId)
{
    string cacheKey = $"patient:{patientId}";
    
    // 1. Try to get from Redis (FAST)
    var cachedData = await _redis.StringGetAsync(cacheKey);
    
    if (!cachedData.IsNullOrEmpty)
    {
        // Cache HIT - Return immediately
        return JsonSerializer.Deserialize<Patient>(cachedData);
    }
    
    // 2. Cache MISS - Get from PostgreSQL (SLOW)
    var patient = await _dbContext.Patients.FindAsync(patientId);
    
    // 3. Store in Redis for next time (1 hour expiry)
    var json = JsonSerializer.Serialize(patient);
    await _redis.StringSetAsync(cacheKey, json, TimeSpan.FromHours(1));
    
    return patient;
}
```

### Example 4: Session Management

```csharp
// Store user session
var session = new {
    UserId = 123,
    FormProgress = "page-3",
    StartedAt = DateTime.UtcNow
};

string sessionKey = $"session:{sessionId}";
string sessionJson = JsonSerializer.Serialize(session);

// Store with 30-minute expiry
await _redis.StringSetAsync(sessionKey, sessionJson, TimeSpan.FromMinutes(30));

// Retrieve session
var sessionData = await _redis.StringGetAsync(sessionKey);
if (!sessionData.IsNullOrEmpty)
{
    var userSession = JsonSerializer.Deserialize<dynamic>(sessionData);
    // User can continue from where they left off
}
```

### Example 5: Rate Limiting

```csharp
public async Task<bool> IsRateLimitExceeded(string userId)
{
    string key = $"ratelimit:{userId}";
    
    // Increment counter
    long count = await _redis.StringIncrementAsync(key);
    
    // Set expiry on first request (1 minute window)
    if (count == 1)
    {
        await _redis.KeyExpireAsync(key, TimeSpan.FromMinutes(1));
    }
    
    // Allow max 10 requests per minute
    return count > 10;
}
```

### Example 6: Real-time Counter

```csharp
// Increment form submission counter
await _redis.StringIncrementAsync("stats:total-submissions");

// Get current count
long totalSubmissions = (long)await _redis.StringGetAsync("stats:total-submissions");

// Increment daily counter
string todayKey = $"stats:submissions:{DateTime.UtcNow:yyyy-MM-dd}";
await _redis.StringIncrementAsync(todayKey);
await _redis.KeyExpireAsync(todayKey, TimeSpan.FromDays(30)); // Keep for 30 days
```

---

## 7️⃣ Best Practices

### ✅ DO's

1. **Use Expiry Times**
   ```csharp
   // Always set TTL (Time To Live)
   db.StringSet("key", "value", TimeSpan.FromHours(1));
   ```

2. **Use Meaningful Keys**
   ```csharp
   // Good
   "patient:123:profile"
   "survey:456:responses"
   
   // Bad
   "p123"
   "data"
   ```

3. **Handle Cache Misses**
   ```csharp
   var data = await GetFromRedis();
   if (data == null)
   {
       data = await GetFromDatabase();
       await SaveToRedis(data);
   }
   ```

4. **Use Connection Pooling**
   ```csharp
   // Create once, reuse everywhere
   private static readonly ConnectionMultiplexer redis = 
       ConnectionMultiplexer.Connect("localhost:6379");
   ```

### ❌ DON'Ts

1. **Don't Store Large Objects**
   - Redis is RAM-based (expensive)
   - Keep values < 1MB

2. **Don't Store Sensitive Data Without Encryption**
   - Encrypt before storing in Redis

3. **Don't Use Redis as Primary Database**
   - Always have PostgreSQL as source of truth
   - Redis is for caching/temporary data

4. **Don't Forget Error Handling**
   ```csharp
   try
   {
       await _redis.StringSetAsync(key, value);
   }
   catch (RedisException ex)
   {
       // Fallback to database if Redis fails
       _logger.LogError(ex, "Redis error");
   }
   ```

---

## 8️⃣ Your Health Intake App - Redis Strategy

### What to Cache

```csharp
// ✅ Cache these (frequently accessed, rarely changed)
- Patient profile data
- Country/State/City lists
- Survey templates
- Configuration settings

// ❌ Don't cache these (frequently changed, critical data)
- Survey responses (save directly to PostgreSQL)
- Payment information
- Audit logs
```

### Caching Strategy

```
User Flow:
1. User fills landing page → Save to PostgreSQL + Cache in Redis (1 hour)
2. User fills survey → Save to PostgreSQL + Cache in Redis (1 hour)
3. User views review page → Get from Redis (fast) or PostgreSQL (fallback)
4. User submits → Save to PostgreSQL + Clear Redis cache
```

### Redis Keys Structure

```
patient:{patientId}                    → Patient profile
patient:{patientId}:survey             → Survey responses
session:{sessionId}                    → User session
location:countries                     → Country list (cache for 24 hours)
location:{countryCode}:states          → State list
stats:submissions:today                → Daily counter
```

---

## 9️⃣ Performance Comparison

### Without Redis (PostgreSQL Only)
```
Request → PostgreSQL Query (50ms) → Response
Total: 50ms per request
```

### With Redis (Cached)
```
Request → Redis Check (1ms) → Response
Total: 1ms per request (50x faster!)
```

### Real-World Impact
```
1000 users accessing patient data:
- Without Redis: 1000 × 50ms = 50 seconds of DB load
- With Redis: 1000 × 1ms = 1 second of cache load

Result: 50x less database load, 50x faster response
```

---

## 🎯 Summary

### Redis in Simple Terms
- **What**: Super-fast temporary storage in RAM
- **Why**: Speed up your app, reduce database load
- **When**: Cache frequently accessed data
- **Where**: Between your API and PostgreSQL

### Your Implementation
```
Angular → .NET API → Redis (Cache) → PostgreSQL (Database)
                      ↑ Fast          ↑ Permanent
```

### Key Takeaways
1. Redis is NOT a replacement for PostgreSQL
2. Redis is for TEMPORARY/CACHED data
3. PostgreSQL is for PERMANENT data
4. Use Redis to make your app FASTER
5. Always have fallback to PostgreSQL

---

## 📚 Additional Resources

- Official Docs: https://redis.io/docs/
- .NET Client: https://stackexchange.github.io/StackExchange.Redis/
- Redis Commands: https://redis.io/commands/
- Try Redis Online: https://try.redis.io/

---

**End of Document**
