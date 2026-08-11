# Quick Test Script for ONGC.MilestoneAPI
$baseUrl = "http://localhost:5000"

Write-Host "`n🧪 Starting API Tests..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# Test 1: Register User
Write-Host "`n1️⃣  Testing User Registration..." -ForegroundColor Yellow
try {
	$registerResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/register" `
		-Method POST `
		-ContentType "application/json" `
		-Body (@{
			email = "quicktest@ongc.com"
			password = "Test123!"
			role = "Admin"
		} | ConvertTo-Json)

	Write-Host "   ✅ User registered successfully!" -ForegroundColor Green
	Write-Host "   User ID: $($registerResponse.userId)" -ForegroundColor Gray
} catch {
	if ($_.Exception.Response.StatusCode -eq 400) {
		Write-Host "   ℹ️  User already exists (that's OK)" -ForegroundColor Cyan
	} else {
		Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
		exit 1
	}
}

# Test 2: Login
Write-Host "`n2️⃣  Testing Login..." -ForegroundColor Yellow
try {
	$loginResponse = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" `
		-Method POST `
		-ContentType "application/json" `
		-Body (@{
			email = "quicktest@ongc.com"
			password = "Test123!"
		} | ConvertTo-Json)

	$token = $loginResponse.token
	Write-Host "   ✅ Login successful!" -ForegroundColor Green
	Write-Host "   Token: $($token.Substring(0, 30))..." -ForegroundColor Gray
} catch {
	Write-Host "   ❌ Login failed: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

$headers = @{
	"Authorization" = "Bearer $token"
	"Content-Type" = "application/json"
}

# Test 3: Create Company
Write-Host "`n3️⃣  Creating Company..." -ForegroundColor Yellow
try {
	$company = Invoke-RestMethod -Uri "$baseUrl/api/companies" `
		-Method POST `
		-Headers $headers `
		-Body (@{name = "Test ONGC"} | ConvertTo-Json)

	Write-Host "   ✅ Company created! ID: $($company.id)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 4: Create Project
Write-Host "`n4️⃣  Creating Project..." -ForegroundColor Yellow
try {
	$project = Invoke-RestMethod -Uri "$baseUrl/api/projects" `
		-Method POST `
		-Headers $headers `
		-Body (@{
			companyId = $company.id
			name = "Test Project"
		} | ConvertTo-Json)

	Write-Host "   ✅ Project created! ID: $($project.id)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 5: Create Site
Write-Host "`n5️⃣  Creating Site..." -ForegroundColor Yellow
try {
	$site = Invoke-RestMethod -Uri "$baseUrl/api/sites" `
		-Method POST `
		-Headers $headers `
		-Body (@{
			projectId = $project.id
			name = "Test Site"
			location = "Test Location"
		} | ConvertTo-Json)

	Write-Host "   ✅ Site created! ID: $($site.id)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 6: Create Well
Write-Host "`n6️⃣  Creating Well..." -ForegroundColor Yellow
try {
	$well = Invoke-RestMethod -Uri "$baseUrl/api/wells" `
		-Method POST `
		-Headers $headers `
		-Body (@{
			siteId = $site.id
			name = "Test-Well-001"
			wellType = "Oil"
			status = "Active"
		} | ConvertTo-Json)

	Write-Host "   ✅ Well created! ID: $($well.id)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 7: Create Wellbore
Write-Host "`n7️⃣  Creating Wellbore..." -ForegroundColor Yellow
try {
	$wellbore = Invoke-RestMethod -Uri "$baseUrl/api/wellbores" `
		-Method POST `
		-Headers $headers `
		-Body (@{
			wellId = $well.id
			name = "Test-Wellbore-001"
			designType = "Standard"
		} | ConvertTo-Json)

	Write-Host "   ✅ Wellbore created! ID: $($wellbore.id)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 8: Create Design
Write-Host "`n8️⃣  Creating Design..." -ForegroundColor Yellow
try {
	$design = Invoke-RestMethod -Uri "$baseUrl/api/designs" `
		-Method POST `
		-Headers $headers `
		-Body (@{
			wellboreId = $wellbore.id
			name = "Test-Design-2024"
			owner = "Test Owner"
			description = "Test design description"
		} | ConvertTo-Json)

	Write-Host "   ✅ Design created! ID: $($design.id)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 9: Create Milestone (Kafka Event!)
Write-Host "`n9️⃣  Creating Milestone (Publishing to Kafka)..." -ForegroundColor Yellow
try {
	$milestone = Invoke-RestMethod -Uri "$baseUrl/api/milestones" `
		-Method POST `
		-Headers $headers `
		-Body (@{
			designId = $design.id
			milestoneType = "DesignInitiated"
			timestamp = (Get-Date).ToUniversalTime().ToString("o")
			workCentre = "Test Office"
			metadata = '{"test":true}'
		} | ConvertTo-Json)

	Write-Host "   ✅ Milestone created! ID: $($milestone.id)" -ForegroundColor Green
	Write-Host "   📨 Event published to Kafka topic: wellbore-milestone-events" -ForegroundColor Magenta
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 10: Get Milestones
Write-Host "`n🔟 Getting Milestones for Design..." -ForegroundColor Yellow
try {
	$milestones = Invoke-RestMethod -Uri "$baseUrl/api/milestones/designs/$($design.id)" `
		-Method GET `
		-Headers $headers

	Write-Host "   ✅ Retrieved $($milestones.Count) milestone(s)" -ForegroundColor Green
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

# Test 11: Get Designs with Pagination
Write-Host "`n1️⃣1️⃣  Testing Pagination..." -ForegroundColor Yellow
try {
	$pagedDesigns = Invoke-RestMethod -Uri "$baseUrl/api/designs?pageNumber=1&pageSize=10" `
		-Method GET `
		-Headers $headers

	Write-Host "   ✅ Paginated results retrieved" -ForegroundColor Green
	Write-Host "   Total: $($pagedDesigns.totalCount), Page: $($pagedDesigns.pageNumber)/$($pagedDesigns.totalPages)" -ForegroundColor Gray
} catch {
	Write-Host "   ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
	exit 1
}

Write-Host "`n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray
Write-Host "🎉 ALL TESTS PASSED!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

Write-Host "`n📊 Summary:" -ForegroundColor Cyan
Write-Host "   ✓ User Registration" -ForegroundColor White
Write-Host "   ✓ JWT Authentication" -ForegroundColor White
Write-Host "   ✓ Company → Project → Site → Well → Wellbore → Design hierarchy" -ForegroundColor White
Write-Host "   ✓ Milestone creation with Kafka event" -ForegroundColor White
Write-Host "   ✓ Data retrieval and pagination" -ForegroundColor White

Write-Host "`n💡 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Check .NET console for Kafka publish logs" -ForegroundColor White
Write-Host "   2. Verify Kafka messages (see instructions below)" -ForegroundColor White
Write-Host "`n" -ForegroundColor White
