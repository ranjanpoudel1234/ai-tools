# Setup Inventory Hub for Local BFF Development

This command provides instructions to configure the Inventory Hub micro-frontend to work with a local BFF for development and testing.

## Prerequisites

- Local BFF running at `https://localhost:58262/` (or your custom port)
- Gateway, Host, and Vehicle Sublet remote installed with dependencies

## Configuration Changes

### 1. Gateway Configuration - appsettings.json

**File:** `gateway/appsettings.json`

**Change:** Comment out the "East" destination for SubletBff to prevent load balancing with dev environment.

**Before:**
```json
"SubletBff": {
  "Destinations": {
    "East": {
      "Address": "https://subletbff-service-dev.trafficmanager.net/"
    }
  }
}
```

**After:**
```json
"SubletBff": {
  "Destinations": {
    // "East": {
    //   "Address": "https://subletbff-service-dev.trafficmanager.net/"
    // }
  }
}
```

**Important:** Remember to uncomment this before committing! This file is tracked by git.

---

### 2. Gateway Configuration - Remove VIM Scope (Optional but Recommended)

**File:** `gateway/appsettings.json`

**Change:** Comment out "VIM" from the Scope array if present. This prevents authentication issues during local development.

**Before:**
```json
"OpenIdConfig": {
  "Scope": [
    "openid",
    "profile",
      "orgidentity",
    "VIM",
    "offline_access"
  ]
}
```

**After:**
```json
"OpenIdConfig": {
  "Scope": [
    "openid",
    "profile",
      "orgidentity",
    // "VIM",  // Commented out for local dev
    "offline_access"
  ]
}
```

**Important:** Remember to uncomment VIM before committing! This file is tracked by git.

---

### 3. Gateway Configuration - appsettings.Development.json

**File:** `gateway/appsettings.Development.json`

**Create this file if it doesn't exist.** This file is gitignored and safe for local dev credentials.

**Full Configuration:**
```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.AspNetCore": "Information",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Yarp.ReverseProxy": "Debug",
      "Gateway": "Debug"
    }
  },
  "OpenIdConfig": {
    "Scope": [
      "openid",
      "profile",
      "orgidentity",
      "offline_access"
    ],
    "Authority": "https://loginqa.example.com",
    "ClientId": "YOUR_CLIENT_ID_HERE",
    "ClientSecret": "YOUR_CLIENT_SECRET_HERE",
    "CallbackPath": "/api/auth/callback/orgPing",
    "RoleClaimType": "org.role"
  },
  "CertificateProvider": {
    "Use": "Store"
  },
  "ReverseProxy": {
    "Routes": {
      "mfe1_route": {
        "Match": {
          "Path": "/remotes/vehicle_sublet/{**catch-all}"
        },
        "ClusterId": "vehicle_sublet_cluster"
      }
    },
    "Clusters": {
      "vehicle_sublet_cluster": {
        "Destinations": {
          "vehicle_sublet": {
            "Address": "http://localhost:3012"
          }
        }
      },
      "SubletBff": {
        "Destinations": {
          "Local": {
            "Address": "https://localhost:58262/"
          }
        }
      }
    }
  },
  "RemoteAccess:VehicleSublet": "[6008,7291,7103]",
  "RemoteAccess:VehicleSubletAllLocations": false,
  "RemoteAccess:ShippingAndReceivingAllLocations": false,
  "RemoteAccess:ShippingAndReceiving": "[YOUR_STORE_NUMBERS_HERE]"
}
```

**Replace:**
- `YOUR_CLIENT_ID_HERE` with your actual OIDC Client ID (ask team for this)
- `YOUR_CLIENT_SECRET_HERE` with your actual OIDC Client Secret
- `https://localhost:58262/` with your local BFF URL (if different)
- `[YOUR_STORE_NUMBERS_HERE]` with allowed store numbers if needed

---

### 4. VS Code Settings - .vscode/settings.json

**File:** `.vscode/settings.json`

**Add Jest Configuration** to handle monorepo test discovery:

