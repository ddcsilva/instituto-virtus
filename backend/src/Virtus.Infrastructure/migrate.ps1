# Script PowerShell para criar e aplicar migrações

# Navega para o diretório da API
Set-Location ../Virtus.API

# Cria a migração inicial
dotnet ef migrations add InitialCreate -p ../Virtus.Infrastructure -s . -o Data/Migrations

# Aplica a migração
dotnet ef database update -p ../Virtus.Infrastructure -s .

Write-Host "Migração criada e aplicada com sucesso!" -ForegroundColor Green