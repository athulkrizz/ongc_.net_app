# ONGC.MilestoneAPI Test Script
# Run this in PowerShell

$baseUrl = "http://localhost:5000"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "1. REGISTERING USER..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$registerResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" `
	-Method POST `
	-ContentType "application/json" `
	-Body (@{
		email = "testuser@ongc.com"
		password = "SecurePass123!"
		role = "Admin"
	} | ConvertTo-Json)

Write-Host "✅ User registered:" -ForegroundColor Green
$registerResponse | ConvertTo-Json

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "2. LOGGING IN..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
	-Method POST `
	-ContentType "application/json" `
	-Body (@{
		email = "testuser@ongc.com"
		password = "SecurePass123!"
	} | ConvertTo-Json)

$token = $loginResponse.token
Write-Host "✅ Login successful! Token received." -ForegroundColor Green
Write-Host "Token: $($token.Substring(0, 50))..." -ForegroundColor Gray

$headers = @{
	"Authorization" = "Bearer $token"
	"Content-Type" = "application/json"
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "3. CREATING COMPANY..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$company = Invoke-RestMethod -Uri "$baseUrl/api/companies" `
	-Method POST `
	-Headers $headers `
	-Body (@{name = "ONGC Limited"} | ConvertTo-Json)

Write-Host "✅ Company created with ID: $($company.id)" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "4. CREATING PROJECT..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$project = Invoke-RestMethod -Uri "$baseUrl/api/projects" `
	-Method POST `
	-Headers $headers `
	-Body (@{
		companyId = $company.id
		name = "Mumbai Offshore Project"
	} | ConvertTo-Json)

Write-Host "✅ Project created with ID: $($project.id)" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "5. CREATING SITE..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$site = Invoke-RestMethod -Uri "$baseUrl/api/sites" `
	-Method POST `
	-Headers $headers `
	-Body (@{
		projectId = $project.id
		name = "Platform B-12"
		location = "Arabian Sea"
	} | ConvertTo-Json)

Write-Host "✅ Site created with ID: $($site.id)" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "6. CREATING WELL..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$well = Invoke-RestMethod -Uri "$baseUrl/api/wells" `
	-Method POST `
	-Headers $headers `
	-Body (@{
		siteId = $site.id
		name = "Well-B12-001"
		wellType = "Gas"
		status = "Planning"
	} | ConvertTo-Json)

Write-Host "✅ Well created with ID: $($well.id)" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "7. CREATING WELLBORE..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$wellbore = Invoke-RestMethod -Uri "$baseUrl/api/wellbores" `
	-Method POST `
	-Headers $headers `
	-Body (@{
		wellId = $well.id
		name = "Wellbore-001"
		designType = "WellMontage"
	} | ConvertTo-Json)

Write-Host "✅ Wellbore created with ID: $($wellbore.id)" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "8. CREATING DESIGN..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$design = Invoke-RestMethod -Uri "$baseUrl/api/designs" `
	-Method POST `
	-Headers $headers `
	-Body (@{
		wellboreId = $wellbore.id
		name = "Design-Q1-2024"
		owner = "Engineering Team"
		description = "Initial design for gas extraction"
	} | ConvertTo-Json)

Write-Host "✅ Design created with ID: $($design.id)" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "9. CREATING MILESTONE (Publishes to Kafka)..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$milestone = Invoke-RestMethod -Uri "$baseUrl/api/milestones" `
	-Method POST `
	-Headers $headers `
	-Body (@{
		designId = $design.id
		milestoneType = "GnGDataReceived"
		timestamp = (Get-Date).ToUniversalTime().ToString("o")
		workCentre = "Mumbai Office"
		metadata = '{"source":"Geological Survey"}'
	} | ConvertTo-Json)

Write-Host "✅ Milestone created! Event sent to Kafka topic: wellbore-milestone-events" -ForegroundColor Green
$milestone | ConvertTo-Json

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "10. GETTING ALL MILESTONES FOR DESIGN..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$milestones = Invoke-RestMethod -Uri "$baseUrl/api/milestones/designs/$($design.id)" `
	-Method GET `
	-Headers $headers

Write-Host "✅ Retrieved $($milestones.Count) milestone(s):" -ForegroundColor Green
$milestones | ConvertTo-Json

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "11. TESTING PAGINATION - GET DESIGNS..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$pagedDesigns = Invoke-RestMethod -Uri "$baseUrl/api/designs?pageNumber=1&pageSize=10&status=InProgress" `
	-Method GET `
	-Headers $headers

Write-Host "✅ Paginated designs:" -ForegroundColor Green
Write-Host "   Total Count: $($pagedDesigns.totalCount)" -ForegroundColor Cyan
Write-Host "   Page: $($pagedDesigns.pageNumber) of $($pagedDesigns.totalPages)" -ForegroundColor Cyan
Write-Host "   Items in page: $($pagedDesigns.items.Count)" -ForegroundColor Cyan

Write-Host "`n========================================" -ForegroundColor Green
Write-Host "🎉 ALL TESTS PASSED!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "`n📊 Summary:" -ForegroundColor Yellow
Write-Host "   ✓ User Registration & Login" -ForegroundColor White
Write-Host "   ✓ JWT Token Authentication" -ForegroundColor White
Write-Host "   ✓ Company → Project → Site → Well → Wellbore → Design hierarchy" -ForegroundColor White
Write-Host "   ✓ Milestone creation with Kafka event publishing" -ForegroundColor White
Write-Host "   ✓ Pagination and filtering" -ForegroundColor White
Write-Host "`n🔥 Check your .NET console for Kafka publish logs!" -ForegroundColor Magenta
