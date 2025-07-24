# Advanced Functionalities and Features
## Register Custom User and Role Models

```csharp
services.AddIdentityService<IdentityDbContext, User, Role>(options =>
{
    options.RegisterServicesFromAssembly(typeof(Program).Assembly);
    options.UseSqlServer(connectionString);
});

```

## Register Google Authenticator Options
```csharp
services.AddGoogleAuthenticator(s =>
{
    s.Issuer = "AdeNote";
    s.AutheticatorKey = Encoding.ASCII.GetBytes(tokenSecret);
});

```

## Add Password Policy (Optional)
```csharp
services.AddIdentityRule(s =>
{
    s.IsRequireEmailConfirmation = true;
    s.Password = new PasswordRule
    {
        MaximumPasswordLength = 10,
        MinimumPasswordLength = 3,
        HasCapitalLetter = true,
        HasNumber = true,
        HasSmallLetter = true,
        HasSpecialNumber = true,
    };
});

```