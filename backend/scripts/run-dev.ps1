Write-Host "🚀 Iniciando Instituto Virtus Backend..." -ForegroundColor Green

# Navegar para o diretório da API
Set-Location src\InstitutoVirtus.API

# Restaurar pacotes
Write-Host "📦 Restaurando pacotes..." -ForegroundColor Yellow
dotnet restore

# Aplicar migrations
Write-Host "🗄️ Aplicando migrations..." -ForegroundColor Yellow
dotnet ef database update

# Executar aplicação
Write-Host "✅ Iniciando API..." -ForegroundColor Green
dotnet run

# API disponível em:
# - http://localhost:5000
# - https://localhost:5001
# - Swagger: http://localhost:5000/swagger