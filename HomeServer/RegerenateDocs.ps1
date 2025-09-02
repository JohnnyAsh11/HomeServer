<#
    .SYNOPSIS
        Generates a static API documentation site.
    .EXAMPLE
        ./Invoke-Redoc.ps1
#>

redocly build-docs HomeServer.OpenApi/openapi.yaml --output=HomeServer.TodoList/wwwroot/index.html