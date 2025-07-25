# Access-forge
This package offers a comprehensive authentication system built with C# on the .NET 8 framework. It encompasses essential components, including a token provider, password manager, user manager, and sign-in manager.

Access-forge streamlines development by providing a customizable, ready-to-use authentication system that integrates seamlessly with a single method call, allowing developers to focus on core application features.

## Installation
Use .Net CLI to install the package

```csharp
dotnet add package access-forge --version 1.0.0
```

## How to use
To use access-forge to authenticate users

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

## Additional Resources

- [Code Documentation](https://adexandria.github.io/access-forge/html/)
- [Project Guide](https://adexandria.github.io/access-forge-docs/intro.html)
