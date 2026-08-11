# Kafka Event Consumer Test Script
# This script helps you verify that milestone events are being published to Kafka

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "   Kafka Event Consumer Test" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Check if Kafka container is running
Write-Host "🔍 Checking Kafka container..." -ForegroundColor Yellow
$kafkaContainer = docker ps --filter "name=kafka" --format "{{.Names}}"

if (-not $kafkaContainer) {
	Write-Host "❌ Kafka container is not running!" -ForegroundColor Red
	Write-Host "   Run: cd ONGC.MilestoneAPI && docker-compose up -d" -ForegroundColor Yellow
	exit 1
}

Write-Host "✅ Kafka container found: $kafkaContainer" -ForegroundColor Green
Write-Host ""

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Choose an option:" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "1. List all Kafka topics" -ForegroundColor White
Write-Host "2. Consume events from beginning (see all)" -ForegroundColor White
Write-Host "3. Consume latest events (wait for new)" -ForegroundColor White
Write-Host "4. Count total messages in topic" -ForegroundColor White
Write-Host "5. Exit" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Enter choice (1-5)"

switch ($choice) {
	"1" {
		Write-Host ""
		Write-Host "📋 Listing all Kafka topics..." -ForegroundColor Yellow
		docker exec -it $kafkaContainer kafka-topics --bootstrap-server localhost:9092 --list
	}
	"2" {
		Write-Host ""
		Write-Host "📥 Consuming events from beginning..." -ForegroundColor Yellow
		Write-Host "   Topic: wellbore-milestone-events" -ForegroundColor Cyan
		Write-Host "   Press Ctrl+C to stop" -ForegroundColor Yellow
		Write-Host ""
		docker exec -it $kafkaContainer kafka-console-consumer `
			--bootstrap-server localhost:9092 `
			--topic wellbore-milestone-events `
			--from-beginning `
			--property print.timestamp=true `
			--property print.key=true
	}
	"3" {
		Write-Host ""
		Write-Host "📥 Waiting for new events..." -ForegroundColor Yellow
		Write-Host "   Topic: wellbore-milestone-events" -ForegroundColor Cyan
		Write-Host "   Create a milestone in Postman to see events" -ForegroundColor Yellow
		Write-Host "   Press Ctrl+C to stop" -ForegroundColor Yellow
		Write-Host ""
		docker exec -it $kafkaContainer kafka-console-consumer `
			--bootstrap-server localhost:9092 `
			--topic wellbore-milestone-events `
			--property print.timestamp=true `
			--property print.key=true
	}
	"4" {
		Write-Host ""
		Write-Host "🔢 Counting messages..." -ForegroundColor Yellow

		$output = docker exec $kafkaContainer kafka-run-class kafka.tools.GetOffsetShell `
			--broker-list localhost:9092 `
			--topic wellbore-milestone-events `
			--time -1

		if ($output -match ":(\d+)$") {
			$count = $Matches[1]
			Write-Host "✅ Total messages in topic: $count" -ForegroundColor Green
		} else {
			Write-Host "❌ Could not determine message count" -ForegroundColor Red
		}
	}
	"5" {
		Write-Host "👋 Goodbye!" -ForegroundColor Cyan
		exit 0
	}
	default {
		Write-Host "❌ Invalid choice!" -ForegroundColor Red
		exit 1
	}
}

Write-Host ""
Write-Host "✅ Done!" -ForegroundColor Green