```json
{
  // ... existing settings ...
  "jest.autoRun": "off",
  "jest.disabledWorkspaceFolders": [
    "host",
    "gateway",
    "shared",
    "shipping_receiving",
    "vehicle_preparation"
  ]
}
```

This configures the Jest VS Code extension to only run tests for vehicle_sublet.

---

### 5. NPM Configuration - .npmrc (Optional)

**File:** `.npmrc`

**No changes required for local BFF testing.**

If you encounter Artifactory authentication issues during `pnpm install`, add:
```
@org:registry=https://example.jfrog.io/artifactory/api/npm/npm/
//example.jfrog.io/artifactory/api/npm/npm/:_auth=YOUR_BASE64_ENCODED_CREDENTIALS
//example.jfrog.io/artifactory/api/npm/npm/:always-auth=true
```

---

## Starting the Application

### 1. Start your local BFF
```bash
# In your BFF project directory
dotnet run
# Should be listening at https://localhost:58262/
```

### 2. Start the Gateway
```bash
cd gateway
dotnet run
# Should start at http://localhost:5299
```

### 3. Start the Frontend
```bash
# From project root
pnpm start:frontend
# Starts Host (3000) + all remotes (3010, 3011, 3012)
```

### 4. Open the Application
```
http://localhost:3000
```

---

## Verification

### Check Gateway is Routing to Local BFF

1. Open `http://localhost:3000`
2. Open DevTools → Network tab
3. Navigate to Vehicle Sublet
4. Look for requests to `/api/sublet-queue/...`
5. Check Gateway console output - should show:
   ```
   Proxying to https://localhost:58262/api/sublet-queue/...
   ```
6. **Should NOT show:** `Proxying to https://subletbff-service-dev.trafficmanager.net/...`

### Check BFF is Receiving Requests

1. Set breakpoint in your local BFF controller
2. Perform action in UI (open drawer, save changes)
3. Breakpoint should be hit
4. Verify request payload has new fields (isWarranty, isAtCustomerLocation)

---

## Troubleshooting

### Gateway fails to start
- **Missing appsettings.Development.json:** Create the file with OIDC credentials
- **Invalid credentials:** Get correct ClientId/ClientSecret from your team
- **Port 5299 in use:** Kill the process or change port in launchSettings.json

### Requests going to dev BFF instead of local
- **Check appsettings.json:** Ensure "East" destination is commented out
- **Restart Gateway:** Kill and restart after config changes
- **Check Gateway logs:** Should only show "Destination 'Local' has been added" for SubletBff

### Frontend can't connect to Gateway
- **ECONNREFUSED errors:** Gateway isn't running - start it
- **CORS errors:** Check Gateway CORS policy
- **Auth errors:** Clear cookies and localStorage, try again

### Certificate errors
- **MTLS errors:** Install certificate to Windows Certificate Store (Current User → Personal)
- **SSL errors from BFF:** Ensure your local BFF has valid SSL cert or disable validation temporarily

---

## Cleanup Before Committing

**IMPORTANT:** Before committing your changes:

1. ✅ **Uncommit appsettings.json changes**
   ```bash
   git restore gateway/appsettings.json
   ```
   Both the East destination AND the VIM scope should be restored to their original state.

2. ✅ **Keep appsettings.Development.json local**
   Already gitignored - safe to keep

3. ✅ **Keep .vscode/settings.json changes**
   Can be committed - helps other devs

4. ✅ **Remove any console.log debugging**
   Clean up debug code before PR

---

## Quick Start (TL;DR)

```bash
# 1. Start local BFF (in BFF project)
dotnet run

# 2. Comment out East destination in gateway/appsettings.json
# (See step 1 above)

# 3. Comment out VIM scope in gateway/appsettings.json if present
# (See step 2 above)

# 4. Create gateway/appsettings.Development.json with OIDC + Local BFF config
# (See step 3 above)

# 5. Start Gateway
cd gateway && dotnet run

# 6. Start Frontend (new terminal)
pnpm start:frontend

# 7. Open browser
# http://localhost:3000
```

All `/api/sublet-queue/*` requests will now route to your local BFF!
