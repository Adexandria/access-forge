# Basic Operations

![4](images/4.jpg)

## Task: User Authentication

### Step 1: Configure Identity and JWT

```csharp
var tokenSecret = "123456AccessForge@!2092000111@@wqrys";
var connectionString = "Server=localhost\\SQLEXPRESS;Database=AccessForge;Trusted_Connection=True;TrustServerCertificate=true;";

services.AddIdentityService<IdentityDbContext>(options =>
{
    options.UseSqlServer(connectionString);
})
.AddJwtBearer(s =>
{
    s.TokenSecret = tokenSecret;
    s.AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;
    s.ExpirationTime = 30;
});

```

### Step 2: Register User

```csharp
AccessResult<User> result = userManager.CreateUser(user);
```

### Step 3: Authenticate User

```csharp
SignInResult<User> result = signInManager.SignInByEmail(email, password);

```
