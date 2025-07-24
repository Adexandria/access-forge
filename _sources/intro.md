# Introduction
![table-tennis](intro.jpg)

## Overview  
This package provides an authentication system developed in C# using .NET 8. It includes core components such as the token provider, password manager, user manager, and sign-in manager.

## Purpose  
Access Forge saves development time by eliminating the need to build an authentication system from scratch. Developers can focus on core application requirements instead of repeatedly implementing the same functionality. Access Forge offers a flexible and customizable authentication solution that seamlessly integrates with a single method call.

---

## Key Features
These are the key features that exist in access-forge 1.0.0

````{tab-set}
```{tab-item} Token Provider
 Generates JWTs using claims, random tokens, numeric OTPs, verifies OTPs, and reads tokens. 
 ```

```{tab-item} Password Manager
 Securely generates and verifies passwords.
```

```{tab-item} Multi-Factor Authentication
 Uses Google Authenticator to generate and verify TOTP codes and supports QR code generation. 
```

```{tab-item} User Manager
Handles actions like user creation, name changes, password updates, and email confirmation.  
```

```{tab-item} Sign-in Manager
Manages the user sign-in process, including authentication and account status checks.
```
````