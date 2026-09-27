# Narendra4News-Claude

Separate deployment candidate based on the uploaded Claude project.

Read [the deployment guide](docs/DEPLOY-AZURE.md) for the NEW repository and Azure resources.

## Fixed in this revision

- Local featured-image and inline-image upload, with progress/error handling and save blocking while uploads run.
- Private Blob storage served via the public image API; SQL stores metadata and image references.
- Image type/signature and size checks, generated filenames, and preview/removal controls.
- Missing backend imports/framework reference, JSON enum serialization and consistent error JSON casing.
- Committed EF schema migration and design-time context factory; removed conflicting EnsureCreated startup path.
- Admin article detail lookup preserves category, movie, status and SEO fields; public article lookup excludes drafts and future posts.
- Sanitized rich text on the public article page.
- Separate Azure Bicep template, configurable OIDC deployment, Node 24 frontend build and SQL/Azurite regression CI.

No changes are deployed to the existing Narendra4News website by this package.

## Validation

See docs/VALIDATION.md for completed checks and remaining gates. This package is not a claim of production readiness.
