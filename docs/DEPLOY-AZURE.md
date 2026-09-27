# Deploy Narendra4News-Claude independently

Use a NEW repository `Vinay21470/Narendra4News-Claude` and resource group `rg-n4n-claude`.
Do not use the existing Narendra4News application's database, storage, deployment identity or secrets.

## Architecture and images

React runs in Azure Static Web Apps; ASP.NET Core runs in a separate Linux App Service.
Azure SQL stores content, accounts and media metadata. A private Blob container stores image bytes.
Featured images and the rich-text toolbar image button accept local JPG/PNG/WEBP files up to 8 MB.
The API checks the size and signature, uploads the image, and returns a stable API URL.
Readers load images through `/api/media/files/{generated-name}` without a login. The underlying container stays private.
Anyone holding an uploaded image URL can view the image, including an image uploaded for a draft; uploads are publishing assets, not confidential documents.

## 1. Upload and validate

Create a new repository and upload the contents of this folder at its root. Include `.github` and `.gitignore`.
Run the `Build and test` workflow. Do not deploy until it passes the SQL Server and Azurite integration test.
That test validates migrations, admin login, upload authorization/signatures, private storage, anonymous image retrieval, draft privacy and article publication.
Node 24 and .NET 9 are used by the workflows. Initial EF migrations are included; do NOT generate a second InitialCreate migration.

## 2. Provision (Cloud Shell Bash)

Select the intended subscription, then clone your NEW repository:

```bash
git clone https://github.com/Vinay21470/Narendra4News-Claude.git
cd Narendra4News-Claude
az group create --name rg-n4n-claude --location centralindia
```

Register Microsoft.Web, Microsoft.Sql, Microsoft.Storage, Microsoft.Insights and Microsoft.OperationalInsights if needed.
The B1 plan and Basic database are paid resources that consume trial credit. Static Web Apps uses Free.

Enter the SQL password as a separate command (16+ characters; upper/lowercase, digits and symbols). Save it privately:

```bash
read -rsp 'SQL password: ' N4NC_SQL_PASSWORD
```

Then generate and deploy with a JWT signing secret:

```bash
N4NC_JWT_SECRET=$(openssl rand -hex 32)
az deployment group create --name n4nc-infrastructure --resource-group rg-n4n-claude --template-file infra/main.bicep --parameters prefix=n4ncvinay21470 sqlAdminPassword="$N4NC_SQL_PASSWORD" jwtSecret="$N4NC_JWT_SECRET" --query properties.outputs
unset N4NC_SQL_PASSWORD N4NC_JWT_SECRET
```

Keep the SAME JWT secret on future infrastructure redeployments (retrieve it securely from your own settings); changing it invalidates existing sessions.
The template creates the frontend in `eastasia`, because Static Web Apps region availability differs from App Service. Override `frontendLocation` if your subscription cannot use that region.
Save the output URLs/names. Set `PUBLIC_API_URL` in the API App Service to the returned `apiUrl` if it differs from the generated default. The template sets CORS to the frontend URL.

## 3. Seed admin

In the NEW API App Service -> Environment variables add `ADMIN_EMAIL` and a strong unique `ADMIN_PASSWORD`.
The template already sets `AZURE_SQL_CONNECTION_STRING`, `AZURE_STORAGE_CONNECTION_STRING`, `AZURE_STORAGE_CONTAINER`, `JWT_SECRET`, `PUBLIC_API_URL`, and `CORS_ALLOWED_ORIGINS`.
No Key Vault URI is needed for this deployment. If using Key Vault later, use App Service Key Vault references rather than underscore-containing Key Vault secret names.
After first successful admin login remove the two ADMIN settings. The account remains in SQL.

## 4. GitHub authentication and settings

Create a NEW Entra app registration `n4n-claude-github-deploy` and a federated credential for GitHub repository `Vinay21470/Narendra4News-Claude`, entity type Environment, environment `production`.
Use the new repository's numeric ID (NOT the ID of Narendra4News) and owner ID `72136152` when Azure asks for IDs. Retrieve the repository ID from https://api.github.com/repos/Vinay21470/Narendra4News-Claude .
Assign Website Contributor to this app registration at the NEW API App Service scope.

Create GitHub environment `production`. Add repository secrets:

- AZURE_CLIENT_ID: the NEW deployment app's application/client ID
- AZURE_TENANT_ID: your Azure tenant ID
- AZURE_SUBSCRIPTION_ID: the subscription that owns the new resources
- AZURE_STATIC_WEB_APPS_API_TOKEN: copy from NEW Static Web App -> Manage deployment token

Add repository variables:

- AZURE_WEBAPP_NAME: returned `webAppName` (normally n4ncvinay21470-api)
- VITE_API_BASE_URL: returned API URL plus `/api`

## 5. Deploy and check

Actions -> Deploy Claude version to Azure -> Run workflow -> main.
Validation runs first, then the API deployment, then frontend deployment.
Check API `/health`, then open the FRONTEND URL, sign in at `/login`, and open `/admin/articles/create`.
Choose a local featured image, add an inline image using the editor image icon, save a draft, reopen it, and publish.
Check the public article in an incognito window. Do not enter Blob URLs manually.
If a deployment fails, inspect its failed step. If the app fails to start, inspect the API App Service Log stream.

## Limits

This is a deployment candidate, not a full production security audit. Password reset/email verification, rate limiting, least-privilege SQL access, restricted SQL networking and controlled schema deployment still need work. Startup applies migrations; keep one API instance during this initial rollout. The SQL template uses an administrator connection and the Azure-services firewall exception.
Rendered article HTML is sanitized in the frontend; full server-side content policy and validation remain future work. Analytics concurrency/deduplication and scheduling beyond the existing publish-date filtering require additional testing.
