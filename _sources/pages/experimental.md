# Experimental Functionalities

![5](images/5.jpg)

Access Forge is actively being extended with experimental features. One such addition is the Locator Service, which captures device or IP address information during login. This feature is still under development and has not been thoroughly tested.

## Locator Service
### Step 1: Register IP Configuration Key
```csharp
services.AddIPInfoConfiguration("a9e6713beb5988");
```
### Step 2: Authenticate User
```csharp
SignInResult<User> result = signInManager.SignInByEmail(email, password);
```

```{warning}
This feature is experimental and subject to change. Avoid using it in production without thorough testing
```